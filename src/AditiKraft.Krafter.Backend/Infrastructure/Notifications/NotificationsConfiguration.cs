namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public static class NotificationsConfiguration
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SMTPEmailSettings smtpSettings = configuration.GetSection("SMTPEmailSettings").Get<SMTPEmailSettings>()
                                         ?? new SMTPEmailSettings();
        SendGridEmailSettings sendGridSettings =
            configuration.GetSection("SendGridEmailSettings").Get<SendGridEmailSettings>()
            ?? new SendGridEmailSettings();

        services.AddSingleton(smtpSettings);
        services.AddSingleton(sendGridSettings);

        services.AddScoped<IEmailDeliveryProvider, SmtpEmailDeliveryProvider>();
        services.AddScoped<IEmailDeliveryProvider, SendGridEmailDeliveryProvider>();
        services.AddScoped<IEmailDeliveryProvider, DummyEmailDeliveryProvider>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
