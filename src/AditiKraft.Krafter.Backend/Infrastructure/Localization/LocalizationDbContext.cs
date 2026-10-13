using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Localization.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Localization;
using AditiKraft.Krafter.Contracts.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AditiKraft.Krafter.Backend.Infrastructure.Localization;

public class LocalizationDbContext(
    DbContextOptions<LocalizationDbContext> options,
    ICurrentUser _currentUser,
    ITenantGetterService _tenantGetterService) : DbContext(options)
{

    public DbSet<Language> Languages { get; set; } = null!;
    public DbSet<Resource> Resources { get; set; } = null!;
    public DbSet<ResourceTranslation> ResourceTranslations { get; set; } = null!;
    public DbSet<GridState> GridStates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new LanguageConfiguration());
        modelBuilder.ApplyConfiguration(new ResourceConfiguration());
        modelBuilder.ApplyConfiguration(new ResourceTranslationConfiguration());
        modelBuilder.ApplyConfiguration(new GridStateConfiguration());
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatusesAndSetTenant();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatusesAndSetTenant();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateSoftDeleteStatusesAndSetTenant();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void UpdateSoftDeleteStatusesAndSetTenant()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    SetTenantAndHistoryInfo(entry);
                    break;
                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete)
                    {
                        entry.State = EntityState.Modified;
                        entry.CurrentValues["IsDeleted"] = true;
                        SetTenantAndHistoryInfo(entry);
                    }

                    break;
            }
        }
    }

    private void SetTenantAndHistoryInfo(EntityEntry entry)
    {
        if (entry.Entity is IHistory)
        {
            entry.CurrentValues["CreatedOn"] = DateTime.UtcNow;
            entry.Property("CreatedOn").IsModified = true;

            if (!string.IsNullOrWhiteSpace(_currentUser.GetUserId()))
            {
                entry.CurrentValues["CreatedById"] = _currentUser.GetUserId();
                entry.Property("CreatedById").IsModified = true;
            }
        }

        if (entry.Entity is ITenant)
        {
            CurrentTenantDetails tenant = _tenantGetterService.Tenant;
            entry.CurrentValues["TenantId"] = tenant.Id;
        }
    }
}
