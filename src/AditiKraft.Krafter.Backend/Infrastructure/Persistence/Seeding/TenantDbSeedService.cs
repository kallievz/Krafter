using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

public static class TenantDbSeedService
{
    public static async Task SeedRootTenantAsync(TenantDbContext dbContext, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        int rootTenantCount = await dbContext.Tenants
            .AsNoTracking()
            .CountAsync(tenant => tenant.Id == SeedDataConstants.RootTenant.Id, cancellationToken).ConfigureAwait(false);

        bool hasRootTenant = rootTenantCount > 0;
        if (!hasRootTenant)
        {
            Tenant rootTenant = TenantSeedData.GetSeeds().Single();
            await dbContext.Tenants.AddAsync(rootTenant, cancellationToken).ConfigureAwait(false);
        }

        List<TenantSetting> existingRootSettings = await dbContext.TenantSettings
            .AsNoTracking()
            .Where(setting => setting.TenantId == SeedDataConstants.RootTenant.Id)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        IEnumerable<TenantSetting> missingSettings = TenantSettingSeedData.GetSeeds()
            .Where(seedSetting => existingRootSettings.All(existing => existing.Key != seedSetting.Key));

        List<TenantSetting> settingsToInsert = missingSettings.ToList();
        if (settingsToInsert.Count > 0)
        {
            await dbContext.TenantSettings.AddRangeAsync(settingsToInsert, cancellationToken).ConfigureAwait(false);
        }

        if (!hasRootTenant || settingsToInsert.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
