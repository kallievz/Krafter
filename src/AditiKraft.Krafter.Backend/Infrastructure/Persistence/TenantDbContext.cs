using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public class TenantDbContext(DbContextOptions<TenantDbContext> options)
    : DbContext(options)
{
    // The expression index is created by UniqueTenantIdentifiers; EF indexes only model properties.
    public const string TenantIdentifierIndexName = "IX_Tenant_Identifier_Lower";

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantSetting> TenantSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasQueryFilter(c => c.IsDeleted == false);

            entity.ToTable(nameof(Tenant));

            entity.HasData(new List<Tenant>
            {
                new()
                {
                    Id = SeedDataConstants.RootTenant.Id,
                    Identifier = SeedDataConstants.RootTenant.Identifier,
                    IsActive = true,
                    Name = SeedDataConstants.RootTenant.Name,
                    CreatedOn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ValidUpto = DateTime.MaxValue,
                    AdminEmail = SeedDataConstants.RootUser.EmailAddress
                }
            });
        });

        modelBuilder.Entity<TenantSetting>(entity =>
        {
            entity.ToTable(nameof(TenantSetting));
            entity.Property(setting => setting.Id).HasMaxLength(36);
            entity.Property(setting => setting.TenantId).HasMaxLength(36);
            entity.Property(setting => setting.Key).HasMaxLength(256);
            entity.Property(setting => setting.Value).HasMaxLength(4096);

            entity.HasIndex(setting => new { setting.TenantId, setting.Key }).IsUnique();

            entity.HasOne(setting => setting.Tenant)
                .WithMany(tenant => tenant.Settings)
                .HasForeignKey(setting => setting.TenantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasData(GetRootTenantSettings());
        });
    }

    private static List<TenantSetting> GetRootTenantSettings()
    {
        return
        [
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101001",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordReusePreventionWindowInDays,
                Value = "90"
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101002",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordExpiryInDays,
                Value = "90"
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101003",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordExpiryNotificationCooldownInDays,
                Value = "1"
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101004",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsProvider,
                Value = "smtp"
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101005",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpHost,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101006",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpPort,
                Value = "587"
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101007",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpUserName,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101008",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpPassword,
                Value = string.Empty,
                IsSensitive = true
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101009",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpSenderEmail,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101010",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpSenderName,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101011",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSmtpReplyToEmail,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101012",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSendGridApiKey,
                Value = string.Empty,
                IsSensitive = true
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101013",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSendGridSenderEmail,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101014",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSendGridSenderName,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = "c8a78156-8e6c-4f55-8841-7de7ea101015",
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.NotificationsSendGridReplyToEmail,
                Value = string.Empty
            }
        ];
    }
}
