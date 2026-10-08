using AditiKraft.Krafter.Backend.Common.Entities;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class EmailTemplate : CommonEntityProperty
{
    public EmailTemplate()
    {
        Id = Guid.CreateVersion7().ToString();
    }

    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SubjectTemplate { get; set; } = string.Empty;
    public string BodyTemplate { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<EmailLog> EmailLogs { get; set; } = new List<EmailLog>();
}
