using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
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

        // TenantDbContext (root tenant registry)
        services.AddDbContext<TenantDbContext>((sp, opts) =>
            ConfigureDbContext(opts, connectionString, sp.GetRequiredService<EfCommandTimingInterceptor>()));

        // BackgroundJobsContext (TickerQ operational store)
        services.AddDbContext<BackgroundJobsContext>((sp, opts) =>
            ConfigureBackgroundJobsDbContext(opts, connectionString, sp.GetRequiredService<EfCommandTimingInterceptor>()));

        // ApplicationDbContext (main multi-tenant context)
        services.AddDbContext<ApplicationDbContext>((sp, opts) =>
            ConfigureDbContext(opts, connectionString, sp.GetRequiredService<EfCommandTimingInterceptor>()));

        return services;
    }

    private static void ConfigureDbContext(DbContextOptionsBuilder opts, string connectionString,
        EfCommandTimingInterceptor commandTimingInterceptor)
    {
        opts.UseNpgsql(connectionString)
            .AddInterceptors(commandTimingInterceptor);
    }

    private static void ConfigureBackgroundJobsDbContext(DbContextOptionsBuilder opts, string connectionString,
        EfCommandTimingInterceptor commandTimingInterceptor)
    {
        opts.UseNpgsql(connectionString)
            .AddInterceptors(commandTimingInterceptor)
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}
