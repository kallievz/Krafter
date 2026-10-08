namespace AditiKraft.Krafter.Backend.Features.Integrations.Common;

public interface IExternalTokenProvider
{
    public string ExternalClient { get; }

    public string StorageKey { get; }

    public TimeSpan TokenIdleTimeout { get; }

    public Task<string> RequestTokenAsync(CancellationToken cancellationToken);
}
