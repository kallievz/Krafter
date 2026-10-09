using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;

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
                sp.GetRequiredService<ILogger<AuditSaveChangesInterceptor>>()));

        // TenantDbContext (root tenant registry) - attach audit interceptor so tenant administration is audited
        services.AddDbContext<TenantDbContext>((sp, opts) =>
            opts.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>(), sp.GetRequiredService<AuditSaveChangesInterceptor>()));

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

        // Munsoft integration store (optional separate connection string 'Munsoft')
        string? munsoftConnection = configuration.GetConnectionString("Munsoft") ?? connectionString;
        if (!string.IsNullOrWhiteSpace(munsoftConnection))
        {
            services.AddDbContext<MunsoftDbContext>((sp, opts) =>
                opts.UseSqlServer(munsoftConnection, sqlServer => sqlServer.EnableRetryOnFailure())
                    .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>()));
        }

        // Localization store (uses separate connection string 'Localization' when present, otherwise falls back to appDb)
        string? localizationConnection = configuration.GetConnectionString("Localization") ?? connectionString;
        services.AddDbContext<LocalizationDbContext>((sp, opts) =>
            opts.UseSqlServer(localizationConnection, sqlServer => sqlServer.EnableRetryOnFailure())
                .AddInterceptors(sp.GetRequiredService<EfCommandTimingInterceptor>()));

        return services;
    }
}
