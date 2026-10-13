using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Entities;
using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using AditiKraft.Krafter.Contracts.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Users.Common;

public sealed class UserMutationService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ITenantGetterService tenantGetterService,
    TenantDbContext tenantDbContext,
    ApplicationDbContext db,
    LocalizationDbContext localizationDbContext,
    IJobService jobService,
    IEmailService emailService) : IUserMutationService, IScopedService
{
    public async Task<Response> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        ApplicationRole? basic = await roleManager.FindByNameAsync(RoleConstants.Basic);
        if (basic is null)
        {
            return Response.NotFound("Basic Role Not Found.");
        }

        var roles = (request.Roles ?? []).ToHashSet(StringComparer.Ordinal);
        roles.Add(basic.Id);
        if (!await RolesExistAsync(roles, cancellationToken))
        {
            return Response.BadRequest("One or more roles were not found in the current tenant.");
        }

        string preferredLanguageCode = await NormalizePreferredLanguageCodeAsync(request.PreferredLanguageCode, cancellationToken);

        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = string.IsNullOrWhiteSpace(request.UserName) ? request.Email : request.UserName,
            PhoneNumber = request.PhoneNumber,
            PreferredLanguageCode = preferredLanguageCode,
            IsActive = true
        };

        string password = PasswordGenerator.GeneratePassword();
        IdentityResult result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            return Response.BadRequest("An error occurred while creating user.");
        }

        await SyncRolesAsync(user.Id, roles, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await SendAccountEmailAsync(user, password, cancellationToken);
        return Response.Success();
    }

    public async Task<Response> UpdateAsync(string id, CreateUserRequest request, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Response.NotFound("User Not Found");
        }

        // Existing callers omit roles when they only update a profile.
        HashSet<string>? roles = request.Roles is { Count: > 0 }
            ? request.Roles.ToHashSet(StringComparer.Ordinal)
            : null;
        if (roles is not null && !await RolesExistAsync(roles, cancellationToken))
        {
            return Response.BadRequest("One or more roles were not found in the current tenant.");
        }

        Response result = await SaveProfileAsync(
            user,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            request.PreferredLanguageCode,
            request.UpdateTenantEmail,
            cancellationToken);
        if (result.IsError)
        {
            return result;
        }

        if (roles is not null)
        {
            await SyncRolesAsync(user.Id, roles, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }

        return Response.Success();
    }

    public async Task<Response> UpdateEmailAsync(string id, string email, CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id);
        return user is null
            ? Response.NotFound("User Not Found")
            : await SaveEmailAsync(user, email, updateTenantEmail: false, cancellationToken);
    }

    public async Task<Response> UpdateTelephoneNumberAsync(
        string id,
        string? telephoneNumber,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id);
        return user is null
            ? Response.NotFound("User Not Found")
            : await SaveTelephoneNumberAsync(user, telephoneNumber);
    }

    public async Task<Response> UpdatePreferredLanguageCodeAsync(
        string id,
        string? preferredLanguageCode,
        CancellationToken cancellationToken)
    {
        ApplicationUser? user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return Response.NotFound("User Not Found");
        }

        string normalizedCode = await NormalizePreferredLanguageCodeAsync(preferredLanguageCode, cancellationToken);
        if (string.Equals(user.PreferredLanguageCode, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            return Response.Success();
        }

        user.PreferredLanguageCode = normalizedCode;
        IdentityResult result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return Response.BadRequest(
                $"Update preferred language failed: {string.Join(", ", result.Errors.Select(error => error.Description))}");
        }

        return Response.Success();
    }

    private async Task<Response> SaveProfileAsync(
        ApplicationUser user,
        string firstName,
        string lastName,
        string email,
        string? telephoneNumber,
        string? preferredLanguageCode,
        bool updateTenantEmail,
        CancellationToken cancellationToken)
    {
        bool profileChanged = false;
        if (user.FirstName != firstName)
        {
            user.FirstName = firstName;
            profileChanged = true;
        }

        if (user.LastName != lastName)
        {
            user.LastName = lastName;
            profileChanged = true;
        }

        string normalizedPreferredLanguageCode = await NormalizePreferredLanguageCodeAsync(preferredLanguageCode, cancellationToken);
        if (!string.Equals(user.PreferredLanguageCode, normalizedPreferredLanguageCode, StringComparison.OrdinalIgnoreCase))
        {
            user.PreferredLanguageCode = normalizedPreferredLanguageCode;
            profileChanged = true;
        }

        if (profileChanged)
        {
            IdentityResult profileResult = await userManager.UpdateAsync(user);
            if (!profileResult.Succeeded)
            {
                return Response.BadRequest(
                    $"Update profile failed: {string.Join(", ", profileResult.Errors.Select(e => e.Description))}");
            }
        }

        Response emailResult = await SaveEmailAsync(user, email, updateTenantEmail, cancellationToken);
        if (emailResult.IsError)
        {
            return emailResult;
        }

        return await SaveTelephoneNumberAsync(user, telephoneNumber);
    }

    private async Task<Response> SaveEmailAsync(
        ApplicationUser user,
        string email,
        bool updateTenantEmail,
        CancellationToken cancellationToken)
    {
        string? previousEmail = user.Email;
        string? previousNormalizedEmail = user.NormalizedEmail;
        string? previousUserName = user.UserName;
        string? previousNormalizedUserName = user.NormalizedUserName;

        bool emailChanged = email != previousEmail;
        if (!emailChanged)
        {
            return Response.Success();
        }

        IdentityResult emailResult = await userManager.SetEmailAsync(user, email);
        if (!emailResult.Succeeded)
        {
            user.Email = previousEmail;
            user.NormalizedEmail = previousNormalizedEmail;
            return Response.BadRequest(
                $"Update email failed: {string.Join(", ", emailResult.Errors.Select(e => e.Description))}");
        }

        IdentityResult userNameResult = await userManager.SetUserNameAsync(user, email);
        if (!userNameResult.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(previousEmail))
            {
                _ = await userManager.SetEmailAsync(user, previousEmail);
            }

            if (!string.IsNullOrWhiteSpace(previousUserName))
            {
                _ = await userManager.SetUserNameAsync(user, previousUserName);
            }

            user.Email = previousEmail;
            user.NormalizedEmail = previousNormalizedEmail;
            user.UserName = previousUserName;
            user.NormalizedUserName = previousNormalizedUserName;
            return Response.BadRequest(
                $"Update user name failed: {string.Join(", ", userNameResult.Errors.Select(e => e.Description))}");
        }

        if (updateTenantEmail)
        {
            Tenant? tenant = await tenantDbContext.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantGetterService.Tenant.Id && t.AdminEmail == previousEmail,
                    cancellationToken);
            if (tenant is not null)
            {
                tenant.AdminEmail = email;
                await tenantDbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return Response.Success();
    }

    private async Task<Response> SaveTelephoneNumberAsync(ApplicationUser user, string? telephoneNumber)
    {
        if (user.PhoneNumber == telephoneNumber)
        {
            return Response.Success();
        }

        IdentityResult result = await userManager.SetPhoneNumberAsync(user, telephoneNumber);
        if (!result.Succeeded)
        {
            return Response.BadRequest(
                $"Update telephone number failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return Response.Success();
    }

    private async Task<bool> RolesExistAsync(HashSet<string> roles, CancellationToken cancellationToken)
        => await db.Roles.CountAsync(role => roles.Contains(role.Id), cancellationToken) == roles.Count;

    private async Task SyncRolesAsync(string userId, HashSet<string> requestedRoles, CancellationToken cancellationToken)
    {
        List<ApplicationUserRole> existingRoles = await db.UserRoles.IgnoreQueryFilters()
            .Where(role => role.TenantId == tenantGetterService.Tenant.Id && role.UserId == userId)
            .ToListAsync(cancellationToken);

        foreach (ApplicationUserRole role in existingRoles)
        {
            role.IsDeleted = !requestedRoles.Contains(role.RoleId);
        }

        var existingIds = existingRoles.Select(role => role.RoleId).ToHashSet(StringComparer.Ordinal);
        db.UserRoles.AddRange(requestedRoles.Where(roleId => !existingIds.Contains(roleId))
            .Select(roleId => new ApplicationUserRole { RoleId = roleId, UserId = userId }));
    }

    private async Task<string> NormalizePreferredLanguageCodeAsync(string? preferredLanguageCode,
        CancellationToken cancellationToken)
    {
        string normalized = string.IsNullOrWhiteSpace(preferredLanguageCode)
            ? "en"
            : preferredLanguageCode.Trim().ToLowerInvariant();

        bool languageExists = await localizationDbContext.Languages.AsNoTracking()
            .AnyAsync(language => language.Code == normalized, cancellationToken);

        if (languageExists)
        {
            return normalized;
        }

        return "en";
    }

    private async Task SendAccountEmailAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        string loginUrl = $"{tenantGetterService.Tenant.TenantLink}/login";
        string emailBody = $"Hello {user.FirstName} {user.LastName},<br/><br/>" +
                           "Your account has been created successfully.<br/><br/> " +
                           $"Your username/email is:<br/>{user.UserName}<br/><br/>" +
                           $"Your password is:<br/>{password}<br/><br/>" +
                           $"Please <a href='{loginUrl}'>click here</a> to log in.<br/><br/>" +
                           $"Regards,<br/>{tenantGetterService.Tenant.Name} Team";
        string emailLogId = await emailService.QueueEmailAsync(
            new SendEmailRequestInput { Email = user.Email, Subject = "Account Created", HtmlMessage = emailBody },
            cancellationToken);
        await jobService.EnqueueByLogIdAsync(emailLogId, nameof(Jobs.SendEmailJobAsync), cancellationToken);
    }
}
