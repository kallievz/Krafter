namespace AditiKraft.Krafter.Backend.Features.Tenants.Entities;

public sealed class TenantSetting
{
    public string Id { get; set; } = null!;
    public string TenantId { get; set; } = null!;
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsSensitive { get; set; }

    public Tenant Tenant { get; set; } = null!;
}
