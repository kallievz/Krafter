using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Integrations.Common;

public sealed class ExternalTokenService(
    ApplicationDbContext applicationDbContext,
    IEnumerable<IExternalTokenProvider> externalTokenProviders) : IExternalTokenService, IScopedService
{
    public async Task<string> GetTokenAsync(string externalClient, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(externalClient))
        {
            throw new ArgumentException("External client is required.", nameof(externalClient));
        }

        IExternalTokenProvider tokenProvider = ResolveProvider(externalClient);

        await CleanupExpiredTokensAsync(cancellationToken).ConfigureAwait(false);

        DateTime utcNow = DateTime.UtcNow;
        ExternalTokenStorage? existingToken = await applicationDbContext.ExternalTokenStorages
            .FirstOrDefaultAsync(token => token.Key == tokenProvider.StorageKey, cancellationToken).ConfigureAwait(false);

        if (existingToken is not null && existingToken.IsValidAt(utcNow))
        {
            existingToken.ExpiresOnUtc = utcNow.Add(tokenProvider.TokenIdleTimeout);
            await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return existingToken.Value;
        }

        string newToken = await tokenProvider.RequestTokenAsync(cancellationToken).ConfigureAwait(false);
        if (existingToken is null)
        {
            existingToken = new ExternalTokenStorage
            {
                Id = Guid.CreateVersion7().ToString(),
                Key = tokenProvider.StorageKey
            };

            await applicationDbContext.ExternalTokenStorages.AddAsync(existingToken, cancellationToken).ConfigureAwait(false);
        }

        existingToken.Value = newToken;
        existingToken.ExpiresOnUtc = utcNow.Add(tokenProvider.TokenIdleTimeout);

        await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return newToken;
    }

    private IExternalTokenProvider ResolveProvider(string externalClient)
    {
        List<IExternalTokenProvider> matchingProviders = externalTokenProviders
            .Where(provider => string.Equals(provider.ExternalClient, externalClient, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (matchingProviders.Count == 0)
        {
            throw new InvalidOperationException($"No external token provider configured for client '{externalClient}'.");
        }

        if (matchingProviders.Count > 1)
        {
            throw new InvalidOperationException($"Multiple external token providers are configured for client '{externalClient}'.");
        }

        return matchingProviders[0];
    }

    private async Task CleanupExpiredTokensAsync(CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;
        List<ExternalTokenStorage> expiredTokens = await applicationDbContext.ExternalTokenStorages
            .Where(token => token.ExpiresOnUtc.HasValue && token.ExpiresOnUtc.Value <= utcNow)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (expiredTokens.Count == 0)
        {
            return;
        }

        applicationDbContext.ExternalTokenStorages.RemoveRange(expiredTokens);
        await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
