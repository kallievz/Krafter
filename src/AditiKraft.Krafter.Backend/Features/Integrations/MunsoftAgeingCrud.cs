using System.Linq.Dynamic.Core;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Integrations;

public sealed class MunsoftAgeingCrud
{
    internal sealed class Handler(MunsoftDbContext dbContext) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<MunsoftAgeingDto>>> GetAsync(GetRequestInput requestInput, CancellationToken cancellationToken)
        {
            IQueryable<MunsoftAgeingDto> query = dbContext.MunsoftAgeings
                .AsNoTracking()
                .Where(entity => entity.IsDeleted == requestInput.IsDeleted)
                .Select(entity => new MunsoftAgeingDto
                {
                    Id = entity.Id,
                    InstanceId = entity.InstanceId,
                    MDBCode = entity.MDBCode,
                    AccountNumber = entity.AccountNumber,
                    CurrentMonth = entity.CurrentMonth,
                    DueDate = entity.DueDate,
                    Days30 = entity.Days30,
                    Days60 = entity.Days60,
                    Days90 = entity.Days90,
                    Days120 = entity.Days120,
                    Days150 = entity.Days150,
                    Days180 = entity.Days180,
                    Days210 = entity.Days210,
                    Days240 = entity.Days240,
                    Days270 = entity.Days270,
                    Days300 = entity.Days300,
                    Days330 = entity.Days330,
                    Days360 = entity.Days360,
                    Days390 = entity.Days390,
                    TotalOutstanding = entity.TotalOutstanding,
                    IsProcessed = entity.IsProcessed,
                    IsUpdated = entity.IsUpdated,
                    DeleteReason = entity.DeleteReason,
                    IsDeleted = entity.IsDeleted,
                    CreatedOn = entity.CreatedOn,
                    CreatedById = entity.CreatedById,
                    UpdatedOn = entity.UpdatedOn,
                    UpdatedById = entity.UpdatedById
                });

            if (!string.IsNullOrWhiteSpace(requestInput.Filter))
            {
                query = query.Where(requestInput.Filter);
            }

            query = !string.IsNullOrWhiteSpace(requestInput.OrderBy)
                ? query.OrderBy(requestInput.OrderBy)
                : query.OrderBy(entity => entity.CreatedOn);

            List<MunsoftAgeingDto> items = await query.PageBy(requestInput).ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            return Response<PaginationResponse<MunsoftAgeingDto>>.Success(
                new PaginationResponse<MunsoftAgeingDto>(items, totalCount, requestInput.SkipCount, requestInput.MaxResultCount));
        }

        public async Task<Response<MunsoftAgeingDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftAgeingDto? entity = await dbContext.MunsoftAgeings
                .AsNoTracking()
                .Where(current => current.Id == id)
                .Select(current => new MunsoftAgeingDto
                {
                    Id = current.Id,
                    InstanceId = current.InstanceId,
                    MDBCode = current.MDBCode,
                    AccountNumber = current.AccountNumber,
                    CurrentMonth = current.CurrentMonth,
                    DueDate = current.DueDate,
                    Days30 = current.Days30,
                    Days60 = current.Days60,
                    Days90 = current.Days90,
                    Days120 = current.Days120,
                    Days150 = current.Days150,
                    Days180 = current.Days180,
                    Days210 = current.Days210,
                    Days240 = current.Days240,
                    Days270 = current.Days270,
                    Days300 = current.Days300,
                    Days330 = current.Days330,
                    Days360 = current.Days360,
                    Days390 = current.Days390,
                    TotalOutstanding = current.TotalOutstanding,
                    IsProcessed = current.IsProcessed,
                    IsUpdated = current.IsUpdated,
                    DeleteReason = current.DeleteReason,
                    IsDeleted = current.IsDeleted,
                    CreatedOn = current.CreatedOn,
                    CreatedById = current.CreatedById,
                    UpdatedOn = current.UpdatedOn,
                    UpdatedById = current.UpdatedById
                })
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return entity is null
                ? Response<MunsoftAgeingDto>.NotFound("MunsoftAgeing Not Found")
                : Response<MunsoftAgeingDto>.Success(entity);
        }

        public async Task<Response> CreateAsync(CreateOrUpdateMunsoftAgeingRequest request, CancellationToken cancellationToken)
        {
            MunsoftAgeing entity = new MunsoftAgeing();
            ApplyRequest(entity, request);
            entity.Id = Guid.CreateVersion7();

            await dbContext.MunsoftAgeings.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateAsync(Guid id, CreateOrUpdateMunsoftAgeingRequest request, CancellationToken cancellationToken)
        {
            MunsoftAgeing? entity = await dbContext.MunsoftAgeings
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftAgeing Not Found");
            }

            ApplyRequest(entity, request);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftAgeing? entity = await dbContext.MunsoftAgeings
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftAgeing Not Found");
            }

            dbContext.MunsoftAgeings.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateProcessingAsync(Guid id, UpdateMunsoftAgeingProcessingRequest request, CancellationToken cancellationToken)
        {
            MunsoftAgeing? entity = await dbContext.MunsoftAgeings
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftAgeing Not Found");
            }

            entity.IsProcessed = request.IsProcessed;
            entity.IsUpdated = request.IsUpdated;

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        private static void ApplyRequest(MunsoftAgeing entity, CreateOrUpdateMunsoftAgeingRequest request)
        {
            entity.InstanceId = request.InstanceId;
            entity.MDBCode = request.MDBCode;
            entity.AccountNumber = request.AccountNumber;
            entity.CurrentMonth = request.CurrentMonth;
            entity.DueDate = request.DueDate;
            entity.Days30 = request.Days30;
            entity.Days60 = request.Days60;
            entity.Days90 = request.Days90;
            entity.Days120 = request.Days120;
            entity.Days150 = request.Days150;
            entity.Days180 = request.Days180;
            entity.Days210 = request.Days210;
            entity.Days240 = request.Days240;
            entity.Days270 = request.Days270;
            entity.Days300 = request.Days300;
            entity.Days330 = request.Days330;
            entity.Days360 = request.Days360;
            entity.Days390 = request.Days390;
            entity.TotalOutstanding = request.TotalOutstanding;
            entity.IsProcessed = request.IsProcessed;
            entity.IsUpdated = request.IsUpdated;
            entity.DeleteReason = request.DeleteReason;
            entity.IsDeleted = request.IsDeleted;
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Integrations)
                .AddFluentValidationFilter();

            group.MapGet("/munsoft/ageings", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<MunsoftAgeingDto>> response = await handler.GetAsync(requestInput, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<MunsoftAgeingDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapGet("/munsoft/ageings/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<MunsoftAgeingDto> response = await handler.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<MunsoftAgeingDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapPost("/munsoft/ageings", async (
                    [FromBody] CreateOrUpdateMunsoftAgeingRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Integrations);

            group.MapPatch("/munsoft/ageings/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromBody] CreateOrUpdateMunsoftAgeingRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);

            group.MapDelete("/munsoft/ageings/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Integrations);

            group.MapPatch("/munsoft/ageings/{id:guid}/processing", async (
                    [FromRoute] Guid id,
                    [FromBody] UpdateMunsoftAgeingProcessingRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdateProcessingAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);
        }
    }
}
