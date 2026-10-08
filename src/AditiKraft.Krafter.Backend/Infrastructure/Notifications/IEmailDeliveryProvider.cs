namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public interface IEmailDeliveryProvider
{
    public string Name { get; }

    public Task SendAsync(EmailDeliveryMessage message, CancellationToken cancellationToken);
}
