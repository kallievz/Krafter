namespace AditiKraft.Krafter.Backend.Features.Tenants.Common;

public interface ITenantSettingService
{
    public Task<string?> GetValueAsync(string key, CancellationToken cancellationToken);

    public Task<int> GetIntValueOrDefaultAsync(string key, int fallbackValue, CancellationToken cancellationToken);

    public Task<string> GetStringValueOrDefaultAsync(string key, string fallbackValue, CancellationToken cancellationToken);
}
