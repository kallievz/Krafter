namespace AditiKraft.Krafter.Backend.Features.Users.Common;

public interface IPasswordPolicyService
{
    public Task<bool> IsPasswordReusedWithinWindowAsync(
        ApplicationUser user,
        string newPassword,
        CancellationToken cancellationToken);

    public Task<bool> IsPasswordExpiredAsync(ApplicationUser user, CancellationToken cancellationToken);

    public Task<bool> ShouldSendPasswordExpiryNotificationAsync(ApplicationUser user, CancellationToken cancellationToken);

    public Task RecordPasswordChangeAsync(ApplicationUser user, CancellationToken cancellationToken);

    public void MarkPasswordExpiryNotificationSent(ApplicationUser user);
}
