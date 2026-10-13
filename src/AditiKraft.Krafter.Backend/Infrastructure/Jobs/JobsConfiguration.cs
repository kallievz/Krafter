using TickerQ.Dashboard.DependencyInjection;
using TickerQ.DependencyInjection;
using TickerQ.EntityFrameworkCore.Customizer;
using TickerQ.EntityFrameworkCore.DependencyInjection;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public static class JobsConfiguration
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddScoped<IJobService, JobService>();
        services.AddSingleton<ILocalizationTranslationJobTracker, LocalizationTranslationJobTracker>();

        services.AddTickerQ(options =>
        {
            options.AddOperationalStore(store => store.UseApplicationDbContext<BackgroundJobsContext>(ConfigurationType.IgnoreModelCustomizer));

            ServiceExtensions.AddDashboard(options, dashboard => dashboard.SetBasePath("tickerq"));
        });

        return services;
    }

    public static WebApplication UseBackgroundJobs(this WebApplication app)
    {
        app.UseTickerQ();
        return app;
    }
}
