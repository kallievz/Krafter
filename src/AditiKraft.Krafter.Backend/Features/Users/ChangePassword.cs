using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.Users;

public sealed class ChangePassword
{
    internal sealed class Handler(
        UserManager<ApplicationUser> userManager,
        ICurrentUser currentUser,
        ITenantGetterService tenantGetterService,
        IJobService jobService,
        IEmailService emailService,
        IPasswordPolicyService passwordPolicyService) : IScopedHandler
    {
        public async Task<Response> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByIdAsync(currentUser.GetUserId());
            if (user is null)
            {
                return new Response { IsError = true, Message = "User Not Found", StatusCode = 404 };
            }

            if (await passwordPolicyService.IsPasswordReusedWithinWindowAsync(user, request.NewPassword, cancellationToken))
            {
                return new Response
                {
                    IsError = true,
                    Message = "You cannot reuse a recently used password.",
                    StatusCode = 400
                };
            }

            IdentityResult result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
            if (!result.Succeeded)
            {
                return new Response { IsError = true, Message = "Current password is incorrect", StatusCode = 400 };
            }

            await passwordPolicyService.RecordPasswordChangeAsync(user, cancellationToken);

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
                    cancellationToken);
                await jobService.EnqueueByLogIdAsync(emailLogId, nameof(Jobs.SendEmailJobAsync), cancellationToken);
            }

            return new Response();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder userGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            userGroup.MapPost($"/{RouteSegment.ChangePassword}", async (
                    [FromBody] ChangePasswordRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response res = await handler.ChangePasswordAsync(request, cancellationToken);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Response>()
                .RequireAuthorization();
        }
    }
}
