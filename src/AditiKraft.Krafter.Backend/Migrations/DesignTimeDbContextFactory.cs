using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AditiKraft.Krafter.Backend.Migrations;

internal static class DesignTimeConnectionStringHelper
{
    public static string GetConnectionString()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
        string? connectionString = configuration.GetConnectionString("AppDbMigration");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'AppDbMigration' not found. Set ConnectionStrings__AppDbMigration or add it to appsettings.Local.json.");
        }

        return connectionString;
    }
}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
{
    public TenantDbContext CreateDbContext(string[] args)
    {
        string connectionString = DesignTimeConnectionStringHelper.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure());

        return new TenantDbContext(optionsBuilder.Options);
    }
}

public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string connectionString = DesignTimeConnectionStringHelper.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure());

        return new ApplicationDbContext(optionsBuilder.Options, null!, null!);
    }
}

public class DesignTimeBackgroundJobsContextDbContextFactory : IDesignTimeDbContextFactory<BackgroundJobsContext>
{
    public BackgroundJobsContext CreateDbContext(string[] args)
    {
        string connectionString = DesignTimeConnectionStringHelper.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<BackgroundJobsContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure());

        return new BackgroundJobsContext(optionsBuilder.Options);
    }
}

public class DesignTimeAuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        string connectionString = DesignTimeConnectionStringHelper.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure());

        return new AuditDbContext(optionsBuilder.Options);
    }
}

public class DesignTimeLocalizationDbContextFactory : IDesignTimeDbContextFactory<LocalizationDbContext>
{
    public LocalizationDbContext CreateDbContext(string[] args)
    {
        string connectionString = DesignTimeConnectionStringHelper.GetConnectionString();
        var optionsBuilder = new DbContextOptionsBuilder<LocalizationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlServer => sqlServer.EnableRetryOnFailure());

        return new LocalizationDbContext(optionsBuilder.Options, null!, null!);
    }
}
