namespace AditiKraft.Krafter.Backend.Features.Tenants.Common;

public static class TenantSettingKeys
{
    public const string EmailProviderSmtp = "smtp";
    public const string EmailProviderSendGrid = "sendgrid";
    public const string EmailProviderDummy = "dummy";

    public const string SecurityPasswordReusePreventionWindowInDays = "Security:Password:ReusePreventionWindowInDays";
    public const string SecurityPasswordExpiryInDays = "Security:Password:ExpiryInDays";
    public const string SecurityPasswordExpiryNotificationCooldownInDays = "Security:Password:ExpiryNotificationCooldownInDays";

    public const string EmailProvider = "Email:Provider";
    public const string EmailSmtpHost = "Email:Smtp:Host";
    public const string EmailSmtpPort = "Email:Smtp:Port";
    public const string EmailSmtpUserName = "Email:Smtp:UserName";
    public const string EmailSmtpPassword = "Email:Smtp:Password";
    public const string EmailSendGridApiKey = "Email:SendGrid:ApiKey";
    public const string EmailSenderEmail = "Email:SenderEmail";
    public const string EmailSenderName = "Email:SenderName";
    public const string EmailReplyToEmail = "Email:ReplyToEmail";


    public const string MunsoftBaseAddess = "Munsoft:BaseAddress";
    public const string MunsoftUserName = "Munsoft:UserName";
    public const string MunsoftPassword = "Munsoft:Password";
    public const string MunsoftLogin = "Munsoft:Login";
    public const string MunsoftInstances = "Munsoft:Instances";
    public const string MunsoftInstanceId = "Munsoft:InstanceId";
    public const string MunsoftDebtorParquet = "Munsoft:DebtorParquet";
    public const string MunsoftDebtorSingle = "Munsoft:DebtorSingle";

    public const string MunsoftPropertyParquet = "Munsoft:PropertyParquet";
    public const string MunsoftPropertySingle = "Munsoft:PropertySingle";

    public const string MunsoftMeterParquet = "Munsoft:MeterParquet";
    public const string MunsoftMeterSingle = "Munsoft:MeterSingle";
    public const string MunsoftAgeingParquet = "Munsoft:AgeingParquet";
    public const string MunsoftAgeingSingle = "Munsoft:AgeingSingle";
}
