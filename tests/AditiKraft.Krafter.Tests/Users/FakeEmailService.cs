using AditiKraft.Krafter.Backend.Infrastructure.Notifications;

namespace AditiKraft.Krafter.Tests.Users;

internal sealed class FakeEmailService : IEmailService
{
    public List<SendEmailRequestInput> QueuedEmails { get; } = [];

    public Task<string> QueueEmailAsync(SendEmailRequestInput request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        QueuedEmails.Add(request);
        return Task.FromResult(Guid.CreateVersion7().ToString());
    }

    public Task ProcessEmailAsync(string emailLogId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
