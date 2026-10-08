namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public class SendGridEmailSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string ReplyToEmail { get; set; } = string.Empty;
}
