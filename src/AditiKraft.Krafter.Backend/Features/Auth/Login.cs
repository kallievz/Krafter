using AditiKraft.Krafter.Backend.Features.Auth.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Features.Auth;

public sealed class Login
{
    internal sealed class Handler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        IOptions<SecuritySettings> securitySettings
    ) : IScopedHandler
    {
        private readonly SecuritySettings _securitySettings = securitySettings.Value;

        public async Task<Response<TokenResponse>> LoginAsync(
            TokenRequest request, string ipAddress,
            CancellationToken _)
        {
            ApplicationUser? user = await userManager.FindByEmailAsync(request.Email.Trim().Normalize());
            if (user is null)
            {
                return Response<TokenResponse>.Unauthorized("Invalid Email or Password");
            }

            if (!await userManager.CheckPasswordAsync(user, request.Password))
            {
                return Response<TokenResponse>.BadRequest("Invalid Email or Password");
            }

            if (!user.IsActive)
            {
                return Response<TokenResponse>.BadRequest("User Not Active. Please contact the administrator.");
            }

            if (_securitySettings.RequireConfirmedAccount && !user.EmailConfirmed)
            {
                return Response<TokenResponse>.BadRequest("E-Mail not confirmed.");
            }

            return Response<TokenResponse>.Success(await tokenService.GenerateTokensAndUpdateUser(user, ipAddress));
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder productGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tokens)
                .AddFluentValidationFilter();

            productGroup.MapPost("/", async
            ([FromBody] TokenRequest request, HttpContext context,
                [FromServices] Handler handler) =>
            {
                if (string.IsNullOrWhiteSpace(request.TenantId))
                {
                    request.TenantId = context.Request.Headers["x-tenant-identifier"].ToString();
                }

                string? ipAddress = GetIpAddress(context);
                Response<TokenResponse> res = await handler.LoginAsync(request, ipAddress!, CancellationToken.None);
                return Results.Json(res, statusCode: res.StatusCode);
            }).Produces<Response<TokenResponse>>();
        }

        private string? GetIpAddress(HttpContext httpContext)
        {
            return httpContext.Request.Headers.ContainsKey("X-Forwarded-For")
                ? httpContext.Request.Headers["X-Forwarded-For"]
                : httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? "N/A";
        }
    }
}
