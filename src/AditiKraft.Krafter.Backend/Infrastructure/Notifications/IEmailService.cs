namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public interface IEmailService
{
    public Task<string> QueueEmailAsync(SendEmailRequestInput request, CancellationToken cancellationToken);

    public Task ProcessEmailAsync(string emailLogId, CancellationToken cancellationToken);

    public Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken);
}
