using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using AditiKraft.Krafter.Contracts.Contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Tests.Users;

public sealed class UserMutationTests
{
    [Fact]
    public async Task CreateAssignsBasicOnceAndQueuesEmailAfterSavingRolesAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationRole basic = await context.AddRoleAsync(RoleConstants.Basic);
        var request = new CreateUserRequest
        {
            FirstName = "New",
            LastName = "User",
            Email = "new@example.com",
            Roles = [basic.Id, basic.Id]
        };

        Response result = await new UsersCrud.Handler(
            context.Mutations,
            context.Users,
            context.Roles,
            new CurrentUser(),
            context.Tenant,
            context.Jobs,
            context.EmailService,
            context.PasswordPolicies,
            context.Db).CreateAsync(request, CancellationToken.None);

        Assert.False(result.IsError, result.Message);
        context.Db.ChangeTracker.Clear();
        ApplicationUser user = await context.Db.Users.SingleAsync();
        Assert.Equal("new@example.com", user.UserName);
        Assert.True(await context.Users.CheckPasswordAsync(user,
            ExtractPassword(Assert.Single(context.EmailService.QueuedEmails).HtmlMessage)));
        ApplicationUserRole assignment = await context.Db.UserRoles.SingleAsync();
        Assert.Equal(user.Id, assignment.UserId);
        Assert.Equal(basic.Id, assignment.RoleId);
        Assert.Equal("tenant-a", assignment.TenantId);
    }

    [Fact]
    public async Task UpdateProfileUsesSharedSaveAndRestoresRoleAssignmentsAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationRole basic = await context.AddRoleAsync(RoleConstants.Basic);
        ApplicationRole editor = await context.AddRoleAsync("Editor");
        ApplicationUser user = await context.AddUserAsync();
        context.Db.UserRoles.AddRange(
            new ApplicationUserRole { UserId = user.Id, RoleId = basic.Id },
            new ApplicationUserRole { UserId = user.Id, RoleId = editor.Id, IsDeleted = true });
        await context.Db.SaveChangesAsync();
        context.TenantDb.Tenants.AddRange(
            new Tenant { Id = "tenant-a", Identifier = "a", Name = "A", AdminEmail = user.Email! },
            new Tenant { Id = "tenant-b", Identifier = "b", Name = "B", AdminEmail = user.Email! });
        await context.TenantDb.SaveChangesAsync();
        string? passwordHash = user.PasswordHash;

        Response result = await new UsersCrud.Handler(
            context.Mutations,
            context.Users,
            context.Roles,
            new CurrentUser(),
            context.Tenant,
            context.Jobs,
            context.EmailService,
            context.PasswordPolicies,
            context.Db).PatchAsync(user.Id, new CreateUserRequest
        {
            FirstName = "Changed",
            LastName = "Name",
            PhoneNumber = null,
            Email = "changed@example.com",
            Roles = [editor.Id, editor.Id]
        }, CancellationToken.None);

        Assert.False(result.IsError, result.Message);
        context.Db.ChangeTracker.Clear();
        user = await context.Db.Users.SingleAsync();
        Assert.Equal("Changed", user.FirstName);
        Assert.Equal("Name", user.LastName);
        Assert.Null(user.PhoneNumber);
        Assert.Equal("CHANGED@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal(passwordHash, user.PasswordHash);
        List<ApplicationUserRole> assignments = await context.Db.UserRoles.IgnoreQueryFilters().ToListAsync();
        Assert.Equal(2, assignments.Count);
        Assert.True(assignments.Single(role => role.RoleId == basic.Id).IsDeleted);
        Assert.False(assignments.Single(role => role.RoleId == editor.Id).IsDeleted);
        Assert.Equal("changed@example.com", (await context.TenantDb.Tenants.SingleAsync(t => t.Id == "tenant-a")).AdminEmail);
        Assert.Equal("user@example.com", (await context.TenantDb.Tenants.SingleAsync(t => t.Id == "tenant-b")).AdminEmail);
        Assert.Empty(context.EmailService.QueuedEmails);
    }

    [Fact]
    public async Task TenantEmailUpdatePreservesProfilePasswordAndRolesAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationRole admin = await context.AddRoleAsync(RoleConstants.Admin);
        ApplicationUser user = await context.AddUserAsync();
        context.Db.UserRoles.Add(new ApplicationUserRole { UserId = user.Id, RoleId = admin.Id });
        await context.Db.SaveChangesAsync();
        string? passwordHash = user.PasswordHash;

        Response result = await context.Mutations.UpdateEmailAsync(user.Id, "admin.changed@example.com", CancellationToken.None);

        Assert.False(result.IsError, result.Message);
        context.Db.ChangeTracker.Clear();
        user = await context.Db.Users.SingleAsync();
        Assert.Equal("First", user.FirstName);
        Assert.Equal("Last", user.LastName);
        Assert.Equal("123456", user.PhoneNumber);
        Assert.Equal("admin.changed@example.com", user.Email);
        Assert.Equal("ADMIN.CHANGED@EXAMPLE.COM", user.NormalizedEmail);
        Assert.Equal("ADMIN.CHANGED@EXAMPLE.COM", user.NormalizedUserName);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(admin.Id, (await context.Db.UserRoles.SingleAsync()).RoleId);
        Assert.Empty(context.EmailService.QueuedEmails);
    }

    [Fact]
    public async Task ForeignTenantRoleIsRejectedBeforeUserChangesAreSavedAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationUser user = await context.AddUserAsync();
        context.SetTenant("tenant-b");
        ApplicationRole foreignRole = await context.AddRoleAsync("Foreign");
        context.SetTenant("tenant-a");
        context.Db.ChangeTracker.Clear();

        Response result = await context.Mutations.UpdateAsync(user.Id, new CreateUserRequest
        {
            FirstName = "Changed",
            LastName = "Name",
            Email = "changed@example.com",
            Roles = [foreignRole.Id]
        }, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
        context.Db.ChangeTracker.Clear();
        user = await context.Db.Users.SingleAsync();
        Assert.Equal("First", user.FirstName);
        Assert.Equal("user@example.com", user.Email);
        Assert.Empty(await context.Db.UserRoles.ToListAsync());
    }

    [Fact]
    public async Task FailedEmailUpdateReturnsIdentityErrorsAndDoesNotSaveNewEmailAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationUser user = await context.AddUserAsync();
        await context.AddUserAsync("taken@example.com");

        Response result = await context.Mutations.UpdateEmailAsync(user.Id, "taken@example.com", CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(400, result.StatusCode);
        Assert.Contains("taken", result.Message!, StringComparison.OrdinalIgnoreCase);
        context.Db.ChangeTracker.Clear();
        Assert.Equal("user@example.com", (await context.Db.Users.SingleAsync(u => u.Id == user.Id)).Email);
    }

    [Fact]
    public async Task TelephoneNumberUpdatePreservesProfileEmailAndRolesAsync()
    {
        await using var context = new IdentityTestContext();
        ApplicationRole admin = await context.AddRoleAsync(RoleConstants.Admin);
        ApplicationUser user = await context.AddUserAsync();
        context.Db.UserRoles.Add(new ApplicationUserRole { UserId = user.Id, RoleId = admin.Id });
        await context.Db.SaveChangesAsync();
        string? passwordHash = user.PasswordHash;

        Response result = await context.Mutations.UpdateTelephoneNumberAsync(user.Id, "987654", CancellationToken.None);

        Assert.False(result.IsError, result.Message);
        context.Db.ChangeTracker.Clear();
        user = await context.Db.Users.SingleAsync();
        Assert.Equal("First", user.FirstName);
        Assert.Equal("Last", user.LastName);
        Assert.Equal("user@example.com", user.Email);
        Assert.Equal("987654", user.PhoneNumber);
        Assert.Equal(passwordHash, user.PasswordHash);
        Assert.Equal(admin.Id, (await context.Db.UserRoles.SingleAsync()).RoleId);
        Assert.Empty(context.EmailService.QueuedEmails);
    }

    private static string ExtractPassword(string htmlMessage)
    {
        const string start = "Your password is:<br/>";
        int offset = htmlMessage.IndexOf(start, StringComparison.Ordinal) + start.Length;
        return htmlMessage[offset..htmlMessage.IndexOf("<br/>", offset, StringComparison.Ordinal)];
    }
}

