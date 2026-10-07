namespace AditiKraft.Krafter.Backend.Infrastructure.Notifications;

public static class NotificationsConfiguration
{
    public static IServiceCollection AddNotificationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SMTPEmailSettings smtpSettings = configuration.GetSection("SMTPEmailSettings").Get<SMTPEmailSettings>()
                                         ?? throw new InvalidOperationException("SMTP settings must be configured");

        services.AddSingleton(smtpSettings);

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
