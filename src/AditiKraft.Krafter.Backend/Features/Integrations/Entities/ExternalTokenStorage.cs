namespace AditiKraft.Krafter.Backend.Features.Integrations.Common;

public sealed class ExternalTokenStorage
{
    public string Id { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public DateTime? ExpiresOnUtc { get; set; }

    public bool IsValidAt(DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(Value))
        {
            return false;
        }

        return !ExpiresOnUtc.HasValue || ExpiresOnUtc.Value > utcNow;
    }

    public bool IsExpiredAt(DateTime utcNow)
    {
        return ExpiresOnUtc.HasValue && ExpiresOnUtc.Value <= utcNow;
    }
}
