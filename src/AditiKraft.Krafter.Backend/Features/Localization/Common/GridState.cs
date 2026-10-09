using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Features.Localization.Common;

public sealed class GridState : IHistory, ITenant
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ComponentKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StateJson { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public DateTime CreatedOn { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
