using AditiKraft.Krafter.UI.Web.Client;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Http;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Refit;
using AditiKraft.Krafter.UI.Web.Infrastructure.Auth;
using AditiKraft.Krafter.UI.Web.Infrastructure.Services;
using Microsoft.AspNetCore.Components.Authorization;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Services;

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

        string? appDbConnectionString = configuration.GetConnectionString("appDb");
        if (string.IsNullOrWhiteSpace(appDbConnectionString))
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddDistributedPostgresCache(options =>
            {
                options.ConnectionString = appDbConnectionString;
                options.SchemaName = "public";
                options.TableName = "cache";
                options.CreateIfNotExists = true;
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
