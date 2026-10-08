using Microsoft.EntityFrameworkCore;
using TickerQ.EntityFrameworkCore.DbContextFactory;
using TickerQ.Utilities.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public class BackgroundJobsContext(DbContextOptions<BackgroundJobsContext> options) : TickerQDbContext<TimeTickerEntity, CronTickerEntity>(options)
{
}
