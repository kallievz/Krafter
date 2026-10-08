using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

public static class ApplicationDbSeedService
{
    public static async Task SeedExternalTokensAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        List<ExternalTokenStorage> existingTokens = await dbContext.ExternalTokenStorages
            .AsNoTracking()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        IEnumerable<ExternalTokenStorage> missingTokens = ExternalTokenStorageSeedData.GetSeeds()
            .Where(seedToken => existingTokens.All(existingToken => existingToken.Key != seedToken.Key && existingToken.Id != seedToken.Id));

        List<ExternalTokenStorage> tokensToInsert = missingTokens.ToList();
        if (tokensToInsert.Count > 0)
        {
            await dbContext.ExternalTokenStorages.AddRangeAsync(tokensToInsert, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
