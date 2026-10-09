using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AditiKraft.Krafter.Tests.Persistence;

public sealed class PersistenceTests
{
    [Fact]
    public void SqlServerTenantMigrationGeneratesUpgradeAndDowngradeScripts()
    {
        DbContextOptions<TenantDbContext> options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseSqlServer("Data Source=KALLIENTB;Initial Catalog=RumasApp;Persist Security Info=True;User ID=kallievz;Password=No1enters@;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"Application\";Command Timeout=0")
            .Options;
        using var db = new TenantDbContext(options);

        Assert.False(db.Database.HasPendingModelChanges());
        IMigrator migrator = db.GetService<IMigrator>();
        string latestMigration = db.Database.GetMigrations().Last();

        string upgrade = migrator.GenerateScript(fromMigration: "0", toMigration: latestMigration);
        Assert.Contains("__EFMigrationsHistory", upgrade, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrWhiteSpace(upgrade));

        string downgrade = migrator.GenerateScript(fromMigration: latestMigration, toMigration: "0");
        Assert.Contains("__EFMigrationsHistory", downgrade, StringComparison.OrdinalIgnoreCase);
        Assert.False(string.IsNullOrWhiteSpace(downgrade));
    }

    [Fact]
    public void PostgreSqlModelMatchesExistingMigrations()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer("Data Source=KALLIENTB;Initial Catalog=RumasApp;Persist Security Info=True;User ID=kallievz;Password=No1enters@;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"Application\";Command Timeout=0")
            .Options;
        using var db = new ApplicationDbContext(options, new CurrentUser(), new CurrentTenantService());

        // Model comparison does not connect to a database.
        Assert.False(db.Database.HasPendingModelChanges());
    }

    [Fact]
    public async Task HistoryRequestReturnsBadRequestWithoutQueryingDatabaseAsync()
    {
        await using var db = new TenantDbContext(new DbContextOptions<TenantDbContext>());

        Response<PaginationResponse<TenantDto>> result = await new TenantsCrud.Handler(db, null!, null!, null!, null!, null!, null!).GetAsync(new GetRequestInput { History = true },
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("history is not supported", result.Message);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TenantListKeepsCurrentAndDeletedRecordsSeparateAsync(bool deleted)
    {
        DbContextOptions<TenantDbContext> options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString()).Options;
        await using var db = new TenantDbContext(options);
        db.Tenants.AddRange(
            new Tenant { Id = "current", Name = "Current", Identifier = "current", AdminEmail = "a@example.com" },
            new Tenant { Id = "deleted", Name = "Deleted", Identifier = "deleted", AdminEmail = "b@example.com", IsDeleted = true });
        await db.SaveChangesAsync();

        Response<PaginationResponse<TenantDto>> result = await new TenantsCrud.Handler(db, null!, null!, null!, null!, null!, null!).GetAsync(
            new GetRequestInput { IsDeleted = deleted, MaxResultCount = 10 }, CancellationToken.None);

        Assert.False(result.IsError);
        Assert.Equal(deleted ? "deleted" : "current", Assert.Single(result.Data!.Items).Id);
        Assert.Equal(1, result.Data.TotalCount);
    }
}

