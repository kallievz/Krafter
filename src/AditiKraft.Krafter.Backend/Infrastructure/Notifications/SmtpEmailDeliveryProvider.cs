using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Core.Interfaces;
using FluentEmail.Smtp;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class SmtpEmailDeliveryProvider : IEmailDeliveryProvider
{
    public string Name => "smtp";

    public async Task SendAsync(EmailDeliveryMessage message, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(message.Host))
        {
            throw new InvalidOperationException("SMTP host is required when SMTP provider is selected.");
        }

        if (message.Port is null)
        {
            throw new InvalidOperationException("SMTP port is required when SMTP provider is selected.");
        }

        using SmtpClient smtpClient = new(message.Host, message.Port.Value)
        {
            Credentials = new NetworkCredential(message.UserName, message.Password),
            EnableSsl = true
        };

        ISender sender = new SmtpSender(() => smtpClient);
        Email fluentEmail = new();
        fluentEmail.Sender = sender;

        FluentEmail.Core.IFluentEmail fluentEmailMessage = fluentEmail
            .To(message.ToEmail)
            .SetFrom(message.SenderEmail, message.SenderName)
            .Subject(message.Subject)
            .Body(message.HtmlBody, true);

        if (!string.IsNullOrWhiteSpace(message.ReplyToEmail))
        {
            _ = fluentEmailMessage.ReplyTo(message.ReplyToEmail);
        }

        FluentEmail.Core.Models.SendResponse response = await fluentEmailMessage.SendAsync(cancellationToken);
        if (!response.Successful)
        {
            string errors = string.Join(", ", response.ErrorMessages);
            throw new InvalidOperationException($"SMTP email send failed: {errors}");
        }
    }
}
