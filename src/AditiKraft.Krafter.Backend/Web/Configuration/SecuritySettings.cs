namespace AditiKraft.Krafter.Backend.Web.Configuration;

public class SecuritySettings
{
    public string? Provider { get; set; }
    public bool RequireConfirmedAccount { get; set; }
    public int PasswordReusePreventionWindowInDays { get; set; } = 90;
    public int PasswordExpiryInDays { get; set; } = 90;
    public int PasswordExpiryNotificationCooldownInDays { get; set; } = 1;
}
