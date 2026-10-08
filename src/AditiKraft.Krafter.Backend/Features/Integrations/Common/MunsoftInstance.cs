using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Features.Integrations.Common;

public sealed class MunsoftInstance : IHistory, ISoftDelete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MunicipalityName { get; set; } = string.Empty;
    public string DemarcationCode { get; set; } = string.Empty;

    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
