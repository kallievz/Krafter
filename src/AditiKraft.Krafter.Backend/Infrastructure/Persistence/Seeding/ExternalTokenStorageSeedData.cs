using AditiKraft.Krafter.Backend.Features.Integrations.Common;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

internal static class ExternalTokenStorageSeedData
{
    public static IEnumerable<ExternalTokenStorage> GetSeeds()
    {
        return
        [
            new ExternalTokenStorage
            {
                Id = "7f7ec13f-6079-47d3-a8b9-3768e5f2b6d1",
                Key = ExternalTokenStorageKeys.MunsoftToken,
                Value = string.Empty,
                ExpiresOnUtc = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        ];
    }
}
