using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Users;

public sealed class GetUserPermissions
{
    internal sealed class Handler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db) : IScopedHandler
    {
        public async Task<Response<List<string>>> GetPermissionsAsync(string userId,
            CancellationToken cancellationToken)
        {
            ApplicationUser? user = await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == userId, cancellationToken);

            if (user is null)
            {
                return new Response<List<string>> { IsError = true, Message = "User Not Found", StatusCode = 404 };
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user);
            List<string> roleIds = await roleManager.Roles.AsNoTracking()
                .Where(r => r.Name != null && userRoles.Contains(r.Name) && r.IsDeleted == false)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            if (roleIds.Count == 0)
            {
                return new Response<List<string>> { Data = [] };
            }

            List<string> permissions = await db.RoleClaims.AsNoTracking()
                .Where(rc => roleIds.Contains(rc.RoleId) &&
                             rc.ClaimType == AppClaimTypes.Permission &&
                             rc.IsDeleted == false)
                .Select(rc => rc.ClaimValue!)
                .Distinct()
                .ToListAsync(cancellationToken);

            return new Response<List<string>> { Data = permissions };
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder userGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            userGroup.MapGet($"/{RouteSegment.Permissions}", async (
                    [FromServices] Handler handler,
                    [FromServices] ICurrentUser currentUser) =>
                {
                    Response<List<string>> res =
                        await handler.GetPermissionsAsync(currentUser.GetUserId(), CancellationToken.None);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Response<List<string>>>()
                .RequireAuthorization();
        }
    }
}
