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
using Serilog;
using Serilog.Events;
using System.Reflection;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;

namespace AditiKraft.Krafter.Backend;

public static class Program
{
    private static async Task Main(string[] args)
    {
        // Configure Serilog early so all startup logs are captured
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        IConfiguration configuration = configurationBuilder.Build();
        Assembly[] loggerAssemblies = [typeof(Program).Assembly, typeof(HostingExtensions).Assembly, typeof(TenantDbContext).Assembly];

        await LoggerRegistration.EnsureSerilogLogTableAsync(configuration).ConfigureAwait(false);
        Log.Logger = LoggerRegistration.CreateLogger(configuration, loggerAssemblies);

        try
        {
            Log.Information("Starting host");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();
            builder.Configuration.AddConfiguration(configuration);

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

            // Ensure Serilog request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.GetLevel = static (httpContext, elapsed, exception) =>
                {
                    if (exception is not null || httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError)
                    {
                        return LogEventLevel.Error;
                    }

                    if (httpContext.Response.StatusCode >= StatusCodes.Status400BadRequest)
                    {
                        return LogEventLevel.Warning;
                    }

                    if (httpContext.Request.Path.StartsWithSegments("/metrics") ||
                        httpContext.Request.Path.StartsWithSegments("/health") ||
                        httpContext.Request.Path.StartsWithSegments("/alive"))
                    {
                        return LogEventLevel.Debug;
                    }

                    return LogEventLevel.Information;
                };
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("Host", httpContext.Request.Host.Value);
                    diagnosticContext.Set("Scheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                };
            });

            app.UseCorsConfiguration();
            app.UseSwaggerConfiguration();
            app.UseHttpsRedirection();

            app.UseBackendMiddleware();

            // Backend endpoints (VSA routes, SignalR hub, background jobs dashboard)
            app.MapBackendEndpoints();

            await app.RunAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Serilog fatal for unhandled host exceptions
            Log.Fatal(ex, "Host terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task ApplyDatabaseMigrationsAsync(IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();

        ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("Program");

        try
        {
            logger.LogInformation("Starting database migrations.");

            // Tenant DB - root tenant registry and required baseline
            TenantDbContext tenantDbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            logger.LogInformation("Migrating TenantDb...");
            await tenantDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("TenantDb migration complete.");

            // Seed root tenant immediately after tenant migrations
            logger.LogInformation("Seeding root tenant...");
            await TenantDbSeedService.SeedRootTenantAsync(tenantDbContext, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation("Root tenant seeding complete.");

            // Background jobs (TickerQ) operational store
            BackgroundJobsContext backgroundJobsContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsContext>();
            logger.LogInformation("Migrating BackgroundJobs (TickerQ) DB...");
            await backgroundJobsContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("BackgroundJobs migration complete.");

            // Munsoft integration DB (optional)
            IConfiguration? scopedConfig = scope.ServiceProvider.GetService<IConfiguration>();
            string? munsoftConn = scopedConfig?.GetConnectionString("Munsoft");
            if (!string.IsNullOrWhiteSpace(munsoftConn))
            {
                MunsoftDbContext munsoftDbContext = scope.ServiceProvider.GetRequiredService<MunsoftDbContext>();
                logger.LogInformation("Migrating MunsoftDb...");
                await munsoftDbContext.Database.MigrateAsync().ConfigureAwait(false);
                logger.LogInformation("MunsoftDb migration complete.");
            }

            // Localization DB (uses separate 'Localization' connection when configured, otherwise appDb)
            LocalizationDbContext localizationDbContext = scope.ServiceProvider.GetRequiredService<LocalizationDbContext>();
            logger.LogInformation("Migrating LocalizationDb...");
            await localizationDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("LocalizationDb migration complete.");

            logger.LogInformation("Seeding localization languages...");
            await LocalizationDbSeedService.SeedLanguagesAsync(localizationDbContext, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation("Localization language seeding complete.");

            logger.LogInformation("Seeding localization resources...");
            await LocalizationDbSeedService.SeedResourcesAsync(localizationDbContext, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation("Localization resource seeding complete.");

            // Audit DB - must be present before application writes via audit interceptor
            AuditDbContext auditDbContext = scope.ServiceProvider.GetRequiredService<AuditDbContext>();
            logger.LogInformation("Migrating AuditDb...");
            await auditDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("AuditDb migration complete.");

            // Application DB - main application schema and seed data
            ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            logger.LogInformation("Migrating ApplicationDb...");
            await applicationDbContext.Database.MigrateAsync().ConfigureAwait(false);
            logger.LogInformation("ApplicationDb migration complete.");

            logger.LogInformation("Seeding external integration tokens...");
            await ApplicationDbSeedService.SeedExternalTokensAsync(applicationDbContext, CancellationToken.None).ConfigureAwait(false);
            logger.LogInformation("External integration token seeding complete.");

            // Run application-level seeding
            logger.LogInformation("Starting application data seed...");
            DataSeedService dataSeedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
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
            string message = "Database migration or seeding failed and prevented application startup.";
            ILoggerFactory? loggerFactory2 = scope.ServiceProvider.GetService<ILoggerFactory>();
            Microsoft.Extensions.Logging.ILogger? logger2 = loggerFactory2?.CreateLogger("Program");
            logger2?.LogCritical(ex, message);
            Log.Fatal(ex, "{Message}", message);
            throw;
        }
    }
}

