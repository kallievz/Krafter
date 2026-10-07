using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.Users;

public sealed class ResetPassword
{
    internal sealed class Handler(
        UserManager<ApplicationUser> userManager,
        IPasswordPolicyService passwordPolicyService) : IScopedHandler
    {
        public async Task<Response> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email?.Normalize()!);
            if (user is null)
            {
                return new Response
                {
                    Message = "If the email is registered, you will receive a password reset link.",
                    StatusCode = 200,
                    IsError = false
                };
            }

            if (await passwordPolicyService.IsPasswordReusedWithinWindowAsync(user, request.Password!, cancellationToken))
            {
                return new Response
                {
                    Message = "You cannot reuse a recently used password.",
                    StatusCode = 400,
                    IsError = true
                };
            }

            IdentityResult result = await userManager.ResetPasswordAsync(user, request.Token!, request.Password!);
            if (!result.Succeeded)
            {
                return new Response
                {
                    Message = "An error occurred while resetting password",
                    StatusCode = 400,
                    IsError = true
                };
            }

            await passwordPolicyService.RecordPasswordChangeAsync(user, cancellationToken);
            return new Response();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder userGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            userGroup.MapPost($"/{RouteSegment.ResetPassword}", async (
                    [FromBody] ResetPasswordRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response res = await handler.ResetPasswordAsync(request, cancellationToken);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Response>();
        }
    }
}
