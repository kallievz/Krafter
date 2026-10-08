using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.SendGrid;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class SendGridEmailDeliveryProvider : IEmailDeliveryProvider
{
    public string Name => "sendgrid";

    public async Task SendAsync(EmailDeliveryMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.ApiKey))
        {
            throw new InvalidOperationException("SendGrid API key is required when SendGrid provider is selected.");
        }

        ISender sender = new SendGridSender(message.ApiKey);
        Email fluentEmail = new();
        fluentEmail.Sender = sender;

        IFluentEmail fluentEmailMessage = fluentEmail
            .To(message.ToEmail)
            .SetFrom(message.SenderEmail, message.SenderName)
            .Subject(message.Subject)
            .Body(message.HtmlBody, true);

        if (!string.IsNullOrWhiteSpace(message.ReplyToEmail))
        {
            _ = fluentEmailMessage.ReplyTo(message.ReplyToEmail);
        }

        FluentEmail.Core.Models.SendResponse response = await fluentEmailMessage.SendAsync(cancellationToken).ConfigureAwait(false);
        if (!response.Successful)
        {
            string errors = string.Join(", ", response.ErrorMessages);
            throw new InvalidOperationException($"SendGrid email send failed: {errors}");
        }
    }
}
