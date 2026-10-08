using AditiKraft.Krafter.Backend.Common.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class EmailLog : CommonEntityProperty
{
    public EmailLog()
    {
        Id = Guid.CreateVersion7().ToString();
    }

    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime? SentOn { get; set; }
    public string? ErrorMessage { get; set; }
    public string? EmailTemplateId { get; set; }

    public EmailTemplate? EmailTemplate { get; set; }
}
