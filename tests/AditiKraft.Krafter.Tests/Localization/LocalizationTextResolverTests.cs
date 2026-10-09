using System.Security.Claims;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace AditiKraft.Krafter.Tests.Localization;

public sealed class LocalizationTextResolverTests
{
    [Fact]
    public async Task ResolveAsyncWhenTranslationMissingThenAutoCreatesEnglishAndReturnsFormattedFallbackAsync()
    {
        await using TestContext context = CreateContext();

        LocalizeTextRequest request = new()
        {
            Key = "Users.Delete.Failed",
            FallbackValue = "Record {0} could not be deleted",
            Arguments = ["ACME"]
        };

        LocalizedTextDto result = await context.Resolver
            .ResolveAsync(request, "af", CancellationToken.None)
            .ConfigureAwait(true);

        Assert.Equal("Users.Delete.Failed", result.Key);
        Assert.Equal("Record ACME could not be deleted", result.Value);
        Assert.True(result.UsedFallback);
        Assert.True(result.AutoCreatedEnglishResource);
        Assert.True(result.AutoCreatedEnglishTranslation);

        string? englishTranslation = await context.Db.ResourceTranslations.AsNoTracking()
            .Where(translation => translation.Resource != null && translation.Language != null &&
                                  translation.Resource.Key == "Users.Delete.Failed" && translation.Language.Code == "en")
            .Select(translation => translation.Value)
            .FirstOrDefaultAsync()
            .ConfigureAwait(true);

        Assert.Equal("Record {0} could not be deleted", englishTranslation);
    }

    [Fact]
    public async Task ResolveAsyncWhenPreferredAndEnglishExistThenReturnsPreferredTranslationAsync()
    {
        await using TestContext context = CreateContext();

        string resourceId = Guid.CreateVersion7().ToString();
        string englishLanguageId = Guid.CreateVersion7().ToString();
        string afrikaansLanguageId = Guid.CreateVersion7().ToString();

        context.Db.Languages.AddRange(
            new Language { Id = englishLanguageId, Code = "en", Name = "English", IsActive = true },
            new Language { Id = afrikaansLanguageId, Code = "af", Name = "Afrikaans", IsActive = true });

        context.Db.Resources.Add(new Resource
        {
            Id = resourceId,
            Key = "Users.Welcome",
            Description = "Welcome"
        });

        context.Db.ResourceTranslations.AddRange(
            new ResourceTranslation
            {
                Id = Guid.CreateVersion7().ToString(),
                ResourceId = resourceId,
                LanguageId = englishLanguageId,
                Value = "Welcome {0}"
            },
            new ResourceTranslation
            {
                Id = Guid.CreateVersion7().ToString(),
                ResourceId = resourceId,
                LanguageId = afrikaansLanguageId,
                Value = "Welkom {0}"
            });

        await context.Db.SaveChangesAsync().ConfigureAwait(true);

        LocalizedTextDto result = await context.Resolver
            .ResolveAsync(
                new LocalizeTextRequest
                {
                    Key = "Users.Welcome",
                    FallbackValue = "Welcome {0}",
                    Arguments = ["Sam"]
                },
                "af",
                CancellationToken.None)
            .ConfigureAwait(true);

        Assert.Equal("Welkom Sam", result.Value);
        Assert.False(result.UsedFallback);
        Assert.False(result.AutoCreatedEnglishResource);
        Assert.False(result.AutoCreatedEnglishTranslation);
    }

    [Fact]
    public async Task ResolveAsyncWhenPreferredTranslationMissingThenReturnsFallbackAndKeepsExistingEnglishTranslationAsync()
    {
        await using TestContext context = CreateContext();

        string resourceId = Guid.CreateVersion7().ToString();
        string englishLanguageId = Guid.CreateVersion7().ToString();

        context.Db.Languages.Add(new Language { Id = englishLanguageId, Code = "en", Name = "English", IsActive = true });
        context.Db.Resources.Add(new Resource
        {
            Id = resourceId,
            Key = "Users.NotFound",
            Description = "Not found"
        });
        context.Db.ResourceTranslations.Add(new ResourceTranslation
        {
            Id = Guid.CreateVersion7().ToString(),
            ResourceId = resourceId,
            LanguageId = englishLanguageId,
            Value = "English existing text"
        });

        await context.Db.SaveChangesAsync().ConfigureAwait(true);

        LocalizedTextDto result = await context.Resolver
            .ResolveAsync(
                new LocalizeTextRequest
                {
                    Key = "Users.NotFound",
                    FallbackValue = "Fallback text"
                },
                "af",
                CancellationToken.None)
            .ConfigureAwait(true);

        Assert.Equal("Fallback text", result.Value);
        Assert.True(result.UsedFallback);
        Assert.False(result.AutoCreatedEnglishResource);
        Assert.False(result.AutoCreatedEnglishTranslation);

        int englishTranslationCount = await context.Db.ResourceTranslations.AsNoTracking()
            .CountAsync(translation => translation.ResourceId == resourceId && translation.LanguageId == englishLanguageId)
            .ConfigureAwait(true);

        Assert.Equal(1, englishTranslationCount);
    }

    private static TestContext CreateContext()
    {
        ServiceCollection services = new();
        string databaseName = Guid.CreateVersion7().ToString();

        services.AddMemoryCache();
        services.AddSingleton<ICurrentUser, TestCurrentUser>();
        services.AddSingleton<ITenantGetterService, TestTenantGetterService>();
        services.AddDbContext<LocalizationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddScoped<ILocalizationTextResolver, LocalizationTextResolver>();

        ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        AsyncServiceScope scope = provider.CreateAsyncScope();

        LocalizationDbContext db = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
        ILocalizationTextResolver resolver = scope.ServiceProvider.GetRequiredService<ILocalizationTextResolver>();

        return new TestContext(provider, scope, db, resolver);
    }

    private sealed class TestContext(
        ServiceProvider provider,
        AsyncServiceScope scope,
        LocalizationDbContext db,
        ILocalizationTextResolver resolver) : IAsyncDisposable
    {
        public LocalizationDbContext Db { get; } = db;
        public ILocalizationTextResolver Resolver { get; } = resolver;

        public async ValueTask DisposeAsync()
        {
            await scope.DisposeAsync().ConfigureAwait(false);
            await provider.DisposeAsync().ConfigureAwait(false);
        }
    }

    private sealed class TestCurrentUser : ICurrentUser
    {
        public string? Name => "test-user";

        public string GetUserId() => "test-user-id";

        public string? GetUserEmail() => "test@example.com";

        public bool IsAuthenticated() => true;

        public bool IsInRole(string role) => false;

        public IEnumerable<Claim>? GetUserClaims() => [];
    }

    private sealed class TestTenantGetterService : ITenantGetterService
    {
        public CurrentTenantDetails Tenant { get; } = new()
        {
            Id = "tenant-a",
            Name = "Tenant A",
            TenantLink = "https://tenant-a.example.com"
        };
    }
}
