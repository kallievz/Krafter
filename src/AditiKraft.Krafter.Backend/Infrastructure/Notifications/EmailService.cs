using System.Net;
using System.Net.Mail;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Web.Configuration;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class EmailService(
    ITenantSettingService tenantSettingService,
    SMTPEmailSettings smtpEmailSettings,
    IOptions<SecuritySettings> securitySettings) : IEmailService
{
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage,
        CancellationToken cancellationToken)
    {
        string fallbackProvider = string.IsNullOrWhiteSpace(_securitySettings.Provider)
            ? "smtp"
            : _securitySettings.Provider;
        string provider = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsProvider,
            fallbackProvider,
            cancellationToken);

        if (provider.Equals("sendgrid", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "SendGrid provider is configured for this tenant, but SendGrid delivery is not implemented.");
        }

        string host = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpHost,
            smtpEmailSettings.Host,
            cancellationToken);
        int port = await tenantSettingService.GetIntValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpPort,
            smtpEmailSettings.Port,
            cancellationToken);
        string userName = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpUserName,
            smtpEmailSettings.UserName,
            cancellationToken);
        string password = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpPassword,
            smtpEmailSettings.Password,
            cancellationToken);
        string senderEmail = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpSenderEmail,
            smtpEmailSettings.SenderEmail,
            cancellationToken);
        string senderName = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.NotificationsSmtpSenderName,
            smtpEmailSettings.SenderName,
            cancellationToken);

        using SmtpClient smtpClient = new(host, port)
        {
            Credentials = new NetworkCredential(userName, password),
            EnableSsl = true
        };

        using MailMessage message = new();
        message.From = new MailAddress(senderEmail, senderName);
        message.To.Add(new MailAddress(email));
        message.Subject = subject;
        message.Body = htmlMessage;
        message.IsBodyHtml = true;
        await smtpClient.SendMailAsync(message, cancellationToken);
    }
}
