namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class DummyEmailDeliveryProvider : IEmailDeliveryProvider
{
    public string Name => "dummy";

    public Task SendAsync(EmailDeliveryMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
