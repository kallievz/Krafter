using AditiKraft.Krafter.Backend.Features.Users.Common;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Tests.Users;

public sealed class PasswordPolicyTests
{
    [Fact]
    public async Task CurrentPasswordIsRejectedForReuseWithinConfiguredWindowAsync()
    {
        await using IdentityTestContext context = new();
        ApplicationUser user = await context.AddUserAsync();

        bool isReused = await context.PasswordPolicies.IsPasswordReusedWithinWindowAsync(
            user,
            "TestPassword1!",
            CancellationToken.None);

        Assert.True(isReused);
    }

    [Fact]
    public async Task RecordPasswordChangeStoresPasswordHistoryAndUpdatesUserMetadataAsync()
    {
        await using IdentityTestContext context = new();
        ApplicationUser user = await context.AddUserAsync();

        await context.PasswordPolicies.RecordPasswordChangeAsync(user, CancellationToken.None);

        context.Db.ChangeTracker.Clear();
        ApplicationUser persistedUser = await context.Db.Users.SingleAsync();
        List<PasswordHistory> history = await context.Db.PasswordHistories.ToListAsync();
        Assert.NotNull(persistedUser.LastPasswordChangedOn);
        Assert.Null(persistedUser.LastPasswordExpiryNotificationOn);
        Assert.Single(history);
        Assert.Equal(user.Id, history[0].UserId);
    }

    [Fact]
    public async Task ExpiredPasswordRequiresNotificationCooldownBeforeSendingAgainAsync()
    {
        await using IdentityTestContext context = new();
        ApplicationUser user = await context.AddUserAsync();
        user.LastPasswordChangedOn = DateTime.UtcNow.AddDays(-91);
        user.LastPasswordExpiryNotificationOn = DateTime.UtcNow;
        await context.Users.UpdateAsync(user);

        bool isExpired = await context.PasswordPolicies.IsPasswordExpiredAsync(user, CancellationToken.None);
        bool shouldNotifyWithRecentNotification =
            await context.PasswordPolicies.ShouldSendPasswordExpiryNotificationAsync(user, CancellationToken.None);

        user.LastPasswordExpiryNotificationOn = DateTime.UtcNow.AddDays(-2);
        bool shouldNotifyAfterCooldown =
            await context.PasswordPolicies.ShouldSendPasswordExpiryNotificationAsync(user, CancellationToken.None);

        Assert.True(isExpired);
        Assert.False(shouldNotifyWithRecentNotification);
        Assert.True(shouldNotifyAfterCooldown);
    }
}
