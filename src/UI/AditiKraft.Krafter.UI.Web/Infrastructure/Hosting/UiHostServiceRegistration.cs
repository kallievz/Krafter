using AditiKraft.Krafter.UI.Web.Client;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Http;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Refit;
using AditiKraft.Krafter.UI.Web.Infrastructure.Auth;
using AditiKraft.Krafter.UI.Web.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace AditiKraft.Krafter.UI.Web.Infrastructure.Hosting;

public static class UiHostServiceRegistration
{
    public static IServiceCollection AddUiHostServices(this IServiceCollection services, IConfiguration configuration,
        BlazorHostingMode hostingMode = BlazorHostingMode.SplitHost)
    {
        services.AddSingleton(UiUrlConfiguration.Resolve(configuration, hostingMode));
        services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
        services.AddHttpContextAccessor();

        string? sqlCacheConnectionString = configuration.GetConnectionString("cacheDb");
        if (string.IsNullOrWhiteSpace(sqlCacheConnectionString))
        {
            sqlCacheConnectionString = configuration.GetConnectionString("appDb");
        }

        if (string.IsNullOrWhiteSpace(sqlCacheConnectionString))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = sqlCacheConnectionString;
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });
        }

        services.AddHybridCache();
        services.AddMemoryCache();

        services.AddSingleton<IFormFactor, FormFactorServer>();
        services.AddSingleton<IRequestContextAccessor, ServerRequestContextAccessor>();
        services.AddScoped<IAuthApiService, ServerAuthApiService>();
        services.AddSingleton<IAuthStorageService, AuthStorageServiceServer>();
        services.AddCascadingAuthenticationState();
        services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>()
            .AddAuthorizationCore(PermissionRegistration.RegisterPermissionClaims);

        services.AddUIServices();
        services.AddRadzenComponents();
        services.AddScoped<TenantIdentifier>();
        services.AddApiRefitClients();
        return services;
    }
}
