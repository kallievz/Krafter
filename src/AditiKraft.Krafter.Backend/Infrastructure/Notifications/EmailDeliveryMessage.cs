namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed record EmailDeliveryMessage(
    string ToEmail,
    string Subject,
    string HtmlBody,
    string Provider,
    string SenderEmail,
    string SenderName,
    string? ReplyToEmail,
    string? Host,
    int? Port,
    string? UserName,
    string? Password,
    string? ApiKey);
