namespace AditiKraft.Krafter.Backend.Common.Entities;

public sealed class AuditLog
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string TableName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Added, Modified, Deleted
    public string KeyValues { get; set; } = string.Empty; // JSON
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
