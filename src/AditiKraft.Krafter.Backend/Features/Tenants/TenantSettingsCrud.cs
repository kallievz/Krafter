using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Tenants;

public sealed class TenantSettingsCrud
{
    internal sealed class Handler(TenantDbContext dbContext) : IScopedHandler
    {
        public async Task<Response<List<TenantSettingDto>>> GetAsync(string tenantId, CancellationToken cancellationToken)
        {
            bool tenantExists = await dbContext.Tenants.AsNoTracking()
                .AnyAsync(tenant => tenant.Id == tenantId, cancellationToken)
                .ConfigureAwait(false);
            if (!tenantExists)
            {
                return Response<List<TenantSettingDto>>.NotFound("Tenant not found.");
            }

            List<TenantSettingDto> data = await dbContext.TenantSettings.AsNoTracking()
                .Where(setting => setting.TenantId == tenantId)
                .OrderBy(setting => setting.Key)
                .Select(setting => new TenantSettingDto
                {
                    Id = setting.Id,
                    TenantId = setting.TenantId,
                    Key = setting.Key,
                    Value = setting.Value,
                    IsSensitive = setting.IsSensitive
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<TenantSettingDto>>.Success(data);
        }

        public async Task<Response> PatchAsync(
            string tenantId,
            UpsertTenantSettingRequest request,
            CancellationToken cancellationToken)
        {
            bool tenantExists = await dbContext.Tenants.AsNoTracking()
                .AnyAsync(tenant => tenant.Id == tenantId, cancellationToken)
                .ConfigureAwait(false);
            if (!tenantExists)
            {
                return Response.NotFound("Tenant not found.");
            }

            string key = request.Key.Trim();
            TenantSetting? entity;

            if (!string.IsNullOrWhiteSpace(request.Id))
            {
                entity = await dbContext.TenantSettings
                    .FirstOrDefaultAsync(setting => setting.Id == request.Id && setting.TenantId == tenantId, cancellationToken)
                    .ConfigureAwait(false);

                if (entity is null)
                {
                    return Response.NotFound("Tenant setting not found.");
                }
            }
            else
            {
                entity = await dbContext.TenantSettings
                    .FirstOrDefaultAsync(setting => setting.TenantId == tenantId && setting.Key == key, cancellationToken)
                    .ConfigureAwait(false);
            }

            if (entity is null)
            {
                entity = new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = key,
                    Value = request.Value ?? string.Empty,
                    IsSensitive = request.IsSensitive
                };
                dbContext.TenantSettings.Add(entity);
            }
            else
            {
                entity.Key = key;
                entity.Value = request.Value ?? string.Empty;
                entity.IsSensitive = request.IsSensitive;
            }

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder tenantGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tenants)
                .AddFluentValidationFilter();

            tenantGroup.MapGet($"/{RouteSegment.ById}/settings", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<TenantSettingDto>> response = await handler.GetAsync(id, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<TenantSettingDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Tenants);

            tenantGroup.MapPatch($"/{RouteSegment.ById}/settings", async (
                    [FromRoute] string id,
                    [FromBody] UpsertTenantSettingRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.PatchAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Tenants);
        }
    }
}
