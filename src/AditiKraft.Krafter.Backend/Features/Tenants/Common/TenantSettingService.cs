using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Tenants.Common;

public sealed class TenantSettingService(
    TenantDbContext tenantDbContext,
    ITenantGetterService tenantGetterService) : ITenantSettingService, IScopedService
{
    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Setting key is required.", nameof(key));
        }

        string tenantId = tenantGetterService.Tenant.Id ?? SeedDataConstants.RootTenant.Id;

        string? tenantValue = await tenantDbContext.TenantSettings.AsNoTracking()
            .Where(setting => setting.TenantId == tenantId && setting.Key == key)
            .Select(setting => setting.Value)
            .FirstOrDefaultAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(tenantValue))
        {
            return tenantValue;
        }

        if (tenantId == SeedDataConstants.RootTenant.Id)
        {
            return null;
        }

        return await tenantDbContext.TenantSettings.AsNoTracking()
            .Where(setting => setting.TenantId == SeedDataConstants.RootTenant.Id && setting.Key == key)
            .Select(setting => setting.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> GetIntValueOrDefaultAsync(string key, int fallbackValue, CancellationToken cancellationToken)
    {
        string? configuredValue = await GetValueAsync(key, cancellationToken);
        return int.TryParse(configuredValue, out int parsedValue) ? parsedValue : fallbackValue;
    }

    public async Task<string> GetStringValueOrDefaultAsync(string key, string fallbackValue, CancellationToken cancellationToken)
    {
        string? configuredValue = await GetValueAsync(key, cancellationToken);
        return string.IsNullOrWhiteSpace(configuredValue) ? fallbackValue : configuredValue;
    }
}
