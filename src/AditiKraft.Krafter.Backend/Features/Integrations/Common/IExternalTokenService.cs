namespace AditiKraft.Krafter.Backend.Features.Integrations.Common;

public interface IExternalTokenService
{
    public Task<string> GetTokenAsync(string externalClient, CancellationToken cancellationToken);
}
