using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Tests.Persistence;

[CollectionDefinition("Design-time configuration", DisableParallelization = true)]
public sealed class DesignTimeConfigurationCollection { }

[Collection("Design-time configuration")]
public sealed class DesignTimeConfigurationTests
{
    [Fact]
    public void EnvironmentConnectionWorksWithoutBackendSettingsFile()
    {
        const string key = "ConnectionStrings__AppDbMigration";
        const string connection = "Host=localhost;Database=design_time;Username=test;Password=unused";
        string? previousValue = Environment.GetEnvironmentVariable(key);
        string previousDirectory = Directory.GetCurrentDirectory();
        string directory = Directory.CreateTempSubdirectory("krafter-design-time-").FullName;
        try
        {
            Directory.SetCurrentDirectory(directory);
            Environment.SetEnvironmentVariable(key, connection);

            using ApplicationDbContext db = new DesignTimeApplicationDbContextFactory().CreateDbContext([]);
            using TenantDbContext tenants = new DesignTimeDbContextFactory().CreateDbContext([]);
            using BackgroundJobsContext jobs = new DesignTimeBackgroundJobsContextDbContextFactory().CreateDbContext([]);

            Assert.Equal(connection, db.Database.GetConnectionString());
            Assert.Equal(connection, tenants.Database.GetConnectionString());
            Assert.Equal(connection, jobs.Database.GetConnectionString());
        }
        finally
        {
            Directory.SetCurrentDirectory(previousDirectory);
            Environment.SetEnvironmentVariable(key, previousValue);
            Directory.Delete(directory);
        }
    }
}
