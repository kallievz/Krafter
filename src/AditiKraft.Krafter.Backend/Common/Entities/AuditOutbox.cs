namespace AditiKraft.Krafter.Backend.Common.Entities;

public sealed class AuditOutbox
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    // Serialized AuditLog payload
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedOn { get; set; }
    // Failure tracking
    public int FailureCount { get; set; }
    public string? LastError { get; set; }
}
