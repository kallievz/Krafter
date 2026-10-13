using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Entities;

namespace AditiKraft.Krafter.Backend.Features.Integrations.Entities;

public sealed class MunsoftIndigentLog : IHistory, ISoftDelete
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public int InstanceId { get; set; }
    public string? MDBCode { get; set; }
    public string? AccountNumber { get; set; }
    public string? IndigentSequenceNumber { get; set; }
    
    public int? RowNumber { get; set; }
    public string? EntryJson { get; set; }
    public string? ErrorMessage { get; set; }

    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
