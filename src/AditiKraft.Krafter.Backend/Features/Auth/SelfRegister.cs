using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Auth;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.Auth;

public sealed class SelfRegister
{
    internal sealed class Handler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITenantGetterService tenantGetterService) : IScopedHandler
    {
        public async Task<Response> RegisterAsync(SelfRegisterRequest request)
        {
            CurrentTenantDetails currentTenant = tenantGetterService.Tenant;
            if (string.IsNullOrWhiteSpace(currentTenant.Id) || string.IsNullOrWhiteSpace(currentTenant.Identifier))
            {
                return Response.BadRequest("Tenant context is missing.");
            }

            if (string.IsNullOrWhiteSpace(request.TenantId) == false &&
                string.Equals(request.TenantId, currentTenant.Identifier, StringComparison.OrdinalIgnoreCase) == false)
            {
                return Response.BadRequest("Tenant context does not match the registration request.");
            }

            ApplicationUser? existingUser = await userManager.FindByEmailAsync(request.Email.Trim().Normalize()).ConfigureAwait(false);
            if (existingUser is not null)
            {
                return Response.BadRequest("Email is already registered.");
            }

            ApplicationUser? existingByUserName = await userManager.FindByNameAsync(request.UserName.Trim().Normalize()).ConfigureAwait(false);
            if (existingByUserName is not null)
            {
                return Response.BadRequest("Username is already taken.");
            }

            ApplicationUser user = new()
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = currentTenant.Id,
                UserName = request.UserName.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                EmailConfirmed = false,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow
            };

            IdentityResult createResult = await userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
            if (!createResult.Succeeded)
            {
                return Response.BadRequest(string.Join(" ", createResult.Errors.Select(e => e.Description)));
            }

            ApplicationRole? basicRole = await roleManager.FindByNameAsync(RoleConstants.Basic).ConfigureAwait(false);
            if (basicRole is null)
            {
                await userManager.DeleteAsync(user).ConfigureAwait(false);
                return Response.BadRequest("Default user role is not configured.");
            }

            IdentityResult addRoleResult = await userManager.AddToRoleAsync(user, basicRole.Name!).ConfigureAwait(false);
            if (!addRoleResult.Succeeded)
            {
                await userManager.DeleteAsync(user).ConfigureAwait(false);
                return Response.BadRequest(string.Join(" ", addRoleResult.Errors.Select(e => e.Description)));
            }

            return Response.Success("Registration successful.");
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder authGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tokens)
                .AddFluentValidationFilter();

            authGroup.MapPost("/self-register", async (
                    [FromBody] SelfRegisterRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response res = await handler.RegisterAsync(request).ConfigureAwait(false);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Response>();
        }
    }
}
