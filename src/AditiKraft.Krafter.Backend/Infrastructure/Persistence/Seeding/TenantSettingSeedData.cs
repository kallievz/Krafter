using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
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
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordReusePreventionWindowInDays,
                Value = "90"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordExpiryInDays,
                Value = "90"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.SecurityPasswordExpiryNotificationCooldownInDays,
                Value = "1"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailProvider,
                Value = "dummy" // "smtp", "sendgrid"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSmtpHost,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSmtpPort,
                Value = "587"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSmtpUserName,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSmtpPassword,
                Value = string.Empty,
                IsSensitive = true
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSenderEmail,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSenderName,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailReplyToEmail,
                Value = string.Empty
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.EmailSendGridApiKey,
                Value = string.Empty,
                IsSensitive = true
            },

            #region Munsoft API settings
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftBaseAddess,
                Value = "https://api.munsoft.co.za/"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftUserName,
                Value = "service_account@rumas.co.za"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftPassword,
                Value = "OByE=~Ft)5ZJ",
                IsSensitive = true
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftLogin,
                Value = "/login"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftInstances,
                Value = "/instances?page=1&paginate=0"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftInstanceId,
                Value = "0"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftDebtorParquet,
                Value = "/instances/{InstanceId}/consumer_debtors?page=1&paginate=0&export=parquet"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftDebtorSingle,
                Value = "/instances/{InstanceId}/consumer_debtors/{AccountNumber}?cached=false"
            }
            ,
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftPropertyParquet,
                Value = "/instances/{InstanceId}/properties?page=1&paginate=0&export=parquet"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftPropertySingle,
                Value = "/instances/{InstanceId}/properties/{AccountNumber}?cached=false"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftMeterParquet,
                Value = "/instances/{InstanceId}/meters?page=1&paginate=0&export=parquet"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftMeterSingle,
                Value = "/instances/{InstanceId}/meters/{AccountNumber}?cached=false"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftAgeingParquet,
                Value = "/instances/{InstanceId}/ageing?page=1&paginate=0&export=parquet"
            },
            new TenantSetting
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = SeedDataConstants.RootTenant.Id,
                Key = TenantSettingKeys.MunsoftAgeingSingle,
                Value = "/instances/{InstanceId}/ageing/{AccountNumber}?cached=false"
            }
            #endregion Munsoft API settings

        ];
    }
}
