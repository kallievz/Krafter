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

        // Load local overrides when present (appsettings.Local.json) so local connection strings
        // and secrets are available during development and when running from solution root.
        builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

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

        var logger = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<Program>>();

        try
        {
            logger.LogInformation("Starting database migrations.");

            // Tenant DB - root tenant registry and required baseline
            var tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            logger.LogInformation("Migrating TenantDb...");
            await tenantDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("TenantDb migration complete.");

            // Seed root tenant immediately after tenant migrations
            logger.LogInformation("Seeding root tenant...");
            await TenantDbSeedService.SeedRootTenantAsync(tenantDbContext, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation("Root tenant seeding complete.");

            // Background jobs (TickerQ) operational store
            var backgroundJobsContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsContext>();
            logger.LogInformation("Migrating BackgroundJobs (TickerQ) DB...");
            await backgroundJobsContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("BackgroundJobs migration complete.");

            // Audit DB - must be present before application writes via audit interceptor
            var auditDbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
            logger.LogInformation("Migrating AuditDb...");
            await auditDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("AuditDb migration complete.");

            // Application DB - main application schema and seed data
            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation("Migrating ApplicationDb...");
            await applicationDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("ApplicationDb migration complete.");

            // Run application-level seeding
            logger.LogInformation("Starting application data seed...");
            var dataSeedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
            Response seedResponse = await dataSeedService.SeedBasicDataAsync(new SeedDataRequest()).ConfigureAwait(false);
            if (seedResponse.IsError)
            {
                logger.LogError("Application data seed failed: {Message}", seedResponse.Message);
                throw new InvalidOperationException(seedResponse.Message ?? "Failed to seed basic data.");
            }
            logger.LogInformation("Application data seed complete.");

            logger.LogInformation("All database migrations and seed operations completed successfully.");
        }
        catch (Exception ex)
        {
            // Re-throw after logging to stop the host startup so failures are visible.
            var message = "Database migration or seeding failed and prevented application startup.";
            var logger2 = scope.ServiceProvider.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();
            logger2?.LogCritical(ex, message);
            throw;
        }
    }
}
