using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.DbContextFactory;
using TickerQ.Utilities.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public class BackgroundJobsContext : TickerQDbContext<TimeTickerEntity, CronTickerEntity>
{
    public BackgroundJobsContext(DbContextOptions<BackgroundJobsContext> options)
        : base(options)
    {
    }
}
