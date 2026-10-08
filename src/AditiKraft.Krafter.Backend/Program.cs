using AditiKraft.Krafter.Aspire.ServiceDefaults;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend;

public static class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Forwarded headers (proxy support)
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Aspire observability + health checks
        builder.AddServiceDefaults();

        _ = AppUrls.ParseOrigin(builder.Configuration["Urls:ApiBaseUrl"], "Urls:ApiBaseUrl");

        // CORS (split-host needs cross-origin support)
        builder.Services.AddCorsConfiguration(builder.Configuration);

        // All backend services (DB, auth, persistence, jobs, SignalR, swagger, validation, routes)
        builder.AddBackendServices();

        // Build app
        WebApplication app = builder.Build();

        await ApplyDatabaseMigrationsAsync(app.Services).ConfigureAwait(false);

        // Middleware Pipeline
        app.UseForwardedHeaders();
        app.UseResponseCompression();
        app.MapDefaultEndpoints();

        app.UseCorsConfiguration();
        app.UseSwaggerConfiguration();
        app.UseHttpsRedirection();

        app.UseBackendMiddleware();

        // Backend endpoints (VSA routes, SignalR hub, background jobs dashboard)
        app.MapBackendEndpoints();

        await app.RunAsync().ConfigureAwait(false);
    }

    private static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        TenantDbContext tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
        await tenantDbContext.Database.MigrateAsync().ConfigureAwait(false);
        await TenantDbSeedService.SeedRootTenantAsync(tenantDbContext, CancellationToken.None).ConfigureAwait(false);

        BackgroundJobsContext backgroundJobsContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsContext>();
        await backgroundJobsContext.Database.MigrateAsync().ConfigureAwait(false);

        ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.Database.MigrateAsync().ConfigureAwait(false);

        DataSeedService dataSeedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
        Response seedResponse = await dataSeedService.SeedBasicDataAsync(new SeedDataRequest()).ConfigureAwait(false);
        if (seedResponse.IsError)
        {
            throw new InvalidOperationException(seedResponse.Message ?? "Failed to seed basic data.");
        }
    }
}
