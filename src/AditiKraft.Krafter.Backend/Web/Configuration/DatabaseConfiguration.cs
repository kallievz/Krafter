using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AditiKraft.Krafter.Backend.Web.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("appDb")
                                  ?? throw new InvalidOperationException(
                                      "Database connection string 'appDb' not found");

        services.Configure<EfQueryPerformanceOptions>(configuration.GetSection(EfQueryPerformanceOptions.SectionName));
        services.AddHttpContextAccessor();
        services.AddScoped<EfCommandTimingInterceptor>();
        // Metadata cache used by audit interceptor - singleton for concurrent access
        services.AddSingleton<AuditMetadataCache>();
        services.AddScoped(sp =>
            new AuditSaveChangesInterceptor(
                sp.GetRequiredService<ICurrentUser>(),
                sp.GetRequiredService<ITenantGetterService>(),
                sp.GetRequiredService<AuditMetadataCache>(),
                sp,
                sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<AuditSaveChangesInterceptor>>()));

        // TenantDbContext (root tenant registry) - do not attach audit interceptor
        services.AddDbContext<TenantDbContext>((sp, opts) =>
            opts.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>()));

        // BackgroundJobsContext (TickerQ operational store) - do not attach audit interceptor
        services.AddDbContext<BackgroundJobsContext>((sp, opts) =>
            opts.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>())
                .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));

        // ApplicationDbContext (main multi-tenant context) - attach audit interceptor here only
        services.AddDbContext<ApplicationDbContext>((sp, opts) =>
            opts.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>(), sp.GetRequiredService<AuditSaveChangesInterceptor>()));

        // AuditDbContext (separate audit store)
        services.AddDbContext<AuditDbContext>((sp, opts) =>
            opts.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>()));

        return services;
    }
}
