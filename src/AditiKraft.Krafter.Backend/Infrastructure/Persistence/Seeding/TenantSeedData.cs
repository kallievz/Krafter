using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static class TenantSeedData
{
    public static IEnumerable<Tenant> GetSeeds()
    {
        return
        [
            new Tenant
            {
                Id = SeedDataConstants.RootTenant.Id,
                Identifier = SeedDataConstants.RootTenant.Identifier,
                IsActive = true,
                Name = SeedDataConstants.RootTenant.Name,
                CreatedOn = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ValidUpto = DateTime.MaxValue,
                AdminEmail = SeedDataConstants.RootUser.EmailAddress
            }
        ];
    }
}
