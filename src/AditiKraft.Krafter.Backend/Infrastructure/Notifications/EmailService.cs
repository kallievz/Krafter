using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public sealed class EmailService(
    ApplicationDbContext applicationDbContext,
    ITenantSettingService tenantSettingService,
    ITenantGetterService tenantGetterService,
    IEnumerable<IEmailDeliveryProvider> deliveryProviders,
    SMTPEmailSettings smtpEmailSettings,
    SendGridEmailSettings sendGridEmailSettings,
    IOptions<SecuritySettings> securitySettings) : IEmailService
{
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<string> QueueEmailAsync(SendEmailRequestInput request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new ArgumentException("Email recipient is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Subject))
        {
            throw new ArgumentException("Email subject is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.HtmlMessage))
        {
            throw new ArgumentException("Email body is required.", nameof(request));
        }

        string fallbackProvider = string.IsNullOrWhiteSpace(_securitySettings.Provider)
            ? TenantSettingKeys.EmailProviderSmtp
            : _securitySettings.Provider;
        string provider = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailProvider,
            fallbackProvider,
            cancellationToken).ConfigureAwait(false);

        string tenantId = string.IsNullOrWhiteSpace(request.TenantId)
            ? tenantGetterService.Tenant.Id ?? SeedDataConstants.RootTenant.Id
            : request.TenantId;

        EmailLog emailLog = new()
        {
            TenantId = tenantId,
            ToEmail = request.Email,
            Subject = request.Subject,
            Body = request.HtmlMessage,
            Provider = provider,
            Status = "Pending",
            EmailTemplateId = request.EmailTemplateId
        };

        applicationDbContext.EmailLogs.Add(emailLog);
        _ = await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return emailLog.Id;
    }

    public async Task ProcessEmailAsync(string emailLogId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(emailLogId))
        {
            throw new ArgumentException("Email log id is required.", nameof(emailLogId));
        }

        EmailLog? emailLog = await applicationDbContext.EmailLogs
            .FirstOrDefaultAsync(log => log.Id == emailLogId, cancellationToken).ConfigureAwait(false) ?? throw new InvalidOperationException($"Email log not found for id '{emailLogId}'.");
        if (!emailLog.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            EmailDeliveryMessage deliveryMessage = await BuildDeliveryMessageAsync(emailLog, cancellationToken).ConfigureAwait(false);
            IEmailDeliveryProvider deliveryProvider = ResolveProvider(deliveryMessage.Provider);

            await deliveryProvider.SendAsync(deliveryMessage, cancellationToken).ConfigureAwait(false);

            emailLog.Status = "Sent";
            emailLog.SentOn = DateTime.UtcNow;
            emailLog.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            emailLog.Status = "Failed";
            emailLog.ErrorMessage = ex.Message;
            throw;
        }
        finally
        {
            _ = await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken)
    {
        string emailLogId = await QueueEmailAsync(new SendEmailRequestInput
        {
            Email = email,
            Subject = subject,
            HtmlMessage = htmlMessage
        }, cancellationToken);

        await ProcessEmailAsync(emailLogId, cancellationToken);
    }

    private IEmailDeliveryProvider ResolveProvider(string provider)
    {
        IEmailDeliveryProvider? deliveryProvider = deliveryProviders.FirstOrDefault(
            item => item.Name.Equals(provider, StringComparison.OrdinalIgnoreCase)) ?? throw new InvalidOperationException($"Email provider '{provider}' is not supported.");
        return deliveryProvider;
    }

    private async Task<EmailDeliveryMessage> BuildDeliveryMessageAsync(EmailLog emailLog, CancellationToken cancellationToken)
    {
        string provider = emailLog.Provider;

        if (provider.Equals(TenantSettingKeys.EmailProviderSendGrid, StringComparison.OrdinalIgnoreCase))
        {
            string senderEmail = await tenantSettingService.GetStringValueOrDefaultAsync(
                TenantSettingKeys.EmailSenderEmail,
                sendGridEmailSettings.SenderEmail,
                cancellationToken).ConfigureAwait(false);
            string senderName = await tenantSettingService.GetStringValueOrDefaultAsync(
                TenantSettingKeys.EmailSenderName,
                sendGridEmailSettings.SenderName,
                cancellationToken).ConfigureAwait(false);
            string replyToEmail = await tenantSettingService.GetStringValueOrDefaultAsync(
                TenantSettingKeys.EmailReplyToEmail,
                sendGridEmailSettings.ReplyToEmail,
                cancellationToken).ConfigureAwait(false);
            string apiKey = await tenantSettingService.GetStringValueOrDefaultAsync(
                TenantSettingKeys.EmailSendGridApiKey,
                sendGridEmailSettings.ApiKey,
                cancellationToken).ConfigureAwait(false);

            return new EmailDeliveryMessage(
                emailLog.ToEmail,
                emailLog.Subject,
                emailLog.Body,
                provider,
                senderEmail,
                senderName,
                replyToEmail,
                null,
                null,
                null,
                null,
                apiKey);
        }

        if (provider.Equals(TenantSettingKeys.EmailProviderDummy, StringComparison.OrdinalIgnoreCase))
        {
            return new EmailDeliveryMessage(
                emailLog.ToEmail,
                emailLog.Subject,
                emailLog.Body,
                provider,
                string.Empty,
                string.Empty,
                null,
                null,
                null,
                null,
                null,
                null);
        }

        string host = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailSmtpHost,
            smtpEmailSettings.Host,
            cancellationToken);
        int port = await tenantSettingService.GetIntValueOrDefaultAsync(
            TenantSettingKeys.EmailSmtpPort,
            smtpEmailSettings.Port,
            cancellationToken);
        string userName = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailSmtpUserName,
            smtpEmailSettings.UserName,
            cancellationToken);
        string password = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailSmtpPassword,
            smtpEmailSettings.Password,
            cancellationToken);
        string smtpSenderEmail = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailSenderEmail,
            smtpEmailSettings.SenderEmail,
            cancellationToken);
        string smtpSenderName = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailSenderName,
            smtpEmailSettings.SenderName,
            cancellationToken);
        string smtpReplyToEmail = await tenantSettingService.GetStringValueOrDefaultAsync(
            TenantSettingKeys.EmailReplyToEmail,
            smtpEmailSettings.ReplyToEmail,
            cancellationToken);

        return new EmailDeliveryMessage(
            emailLog.ToEmail,
            emailLog.Subject,
            emailLog.Body,
            TenantSettingKeys.EmailProviderSmtp,
            smtpSenderEmail,
            smtpSenderName,
            smtpReplyToEmail,
            host,
            port,
            userName,
            password,
            null);
    }
}
