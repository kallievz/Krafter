using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Features.Users.Common;

public sealed class PasswordPolicyService(
    ApplicationDbContext db,
    UserManager<ApplicationUser> userManager,
    ITenantSettingService tenantSettingService,
    IOptions<SecuritySettings> securitySettings) : IPasswordPolicyService, IScopedService
{
    private readonly SecuritySettings _securitySettings = securitySettings.Value;

    public async Task<bool> IsPasswordReusedWithinWindowAsync(
        ApplicationUser user,
        string newPassword,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(newPassword))
        {
            return false;
        }

        int configuredWindow = await tenantSettingService.GetIntValueOrDefaultAsync(
            TenantSettingKeys.SecurityPasswordReusePreventionWindowInDays,
            _securitySettings.PasswordReusePreventionWindowInDays,
            cancellationToken);
        int policyWindow = Math.Max(0, configuredWindow);
        if (policyWindow == 0)
        {
            return false;
        }

        DateTime cutoff = DateTime.UtcNow.AddDays(-policyWindow);
        List<string> candidateHashes = await db.PasswordHistories
            .Where(history => history.UserId == user.Id && history.CreatedOn >= cutoff)
            .OrderByDescending(history => history.CreatedOn)
            .Select(history => history.PasswordHash)
            .ToListAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            candidateHashes.Add(user.PasswordHash);
        }

        IPasswordHasher<ApplicationUser> hasher = userManager.PasswordHasher;
        return candidateHashes.Any(passwordHash =>
            hasher.VerifyHashedPassword(user, passwordHash, newPassword) != PasswordVerificationResult.Failed);
    }

    public async Task<bool> IsPasswordExpiredAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        int configuredExpiry = await tenantSettingService.GetIntValueOrDefaultAsync(
            TenantSettingKeys.SecurityPasswordExpiryInDays,
            _securitySettings.PasswordExpiryInDays,
            cancellationToken);
        int passwordExpiryInDays = Math.Max(0, configuredExpiry);
        if (passwordExpiryInDays == 0)
        {
            return false;
        }

        DateTime lastChange = user.LastPasswordChangedOn ?? user.CreatedOn;
        if (lastChange == default)
        {
            return false;
        }

        return lastChange.AddDays(passwordExpiryInDays) <= DateTime.UtcNow;
    }

    public async Task<bool> ShouldSendPasswordExpiryNotificationAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        int configuredCooldown = await tenantSettingService.GetIntValueOrDefaultAsync(
            TenantSettingKeys.SecurityPasswordExpiryNotificationCooldownInDays,
            _securitySettings.PasswordExpiryNotificationCooldownInDays,
            cancellationToken);
        int cooldownInDays = Math.Max(1, configuredCooldown);
        DateTime? lastNotification = user.LastPasswordExpiryNotificationOn;
        return lastNotification is null || lastNotification.Value.AddDays(cooldownInDays) <= DateTime.UtcNow;
    }

    public async Task RecordPasswordChangeAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return;
        }

        user.LastPasswordChangedOn = DateTime.UtcNow;
        user.LastPasswordExpiryNotificationOn = null;
        db.PasswordHistories.Add(new PasswordHistory
        {
            Id = Guid.CreateVersion7().ToString(),
            UserId = user.Id,
            PasswordHash = user.PasswordHash
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public void MarkPasswordExpiryNotificationSent(ApplicationUser user)
    {
        ArgumentNullException.ThrowIfNull(user);
        user.LastPasswordExpiryNotificationOn = DateTime.UtcNow;
    }
}
