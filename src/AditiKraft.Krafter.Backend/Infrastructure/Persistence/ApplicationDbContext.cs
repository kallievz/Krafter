using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Auth.Common;
using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;
using AditiKraft.Krafter.Contracts.Common.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    ICurrentUser currentUser,
    ITenantGetterService tenantGetterService)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string, ApplicationUserClaim, ApplicationUserRole,
        ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>(options)
{
#pragma warning disable CS0108, CS0114
    public virtual DbSet<ApplicationUser> Users { get; set; }
#pragma warning restore CS0108, CS0114
    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
    public virtual DbSet<PasswordHistory> PasswordHistories { get; set; }
    public virtual DbSet<EmailLog> EmailLogs { get; set; }
    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
    public virtual DbSet<ExternalTokenStorage> ExternalTokenStorages { get; set; }
    public virtual DbSet<MunsoftInstance> MunsoftInstances { get; set; }
    // AuditOutbox moved to AuditDbContext. Kept processing in AuditOutboxProcessor which uses both contexts.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new ApplicationRoleClaimConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new ApplicationUserClaimConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new ApplicationUserRoleConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new PasswordHistoryConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new EmailLogConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new EmailTemplateConfiguration(tenantGetterService));
        modelBuilder.ApplyConfiguration(new ExternalTokenStorageConfiguration());
        modelBuilder.ApplyConfiguration(new MunsoftInstanceConfiguration());

        modelBuilder.ApplyConfiguration(new ApplicationUserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationUserTokenConfiguration());
        modelBuilder.ApplyConfiguration(new UserRefreshTokenConfiguration());

        modelBuilder.ApplyCommonConfigureAcrossEntity();
    }

    public override int SaveChanges()
    {
        UpdateSoftDeleteStatusesAndSetTenant();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
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
