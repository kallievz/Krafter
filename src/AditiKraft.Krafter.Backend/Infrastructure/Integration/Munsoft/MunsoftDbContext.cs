using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;
using AditiKraft.Krafter.Contracts.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;

public class MunsoftDbContext(
    DbContextOptions<MunsoftDbContext> options,
    ICurrentUser currentUser,
    ITenantGetterService tenantGetterService) : DbContext(options)
{
    public DbSet<MunsoftInstance> MunsoftInstances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new MunsoftInstanceConfiguration());
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatusesAndSetTenant();
        return base.SaveChanges();
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
            if (currentUser is not null && !string.IsNullOrWhiteSpace(currentUser.GetUserId()))
            {
                entry.CurrentValues["CreatedById"] = currentUser.GetUserId();
                entry.Property("CreatedById").IsModified = true;
            }
        }

        if (entry.Entity is ITenant)
        {
            CurrentTenantDetails tenant = tenantGetterService.Tenant;
            entry.CurrentValues["TenantId"] = tenant.Id;
        }
    }
}
