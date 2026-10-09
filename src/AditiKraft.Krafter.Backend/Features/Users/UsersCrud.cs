using System.Linq.Dynamic.Core;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using AditiKraft.Krafter.Contracts.Contracts.Users;
using LinqKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Users;

public sealed class UsersCrud
{
    internal sealed class Handler(
        IUserMutationService mutationService,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUser currentUser,
        ITenantGetterService tenantGetterService,
        IJobService jobService,
        IEmailService emailService,
        IPasswordPolicyService passwordPolicyService,
        ApplicationDbContext db) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<UserDto>>> GetAsync(
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            ExpressionStarter<ApplicationUser>? predicate = PredicateBuilder.New<ApplicationUser>(true);

            if (!string.IsNullOrWhiteSpace(requestInput.Id))
            {
                predicate = predicate.And(user => user.Id == requestInput.Id);
            }

            IQueryable<UserDto> query = db.Users
                .Where(predicate)
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedOn = user.CreatedOn,
                    PhoneNumber = user.PhoneNumber,
                    PreferredLanguageCode = user.PreferredLanguageCode,
                    IsDeleted = user.IsDeleted,
                    DeleteReason = user.DeleteReason,
                    CreatedById = user.CreatedById,
                    CreatedBy = user.CreatedBy != null
                        ? new UserInfo
                        {
                            Id = user.CreatedBy.Id,
                            FirstName = user.CreatedBy.FirstName,
                            LastName = user.CreatedBy.LastName,
                            CreatedOn = user.CreatedBy.CreatedOn
                        }
                        : null,
                    UpdatedOn = user.UpdatedOn,
                    UpdatedBy = user.UpdatedBy != null
                        ? new UserInfo
                        {
                            Id = user.UpdatedBy.Id,
                            FirstName = user.UpdatedBy.FirstName,
                            LastName = user.UpdatedBy.LastName,
                            CreatedOn = user.UpdatedBy.CreatedOn
                        }
                        : null,
                    UpdatedById = user.UpdatedById
                });

            if (!string.IsNullOrEmpty(requestInput.Filter))
            {
                if (requestInput.Filter.Contains("!=") ||
                    requestInput.Filter.Contains("==") ||
                    requestInput.Filter.Contains(".Contains(") ||
                    requestInput.Filter.Contains(".StartsWith(") ||
                    requestInput.Filter.Contains(".EndsWith(") ||
                    requestInput.Filter.Contains("np("))
                {
                    query = query.Where(requestInput.Filter);
                }
                else
                {
                    string filter = requestInput.Filter.ToLower();
                    query = query.Where(user =>
                        (user.FirstName ?? string.Empty).Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                        (user.LastName ?? string.Empty).Contains(filter, StringComparison.CurrentCultureIgnoreCase));
                }
            }

            if (!string.IsNullOrEmpty(requestInput.OrderBy))
            {
                query = query.OrderBy(requestInput.OrderBy);
            }

            List<UserDto> items = await query
                .PageBy(requestInput)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            PaginationResponse<UserDto> result = new PaginationResponse<UserDto>(
                items,
                totalCount,
                requestInput.SkipCount,
                requestInput.MaxResultCount);

            return Response<PaginationResponse<UserDto>>.Success(result);
        }

        public Task<Response> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
        {
            return mutationService.CreateAsync(request, cancellationToken);
        }

        public Task<Response> PatchAsync(string id, CreateUserRequest request, CancellationToken cancellationToken)
        {
            return mutationService.UpdateAsync(id, request, cancellationToken);
        }

        public async Task<Response> UpdatePreferredLanguageAsync(
            ICurrentUser current,
            UpdatePreferredLanguageRequest request,
            CancellationToken cancellationToken)
        {
            string? userId = current.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response.Unauthorized("User not authenticated.");
            }

            return await mutationService.UpdatePreferredLanguageCodeAsync(userId, request.PreferredLanguageCode,
                cancellationToken);
        }

        public async Task<Response> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return Response.NotFound("User Not Found");
            }

            if (user.IsOwner)
            {
                return Response.Forbidden("Owner cannot be deleted");
            }

            user.IsDeleted = true;
            db.Users.Update(user);

            List<ApplicationUserRole> userRoles = await db.UserRoles
                .Where(current => current.UserId == id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (ApplicationUserRole userRole in userRoles)
            {
                userRole.IsDeleted = true;
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response<PaginationResponse<UserInfo>>> GetByRoleAsync(
            string roleId,
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            ExpressionStarter<ApplicationUserRole>? predicate = PredicateBuilder.New<ApplicationUserRole>(true);
            predicate = predicate.And(current => current.RoleId == roleId);

            IQueryable<UserInfo> query = db.UserRoles
                .Where(predicate)
                .Include(current => current.User)
                .Select(current => new UserInfo
                {
                    Id = current.User.Id,
                    FirstName = current.User.FirstName,
                    LastName = current.User.LastName,
                    CreatedOn = current.User.CreatedOn
                });

            if (!string.IsNullOrEmpty(requestInput.Filter))
            {
                if (requestInput.Filter.Contains("!=") ||
                    requestInput.Filter.Contains("==") ||
                    requestInput.Filter.Contains(".Contains(") ||
                    requestInput.Filter.Contains(".StartsWith(") ||
                    requestInput.Filter.Contains(".EndsWith(") ||
                    requestInput.Filter.Contains("np("))
                {
                    query = query.Where(requestInput.Filter);
                }
                else
                {
                    string filter = requestInput.Filter.ToLower();
                    query = query.Where(current =>
                        (current.FirstName ?? string.Empty).Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                        (current.LastName ?? string.Empty).Contains(filter, StringComparison.CurrentCultureIgnoreCase));
                }
            }

            if (!string.IsNullOrEmpty(requestInput.OrderBy))
            {
                query = query.OrderBy(requestInput.OrderBy);
            }

            List<UserInfo> items = await query
                .PageBy(requestInput)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            PaginationResponse<UserInfo> result = new PaginationResponse<UserInfo>(
                items,
                totalCount,
                requestInput.SkipCount,
                requestInput.MaxResultCount);

            return Response<PaginationResponse<UserInfo>>.Success(result);
        }

        public async Task<Response<List<string>>> GetPermissionsAsync(string userId, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(current => current.Id == userId, cancellationToken)
                .ConfigureAwait(false);

            if (user is null)
            {
                return Response<List<string>>.NotFound("User Not Found");
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            List<string> roleIds = await roleManager.Roles.AsNoTracking()
                .Where(role => role.Name != null && userRoles.Contains(role.Name) && role.IsDeleted == false)
                .Select(role => role.Id)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (roleIds.Count == 0)
            {
                return Response<List<string>>.Success([]);
            }

            List<string> permissions = await db.RoleClaims.AsNoTracking()
                .Where(roleClaim => roleIds.Contains(roleClaim.RoleId) &&
                                    roleClaim.ClaimType == AppClaimTypes.Permission &&
                                    roleClaim.IsDeleted == false)
                .Select(roleClaim => roleClaim.ClaimValue!)
                .Distinct()
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<string>>.Success(permissions);
        }

        public async Task<Response<List<UserRoleDto>>> GetRolesAsync(string userId, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user is null)
            {
                return Response<List<UserRoleDto>>.NotFound("User Not Found");
            }

            IList<string> userRoleNames = await userManager.GetRolesAsync(user).ConfigureAwait(false);
            List<UserRoleDto> userRoles = await roleManager.Roles.AsNoTracking()
                .Where(role => role.Name != null && userRoleNames.Contains(role.Name))
                .Select(role => new UserRoleDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = role.Description,
                    Enabled = true
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<UserRoleDto>>.Success(userRoles);
        }

        public async Task<Response> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            string? currentUserId = currentUser.GetUserId();
            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return Response.Unauthorized("User not authenticated.");
            }

            ApplicationUser? user = await userManager.FindByIdAsync(currentUserId).ConfigureAwait(false);
            if (user is null)
            {
                return Response.NotFound("User Not Found");
            }

            if (await passwordPolicyService.IsPasswordReusedWithinWindowAsync(user, request.NewPassword, cancellationToken)
                    .ConfigureAwait(false))
            {
                return Response.BadRequest("You cannot reuse a recently used password.");
            }

            IdentityResult result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword)
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Response.BadRequest("Current password is incorrect");
            }

            await passwordPolicyService.RecordPasswordChangeAsync(user, cancellationToken).ConfigureAwait(false);

            string emailSubject = "Password Changed";
            string userName = $"{user.FirstName} {user.LastName}";
            string emailBody = $@"
<html>
<head>
    <title>Password Changed</title>
</head>
<body>
    <p>Hello {userName},</p>
    <p>Your password has been successfully changed. If you did not initiate this change, please contact our support team immediately.</p>
    <p>Here are some tips to keep your account secure:</p>
    <ul>
        <li>Never share your password with anyone.</li>
    </ul>
    <p>Best regards,</p>
    <p>{tenantGetterService.Tenant.Name} Team</p>
</body>
</html>";

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                string emailLogId = await emailService.QueueEmailAsync(
                        new SendEmailRequestInput { Email = user.Email, Subject = emailSubject, HtmlMessage = emailBody },
                        cancellationToken)
                    .ConfigureAwait(false);
                await jobService.EnqueueByLogIdAsync(emailLogId, nameof(Jobs.SendEmailJobAsync), cancellationToken)
                    .ConfigureAwait(false);
            }

            return Response.Success();
        }

        public async Task<Response> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email.Normalize()!).ConfigureAwait(false);
            if (user is null)
            {
                return Response.NotFound("User Not Found");
            }

            string code = await userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            const string route = "account/reset-password";
            Uri endpointUri = new Uri(string.Concat($"{tenantGetterService.Tenant.TenantLink}/", route));
            string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);

            string emailSubject = "Reset Password";
            string emailBody = $"Hello {user.FirstName} {user.LastName},<br/><br/>" +
                               "We received a request to reset your password. " +
                               "Please reset your password by clicking the link below:<br/><br/>" +
                               $"<a href='{passwordResetUrl}'>Reset Password</a><br/><br/>" +
                               "If you did not request a password reset, please ignore this email.<br/><br/>" +
                               $"Regards,<br/>{tenantGetterService.Tenant.Name} Team";

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                string emailLogId = await emailService.QueueEmailAsync(
                        new SendEmailRequestInput { Email = user.Email, Subject = emailSubject, HtmlMessage = emailBody },
                        cancellationToken)
                    .ConfigureAwait(false);
                await jobService.EnqueueByLogIdAsync(emailLogId, nameof(Jobs.SendEmailJobAsync), cancellationToken)
                    .ConfigureAwait(false);
            }

            return Response.Success();
        }

        public async Task<Response> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email?.Normalize()!).ConfigureAwait(false);
            if (user is null)
            {
                return Response.Success("If the email is registered, you will receive a password reset link.");
            }

            if (await passwordPolicyService.IsPasswordReusedWithinWindowAsync(user, request.Password!, cancellationToken)
                    .ConfigureAwait(false))
            {
                return Response.BadRequest("You cannot reuse a recently used password.");
            }

            IdentityResult result = await userManager.ResetPasswordAsync(user, request.Token!, request.Password!)
                .ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Response.BadRequest("An error occurred while resetting password");
            }

            await passwordPolicyService.RecordPasswordChangeAsync(user, cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder userGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            userGroup.MapGet("/", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<UserDto>> response = await handler
                        .GetAsync(requestInput, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<UserDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Users);

            userGroup.MapPost("/", async (
                    [FromBody] CreateUserRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Users);

            userGroup.MapPatch($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromBody] CreateUserRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.PatchAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Users);

            userGroup.MapDelete($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Users);

            userGroup.MapGet($"/{RouteSegment.ByRole}", async (
                    [FromRoute] string roleId,
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<UserInfo>> response = await handler
                        .GetByRoleAsync(roleId, requestInput, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<UserInfo>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Users);

            userGroup.MapGet($"/{RouteSegment.Permissions}", async (
                    [FromServices] Handler handler,
                    [FromServices] ICurrentUser currentUser,
                    CancellationToken cancellationToken) =>
                {
                    string? userId = currentUser.GetUserId();
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        Response<List<string>> unauthorized = Response<List<string>>.Unauthorized("User not authenticated.");
                        return Results.Json(unauthorized, statusCode: unauthorized.StatusCode);
                    }

                    Response<List<string>> response = await handler
                        .GetPermissionsAsync(userId, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<string>>>()
                .RequireAuthorization();


            userGroup.MapGet($"/{RouteSegment.UserRoles}", async (
                    [FromRoute] string userId,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<UserRoleDto>> response = await handler
                        .GetRolesAsync(userId, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<UserRoleDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.UserRoles);

            userGroup.MapPost($"/{RouteSegment.ChangePassword}", async (
                    [FromBody] ChangePasswordRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.ChangePasswordAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .RequireAuthorization();

            userGroup.MapPost("/profile/language", async (
                    [FromBody] UpdatePreferredLanguageRequest request,
                    [FromServices] Handler handler,
                    [FromServices] ICurrentUser current,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdatePreferredLanguageAsync(current, request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .RequireAuthorization();

            userGroup.MapPost($"/{RouteSegment.ForgotPassword}", async (
                    [FromBody] ForgotPasswordRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.ForgotPasswordAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>();

            userGroup.MapPost($"/{RouteSegment.ResetPassword}", async (
                    [FromBody] ResetPasswordRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.ResetPasswordAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>();
        }
    }
}
