namespace AditiKraft.Krafter.Backend.Features.Tenants.Common;

public static class TenantSettingKeys
{
    public const string NotificationProviderSmtp = "smtp";
    public const string NotificationProviderSendGrid = "sendgrid";
    public const string NotificationProviderDummy = "dummy";

    public const string SecurityPasswordReusePreventionWindowInDays = "Security:Password:ReusePreventionWindowInDays";
    public const string SecurityPasswordExpiryInDays = "Security:Password:ExpiryInDays";
    public const string SecurityPasswordExpiryNotificationCooldownInDays = "Security:Password:ExpiryNotificationCooldownInDays";

    public const string NotificationsProvider = "Notifications:Provider";
    public const string NotificationsSmtpHost = "Notifications:Smtp:Host";
    public const string NotificationsSmtpPort = "Notifications:Smtp:Port";
    public const string NotificationsSmtpUserName = "Notifications:Smtp:UserName";
    public const string NotificationsSmtpPassword = "Notifications:Smtp:Password";
    public const string NotificationsSmtpSenderEmail = "Notifications:Smtp:SenderEmail";
    public const string NotificationsSmtpSenderName = "Notifications:Smtp:SenderName";
    public const string NotificationsSmtpReplyToEmail = "Notifications:Smtp:ReplyToEmail";

    public const string NotificationsSendGridApiKey = "Notifications:SendGrid:ApiKey";
    public const string NotificationsSendGridSenderEmail = "Notifications:SendGrid:SenderEmail";
    public const string NotificationsSendGridSenderName = "Notifications:SendGrid:SenderName";
    public const string NotificationsSendGridReplyToEmail = "Notifications:SendGrid:ReplyToEmail";
}
