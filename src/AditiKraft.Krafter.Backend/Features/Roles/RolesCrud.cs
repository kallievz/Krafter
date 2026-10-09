using System.Linq.Dynamic.Core;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Roles;
using LinqKit;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Roles;

public sealed class RolesCrud
{
    internal sealed class Handler(
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext db,
        RolePermissionService permissionService) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<RoleDto>>> GetListAsync(
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            ExpressionStarter<ApplicationRole>? predicate = PredicateBuilder.New<ApplicationRole>(true);

            if (!string.IsNullOrWhiteSpace(requestInput.Id))
            {
                predicate = predicate.And(role => role.Id == requestInput.Id);
            }

            IQueryable<RoleDto> query = db.Roles
                .Where(predicate)
                .Select(role => new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    CreatedById = role.CreatedById,
                    IsDeleted = role.IsDeleted,
                    CreatedOn = role.CreatedOn,
                    CreatedBy = role.CreatedBy != null
                        ? new UserInfo
                        {
                            Id = role.CreatedBy.Id,
                            FirstName = role.CreatedBy.FirstName,
                            LastName = role.CreatedBy.LastName,
                            CreatedOn = role.CreatedBy.CreatedOn
                        }
                        : null,
                    UpdatedBy = role.UpdatedBy != null
                        ? new UserInfo
                        {
                            Id = role.UpdatedBy.Id,
                            FirstName = role.UpdatedBy.FirstName,
                            LastName = role.UpdatedBy.LastName,
                            CreatedOn = role.UpdatedBy.CreatedOn
                        }
                        : null,
                    UpdatedById = role.UpdatedById,
                    UpdatedOn = role.UpdatedOn,
                    DeleteReason = role.DeleteReason
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
                    query = query.Where(role => (role.Name ?? string.Empty).ToLower().Contains(filter));
                }
            }

            if (!string.IsNullOrEmpty(requestInput.OrderBy))
            {
                query = query.OrderBy(requestInput.OrderBy);
            }

            List<RoleDto> items = await query
                .PageBy(requestInput)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            PaginationResponse<RoleDto> result = new PaginationResponse<RoleDto>(
                items,
                totalCount,
                requestInput.SkipCount,
                requestInput.MaxResultCount);

            return Response<PaginationResponse<RoleDto>>.Success(result);
        }

        public async Task<Response<RoleDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            ApplicationRole? role = await roleManager.Roles
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (role is null)
            {
                return Response<RoleDto>.NotFound("Role Not Found");
            }

            return Response<RoleDto>.Success(role.Adapt<RoleDto>());
        }

        public async Task<Response<RoleDto>> GetByIdWithPermissionsAsync(
            string roleId,
            CancellationToken cancellationToken)
        {
            ApplicationRole? role = await roleManager.Roles
                .SingleOrDefaultAsync(current => current.Id == roleId, cancellationToken)
                .ConfigureAwait(false);

            if (role is null)
            {
                return Response<RoleDto>.NotFound("Role Not Found");
            }

            RoleDto roleDto = role.Adapt<RoleDto>();

            roleDto.Permissions = await db.RoleClaims
                .Where(claim => claim.RoleId == roleId &&
                                claim.ClaimType == AppClaimTypes.Permission &&
                                claim.IsDeleted == false)
                .Select(claim => claim.ClaimValue!)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<RoleDto>.Success(roleDto);
        }

        public async Task<Response> CreateAsync(CreateOrUpdateRoleRequest request, CancellationToken cancellationToken)
        {
            request.Id = null;

            if (request.Permissions is not null && request.Name == RoleConstants.Admin)
            {
                return Response.BadRequest("Not allowed to modify Permissions for this Role.");
            }

            ApplicationRole role = new ApplicationRole(request.Name, request.Description)
            {
                Id = Guid.CreateVersion7().ToString()
            };

            IdentityResult result = await roleManager.CreateAsync(role).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Response.BadRequest($"Register role failed {result.Errors}");
            }

            if (request.Permissions is not null)
            {
                await permissionService.SynchronizeAsync(role.Id, request.Permissions, cancellationToken)
                    .ConfigureAwait(false);
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> PatchAsync(
            string id,
            CreateOrUpdateRoleRequest request,
            CancellationToken cancellationToken)
        {
            request.Id = id;
            ApplicationRole? role = await roleManager.FindByIdAsync(id).ConfigureAwait(false);
            if (role is null)
            {
                return Response.NotFound("Role Not Found");
            }

            if (request.Permissions is not null && role.Name == RoleConstants.Admin)
            {
                return Response.BadRequest("Not allowed to modify Permissions for this Role.");
            }

            if (request.Name != role.Name)
            {
                if (RoleConstants.IsDefault(role.Name!))
                {
                    return Response.Forbidden($"Not allowed to modify {role.Name} Role.");
                }

                role.Name = request.Name;
                role.NormalizedName = request.Name?.ToUpperInvariant();
            }

            if (request.Description != role.Description)
            {
                if (RoleConstants.IsDefault(role.Name!))
                {
                    return Response.Forbidden($"Not allowed to modify {role.Name} Role.");
                }

                role.Description = request.Description;
            }

            IdentityResult result = await roleManager.UpdateAsync(role).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Response.BadRequest($"Update role failed {result.Errors}");
            }

            if (request.Permissions is not null)
            {
                await permissionService.SynchronizeAsync(role.Id, request.Permissions, cancellationToken)
                    .ConfigureAwait(false);
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> PatchPermissionsAsync(
            UpdateRolePermissionsRequest request,
            CancellationToken cancellationToken)
        {
            ApplicationRole? role = await roleManager.FindByIdAsync(request.RoleId).ConfigureAwait(false);
            if (role is null)
            {
                return Response.NotFound("Role Not Found");
            }

            if (role.Name == RoleConstants.Admin)
            {
                return Response.Forbidden("Not allowed to modify Permissions for this Role.");
            }

            await permissionService.SynchronizeAsync(role.Id, request.Permissions, cancellationToken)
                .ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Response.Success("Role permissions updated successfully");
        }

        public async Task<Response> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            ApplicationRole? role = await roleManager.FindByIdAsync(id).ConfigureAwait(false);
            if (role is null)
            {
                return Response.NotFound("Role Not Found");
            }

            if (RoleConstants.IsDefault(role.Name!))
            {
                return Response.Forbidden($"Not allowed to delete {role.Name} Role.");
            }

            role.IsDeleted = true;
            db.Roles.Update(role);

            List<ApplicationRoleClaim> roleClaims = await db.RoleClaims
                .Where(claim => claim.RoleId == id && claim.ClaimType == AppClaimTypes.Permission)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (ApplicationRoleClaim roleClaim in roleClaims)
            {
                roleClaim.IsDeleted = true;
            }

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder roleGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Roles)
                .AddFluentValidationFilter();

            roleGroup.MapGet("/", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<RoleDto>> response = await handler
                        .GetListAsync(requestInput, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<RoleDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Roles);

            roleGroup.MapGet($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<RoleDto> response = await handler.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<RoleDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Roles);

            roleGroup.MapGet($"/{RouteSegment.RolePermissions}", async (
                    [FromRoute] string roleId,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<RoleDto> response = await handler
                        .GetByIdWithPermissionsAsync(roleId, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<RoleDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Roles);

            roleGroup.MapPost("/", async (
                    [FromBody] CreateOrUpdateRoleRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Roles);

            roleGroup.MapPatch($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromBody] CreateOrUpdateRoleRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.PatchAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Roles);

            roleGroup.MapPatch("/permissions", async (
                    [FromBody] UpdateRolePermissionsRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.PatchPermissionsAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Roles);

            roleGroup.MapDelete($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Roles);
        }
    }
}
