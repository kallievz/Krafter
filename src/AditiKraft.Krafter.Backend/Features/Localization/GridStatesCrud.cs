using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Localization.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Grids;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Localization;

public sealed class GridStatesCrud
{
    internal sealed class Handler(
        LocalizationDbContext localizationDbContext,
        ICurrentUser currentUser,
        ITenantGetterService tenantGetterService) : IScopedHandler
    {
        public async Task<Response<List<GridStateDto>>> GetGridStatesAsync(string componentKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(componentKey))
            {
                return Response<List<GridStateDto>>.BadRequest("Component key is required.");
            }

            string? userId = currentUser.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response<List<GridStateDto>>.Unauthorized("User not authenticated.");
            }

            string tenantId = GetTenantIdOrEmpty();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return Response<List<GridStateDto>>.BadRequest("Tenant is not resolved.");
            }

            List<GridStateDto> states = await localizationDbContext.GridStates.AsNoTracking()
                .Where(state => state.TenantId == tenantId && state.UserId == userId && state.ComponentKey == componentKey)
                .OrderByDescending(state => state.IsDefault)
                .ThenBy(state => state.Name)
                .Select(state => new GridStateDto
                {
                    Id = state.Id,
                    ComponentKey = state.ComponentKey,
                    Name = state.Name,
                    StateJson = state.StateJson,
                    IsDefault = state.IsDefault,
                    CreatedOn = state.CreatedOn,
                    UpdatedOn = state.UpdatedOn
                })
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return Response<List<GridStateDto>>.Success(states);
        }

        public async Task<Response<GridStateDto>> CreateGridStateAsync(CreateGridStateRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ComponentKey))
            {
                return Response<GridStateDto>.BadRequest("Component key is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Response<GridStateDto>.BadRequest("State name is required.");
            }

            string? userId = currentUser.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response<GridStateDto>.Unauthorized("User not authenticated.");
            }

            string tenantId = GetTenantIdOrEmpty();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return Response<GridStateDto>.BadRequest("Tenant is not resolved.");
            }

            string trimmedName = request.Name.Trim();

            int existingCount = await localizationDbContext.GridStates.AsNoTracking()
                .CountAsync(state => state.TenantId == tenantId && state.UserId == userId &&
                                     state.ComponentKey == request.ComponentKey && state.Name == trimmedName,
                    cancellationToken).ConfigureAwait(false);

            if (existingCount > 0)
            {
                return Response<GridStateDto>.BadRequest("A grid state with the same name already exists.");
            }

            if (request.IsDefault)
            {
                List<GridState> defaults = await localizationDbContext.GridStates
                    .Where(state => state.TenantId == tenantId && state.UserId == userId &&
                                    state.ComponentKey == request.ComponentKey && state.IsDefault)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                foreach (GridState gridState in defaults)
                {
                    gridState.IsDefault = false;
                    gridState.UpdatedOn = DateTime.UtcNow;
                }
            }

            GridState newState = new GridState
            {
                Id = Guid.CreateVersion7().ToString(),
                TenantId = tenantId,
                UserId = userId,
                ComponentKey = request.ComponentKey,
                Name = trimmedName,
                StateJson = request.StateJson ?? string.Empty,
                IsDefault = request.IsDefault
            };

            await localizationDbContext.GridStates.AddAsync(newState, cancellationToken).ConfigureAwait(false);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            GridStateDto dto = new GridStateDto
            {
                Id = newState.Id,
                ComponentKey = newState.ComponentKey,
                Name = newState.Name,
                StateJson = newState.StateJson,
                IsDefault = newState.IsDefault,
                CreatedOn = newState.CreatedOn,
                UpdatedOn = newState.UpdatedOn
            };

            return Response<GridStateDto>.Success(dto);
        }

        public async Task<Response<GridStateDto>> UpdateGridStateAsync(string id, UpdateGridStateRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response<GridStateDto>.BadRequest("State id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Response<GridStateDto>.BadRequest("State name is required.");
            }

            string? userId = currentUser.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response<GridStateDto>.Unauthorized("User not authenticated.");
            }

            string tenantId = GetTenantIdOrEmpty();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return Response<GridStateDto>.BadRequest("Tenant is not resolved.");
            }

            GridState? state = await localizationDbContext.GridStates
                .FirstOrDefaultAsync(gridState => gridState.Id == id && gridState.TenantId == tenantId &&
                                              gridState.UserId == userId, cancellationToken)
                .ConfigureAwait(false);

            if (state is null)
            {
                return Response<GridStateDto>.NotFound("Grid state not found.");
            }

            string trimmedName = request.Name.Trim();

            int duplicateCount = await localizationDbContext.GridStates.AsNoTracking()
                .CountAsync(gridState => gridState.Id != id && gridState.TenantId == tenantId &&
                                         gridState.UserId == userId &&
                                         gridState.ComponentKey == state.ComponentKey &&
                                         gridState.Name == trimmedName,
                    cancellationToken).ConfigureAwait(false);

            if (duplicateCount > 0)
            {
                return Response<GridStateDto>.BadRequest("A grid state with the same name already exists.");
            }

            if (request.IsDefault)
            {
                List<GridState> defaults = await localizationDbContext.GridStates
                    .Where(gridState => gridState.Id != id && gridState.TenantId == tenantId &&
                                        gridState.UserId == userId &&
                                        gridState.ComponentKey == state.ComponentKey && gridState.IsDefault)
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                foreach (GridState defaultState in defaults)
                {
                    defaultState.IsDefault = false;
                    defaultState.UpdatedOn = DateTime.UtcNow;
                }
            }

            state.Name = trimmedName;
            state.StateJson = request.StateJson ?? string.Empty;
            state.IsDefault = request.IsDefault;
            state.UpdatedOn = DateTime.UtcNow;

            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            GridStateDto dto = new GridStateDto
            {
                Id = state.Id,
                ComponentKey = state.ComponentKey,
                Name = state.Name,
                StateJson = state.StateJson,
                IsDefault = state.IsDefault,
                CreatedOn = state.CreatedOn,
                UpdatedOn = state.UpdatedOn
            };

            return Response<GridStateDto>.Success(dto);
        }

        public async Task<Response> DeleteGridStateAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.BadRequest("State id is required.");
            }

            string? userId = currentUser.GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Response.Unauthorized("User not authenticated.");
            }

            string tenantId = GetTenantIdOrEmpty();
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                return Response.BadRequest("Tenant is not resolved.");
            }

            GridState? state = await localizationDbContext.GridStates
                .FirstOrDefaultAsync(gridState => gridState.Id == id && gridState.TenantId == tenantId &&
                                              gridState.UserId == userId, cancellationToken)
                .ConfigureAwait(false);

            if (state is null)
            {
                return Response.NotFound("Grid state not found.");
            }

            localizationDbContext.GridStates.Remove(state);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return Response.Success();
        }

        private string GetTenantIdOrEmpty()
        {
            return tenantGetterService.Tenant.Id ?? string.Empty;
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            group.MapGet("/grid-states", async (
                    [FromQuery] string componentKey,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<GridStateDto>> response = await handler.GetGridStatesAsync(componentKey, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<GridStateDto>>>()
                .RequireAuthorization();

            group.MapPost("/grid-states", async (
                    [FromBody] CreateGridStateRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<GridStateDto> response = await handler.CreateGridStateAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<GridStateDto>>()
                .RequireAuthorization();

            group.MapPut("/grid-states/{id}", async (
                    [FromRoute] string id,
                    [FromBody] UpdateGridStateRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<GridStateDto> response = await handler.UpdateGridStateAsync(id, request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<GridStateDto>>()
                .RequireAuthorization();

            group.MapDelete("/grid-states/{id}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteGridStateAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .RequireAuthorization();
        }
    }
}
