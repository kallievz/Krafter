using System.Collections.Concurrent;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Auth;
using AditiKraft.Krafter.UI.Web.Client.Common.Constants;
using AditiKraft.Krafter.UI.Web.Client.Features.Auth;
using AditiKraft.Krafter.UI.Web.Client.Features.Roles;
using AditiKraft.Krafter.UI.Web.Client.Features.Users;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Auth;
using AditiKraft.Krafter.UI.Web.Infrastructure.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AditiKraft.Krafter.Tests.Auth;

public sealed class ServerRefreshCoordinationTests
{
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(10);

    [Fact]
    public async Task ServerFeatureClientsAndRootScopeShareRefreshAndSaveTokensInEveryRequestAsync()
    {
        TokenResponse original = TokenTestData.Create(expired: true);
        TokenResponse fresh = TokenTestData.Create();
        TaskCompletionSource release = NewSignal();
        var api = new AuthApi(async (_, cancellation) =>
        {
            await release.Task.WaitAsync(_timeout, cancellation);
            return Response<TokenResponse>.Success(fresh);
        });
        var observations = new RefreshObservations();
        var sentTokens = new ConcurrentBag<string?>();
        await using WebApplication backend = await CreateBackendAsync(sentTokens);
        string backendUrl = Assert.Single(backend.Urls);
        await using ServiceProvider provider = CreateProvider(api, observations, sentTokens, backendUrl);

        Task<HttpContext> users = RunRequestAsync(provider, original, services =>
            services.GetRequiredService<IUsersApi>().GetUsersAsync(new GetRequestInput()));
        await api.Started.Task.WaitAsync(_timeout);
        Task<HttpContext> roles = RunRequestAsync(provider, original, services =>
            services.GetRequiredService<IRolesApi>().GetRolesAsync(new GetRequestInput()));
        AuthTokenService? rootTokens = null;
        Task<HttpContext> root = RunRequestAsync(provider, original, async services =>
        {
            rootTokens = services.GetRequiredService<AuthTokenService>();
            Assert.True(await rootTokens.RefreshAsync());
        });

        try
        {
            Assert.Equal(2, observations.Services.Distinct().Count());
            Assert.DoesNotContain(rootTokens!, observations.Services);
            Assert.False(root.IsCompleted);
        }
        finally
        {
            release.TrySetResult();
        }

        HttpContext[] contexts = await Task.WhenAll(users, roles, root).WaitAsync(_timeout);
        Assert.Equal(1, api.RefreshCount);
        Assert.Equal(2, sentTokens.Count);
        Assert.All(sentTokens, token => Assert.Equal(fresh.Token, token));
        Assert.All(contexts, context =>
        {
            Assert.Equal(fresh.Token, context.Items[StorageConstants.Local.AuthToken]);
            Assert.Equal(fresh.RefreshToken, context.Items[StorageConstants.Local.RefreshToken]);
            Assert.Contains(context.Response.Headers.SetCookie,
                cookie => cookie!.StartsWith($"{StorageConstants.Local.RefreshToken}={fresh.RefreshToken};", StringComparison.Ordinal));
        });

        // A later request can still carry the old cookies while the first response is in flight.
        HttpContext late = await RunRequestAsync(provider, original, async services =>
            Assert.True(await services.GetRequiredService<AuthTokenService>().RefreshAsync()));
        Assert.Equal(1, api.RefreshCount);
        Assert.Equal(fresh.RefreshToken, late.Items[StorageConstants.Local.RefreshToken]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task DifferentTokenPairsRefreshIndependentlyAsync(bool sameAccessToken)
    {
        TokenResponse alice = TokenTestData.Create(expired: true);
        TokenResponse bob = sameAccessToken
            ? alice with { RefreshToken = "another-session-refresh-token" }
            : TokenTestData.Create("bob", expired: true);
        TokenResponse freshAlice = TokenTestData.Create();
        TokenResponse freshBob = TokenTestData.Create("bob");
        TaskCompletionSource releaseAlice = NewSignal();
        var api = new AuthApi(async (request, cancellation) =>
        {
            if (request.RefreshToken == alice.RefreshToken)
            {
                await releaseAlice.Task.WaitAsync(_timeout, cancellation);
                return Response<TokenResponse>.Success(freshAlice);
            }

            return Response<TokenResponse>.Success(freshBob);
        });
        await using ServiceProvider provider = CreateProvider(api);
        using IServiceScope firstScope = provider.CreateScope();
        using IServiceScope secondScope = provider.CreateScope();
        Task<Response<TokenResponse>> first = RefreshAsync(firstScope, alice);
        await api.Started.Task.WaitAsync(_timeout);

        try
        {
            Response<TokenResponse> second = await RefreshAsync(secondScope, bob).WaitAsync(_timeout);
            AssertTokensEqual(freshBob, second.Data);
            Assert.False(first.IsCompleted);
        }
        finally
        {
            releaseAlice.TrySetResult();
        }

        AssertTokensEqual(freshAlice, (await first.WaitAsync(_timeout)).Data);
        Assert.Equal(2, api.RefreshCount);
    }

    [Theory]
    [InlineData("error")]
    [InlineData("missing-data")]
    [InlineData("blank-token")]
    public async Task FailedOrIncompleteResponsesCanBeRetriedImmediatelyAsync(string failure)
    {
        TokenResponse original = TokenTestData.Create(expired: true);
        TokenResponse fresh = TokenTestData.Create();
        int calls = 0;
        var api = new AuthApi((_, _) => Task.FromResult(Interlocked.Increment(ref calls) > 1
            ? Response<TokenResponse>.Success(fresh)
            : failure switch
            {
                "error" => new Response<TokenResponse> { IsError = true, StatusCode = 503 },
                "missing-data" => new Response<TokenResponse> { StatusCode = 200 },
                _ => Response<TokenResponse>.Success(fresh with { RefreshToken = " " })
            }));
        await using ServiceProvider provider = CreateProvider(api);
        using IServiceScope firstScope = provider.CreateScope();
        using IServiceScope secondScope = provider.CreateScope();

        await RefreshAsync(firstScope, original);
        Response<TokenResponse> retried = await RefreshAsync(secondScope, original);

        Assert.False(retried.IsError);
        AssertTokensEqual(fresh, retried.Data);
        Assert.Equal(2, api.RefreshCount);
    }

    [Fact]
    public async Task CancelingFirstCallerDoesNotCancelAnotherCallerWaitingForTheSameRefreshAsync()
    {
        TokenResponse original = TokenTestData.Create(expired: true);
        TokenResponse fresh = TokenTestData.Create();
        TaskCompletionSource release = NewSignal();
        CancellationToken refreshCancellation = default;
        var api = new AuthApi(async (_, cancellation) =>
        {
            refreshCancellation = cancellation;
            await release.Task.WaitAsync(_timeout, cancellation);
            return Response<TokenResponse>.Success(fresh);
        });
        await using ServiceProvider provider = CreateProvider(api);
        using IServiceScope firstScope = provider.CreateScope();
        using IServiceScope secondScope = provider.CreateScope();
        using var cancellationSource = new CancellationTokenSource();
        Task<Response<TokenResponse>> first = RefreshAsync(firstScope, original, cancellationSource.Token);
        await api.Started.Task.WaitAsync(_timeout);
        Task<Response<TokenResponse>> second = RefreshAsync(secondScope, original);

        try
        {
            cancellationSource.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => first.WaitAsync(_timeout));
            Assert.False(refreshCancellation.IsCancellationRequested);
            Assert.False(second.IsCompleted);
        }
        finally
        {
            release.TrySetResult();
        }

        AssertTokensEqual(fresh, (await second.WaitAsync(_timeout)).Data);
        Assert.Equal(2, api.RefreshCount);
    }

    private static ServiceProvider CreateProvider(AuthApi api, RefreshObservations? observations = null,
        ConcurrentBag<string?>? sentTokens = null, string? apiBaseUrl = null)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Urls:RootUiUrl"] = "https://localhost:7291",
                ["Urls:ApiBaseUrl"] = apiBaseUrl ?? "https://localhost:5001"
            }).Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(configuration);
        services.AddUiHostServices(configuration);
        services.RemoveAll<IDistributedCache>();
        services.AddDistributedMemoryCache();
        services.AddSingleton<IAuthApi>(api);
        if (observations is not null)
        {
            services.AddSingleton(observations);
            services.AddScoped<IAuthenticationService, ForwardingAuthenticationService>();
        }

        return services.BuildServiceProvider();
    }

    private static async Task<HttpContext> RunRequestAsync(ServiceProvider provider, TokenResponse tokens,
        Func<IServiceProvider, Task> action)
    {
        using IServiceScope scope = provider.CreateScope();
        var context = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("localhost", 5002);
        context.Request.Headers.Cookie = string.Join("; ",
            $"{StorageConstants.Local.AuthToken}={tokens.Token}",
            $"{StorageConstants.Local.RefreshToken}={tokens.RefreshToken}");
        IHttpContextAccessor accessor = provider.GetRequiredService<IHttpContextAccessor>();
        accessor.HttpContext = context;
        try
        {
            await action(scope.ServiceProvider);
            return context;
        }
        finally
        {
            accessor.HttpContext = null;
        }
    }

    private static Task<Response<TokenResponse>> RefreshAsync(IServiceScope scope, TokenResponse tokens,
        CancellationToken cancellationToken = default) =>
        scope.ServiceProvider.GetRequiredService<IAuthApiService>().RefreshTokenAsync(
            new RefreshTokenRequest { Token = tokens.Token, RefreshToken = tokens.RefreshToken }, cancellationToken);

    private static void AssertTokensEqual(TokenResponse expected, TokenResponse? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.Token, actual.Token);
        Assert.Equal(expected.RefreshToken, actual.RefreshToken);
        Assert.Equal(expected.TokenExpiryTime, actual.TokenExpiryTime);
        Assert.Equal(expected.RefreshTokenExpiryTime, actual.RefreshTokenExpiryTime);
        Assert.Equal(expected.Permissions, actual.Permissions);
    }

    private static TaskCompletionSource NewSignal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private sealed class RefreshObservations
    {
        public ConcurrentBag<AuthTokenService> Services { get; } = [];
    }

    private sealed class ForwardingAuthenticationService(AuthTokenService tokens, RefreshObservations observations)
        : IAuthenticationService
    {
        public event Action<string?>? LoginChange
        { add { } remove { } }

        public Task<bool> RefreshAsync()
        {
            observations.Services.Add(tokens);
            return tokens.RefreshAsync();
        }

        public Task<bool> LoginAsync(TokenRequest model) => throw new NotSupportedException();

        public Task LogoutAsync(string methodName) => throw new NotSupportedException();

        public Task<bool> SynchronizeAsync() => throw new NotSupportedException();
    }

    private static async Task<WebApplication> CreateBackendAsync(ConcurrentBag<string?> sentTokens)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        WebApplication app = builder.Build();
        app.Urls.Add("http://127.0.0.1:0");

        app.MapGet("/api/users", (HttpContext context) =>
        {
            sentTokens.Add(ExtractBearerToken(context));
            return Results.Json(new { isError = false, statusCode = 200 });
        });

        app.MapGet("/api/roles", (HttpContext context) =>
        {
            sentTokens.Add(ExtractBearerToken(context));
            return Results.Json(new { isError = false, statusCode = 200 });
        });

        await app.StartAsync();
        return app;
    }

    private static string? ExtractBearerToken(HttpContext context)
    {
        string? authorization = context.Request.Headers.Authorization.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return null;
        }

        const string bearerPrefix = "Bearer ";
        return authorization.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase)
            ? authorization[bearerPrefix.Length..]
            : authorization;
    }

    private sealed class AuthApi(Func<RefreshTokenRequest, CancellationToken, Task<Response<TokenResponse>>> refresh)
        : IAuthApi
    {
        private int _refreshCount;
        public int RefreshCount => _refreshCount;
        public TaskCompletionSource Started { get; } = NewSignal();

        public Task<Response<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _refreshCount);
            Started.TrySetResult();
            return refresh(request, cancellationToken);
        }

        public Task<Response<TokenResponse>> CreateTokenAsync(string? tenantIdentifier, TokenRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<Response<TokenResponse>> GoogleAuthAsync(GoogleAuthRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<Response<TokenResponse>> GetCurrentTokenAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task<Response> SelfRegisterAsync(string? tenantIdentifier, SelfRegisterRequest request, CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task LogoutAsync(CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}

