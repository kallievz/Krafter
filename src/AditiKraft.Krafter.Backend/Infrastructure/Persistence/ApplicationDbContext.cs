using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Auth.Common;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36);
            entity
                .HasIndex(r => new { r.NormalizedName, r.TenantId })
                .IsUnique();

            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);

            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ApplicationRoleClaim>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36);
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36);

            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);

            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.PasswordHistories)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.NormalizedEmail).IsUnique(false);
            entity.HasIndex(e => new { e.NormalizedEmail, e.TenantId }).IsUnique();

            entity.HasIndex(e => e.NormalizedUserName).IsUnique(false);
            entity.HasIndex(e => new { e.NormalizedUserName, e.TenantId }).IsUnique();
        });

        modelBuilder.Entity<ApplicationUserClaim>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36);
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);
        });

        modelBuilder.Entity<ApplicationUserRole>(entity =>
        {
            // Configure the relationship with Role
            entity.HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the relationship with User
            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(c => c.CreatedById).HasMaxLength(36);
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);
        });

        modelBuilder.Entity<PasswordHistory>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
            entity.Property(c => c.UserId).HasMaxLength(36);
            entity.Property(c => c.PasswordHash).IsRequired();
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);

            entity.HasIndex(c => new { c.UserId, c.CreatedOn });
        });

        modelBuilder.Entity<EmailLog>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
            entity.Property(c => c.ToEmail).HasMaxLength(256);
            entity.Property(c => c.Subject).HasMaxLength(500);
            entity.Property(c => c.Provider).HasMaxLength(50);
            entity.Property(c => c.Status).HasMaxLength(50);
            entity.Property(c => c.TemplateKey).HasMaxLength(128);
            entity.Property(c => c.EmailTemplateId).HasMaxLength(36);
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);

            entity.HasIndex(c => new { c.TenantId, c.Status, c.CreatedOn });

            entity.HasOne(c => c.EmailTemplate)
                .WithMany(c => c.EmailLogs)
                .HasForeignKey(c => c.EmailTemplateId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);
            entity.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
            entity.Property(c => c.Key).HasMaxLength(128).IsRequired();
            entity.Property(c => c.Name).HasMaxLength(200).IsRequired();
            entity.Property(c => c.SubjectTemplate).HasMaxLength(1000).IsRequired();
            entity.Property(c => c.BodyTemplate).IsRequired();
            entity.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == tenantGetterService.Tenant.Id);

            entity.HasIndex(c => new { c.TenantId, c.Key }).IsUnique();
        });

        modelBuilder.Entity<ApplicationUserLogin>(entity =>
        {
            // Configure primary keys for custom user login and token classes
            entity
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
        });

        modelBuilder.Entity<ApplicationUserToken>(entity =>
        {
            entity
                .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
        });

        modelBuilder.ApplyCommonConfigureAcrossEntity();

        modelBuilder.Entity<UserRefreshToken>(entity => { entity.HasKey(c => c.UserId); });
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
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["IsDeleted"] = true;
                    SetTenantAndHistoryInfo(entry);
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
