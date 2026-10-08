using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static class TenantSettingSeedData
{
    public static IEnumerable<TenantSetting> GetSeeds()
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
                Value = "dummy" // "smtp", "sendgrid"
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
