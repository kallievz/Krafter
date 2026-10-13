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

public sealed class MunsoftIndigentCrud
{
    internal sealed class Handler(MunsoftDbContext dbContext) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<MunsoftIndigentDto>>> GetAsync(GetRequestInput requestInput, CancellationToken cancellationToken)
        {
            IQueryable<MunsoftIndigentDto> query = dbContext.MunsoftIndigents
                .AsNoTracking()
                .Where(entity => entity.IsDeleted == requestInput.IsDeleted)
                .Select(entity => new MunsoftIndigentDto
                {
                    Id = entity.Id,
                    InstanceId = entity.InstanceId,
                    MDBCode = entity.MDBCode,
                    Touched = entity.Touched,
                    AccountNumber = entity.AccountNumber,
                    AccountHolderName = entity.AccountHolderName,
                    ErfExtension = entity.ErfExtension,
                    ErfLotNumber = entity.ErfLotNumber,
                    ErfSubDivision = entity.ErfSubDivision,
                    ErfUnitNumber = entity.ErfUnitNumber,
                    StreetAddress = entity.StreetAddress,
                    IndigentSequenceNumber = entity.IndigentSequenceNumber,
                    ApplicationDate = entity.ApplicationDate,
                    Approved = entity.Approved,
                    Active = entity.Active,
                    StartDate = entity.StartDate,
                    ExpiryDate = entity.ExpiryDate,
                    InactivateDate = entity.InactivateDate,
                    InactivateReason = entity.InactivateReason,
                    IndigentName = entity.IndigentName,
                    IndigentIdNumber = entity.IndigentIdNumber,
                    NumberOfOccupants = entity.NumberOfOccupants,
                    IndigentIncome = entity.IndigentIncome,
                    TotalIncome = entity.TotalIncome,
                    IndigentPercentCode = entity.IndigentPercentCode,
                    IndigentPercentDescription = entity.IndigentPercentDescription,
                    ReferenceSystem = entity.ReferenceSystem,
                    ReferenceTransactionId = entity.ReferenceTransactionId,
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

            List<MunsoftIndigentDto> items = await query.PageBy(requestInput).ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            return Response<PaginationResponse<MunsoftIndigentDto>>.Success(
                new PaginationResponse<MunsoftIndigentDto>(items, totalCount, requestInput.SkipCount, requestInput.MaxResultCount));
        }

        public async Task<Response<MunsoftIndigentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftIndigentDto? entity = await dbContext.MunsoftIndigents
                .AsNoTracking()
                .Where(current => current.Id == id)
                .Select(current => new MunsoftIndigentDto
                {
                    Id = current.Id,
                    InstanceId = current.InstanceId,
                    MDBCode = current.MDBCode,
                    Touched = current.Touched,
                    AccountNumber = current.AccountNumber,
                    AccountHolderName = current.AccountHolderName,
                    ErfExtension = current.ErfExtension,
                    ErfLotNumber = current.ErfLotNumber,
                    ErfSubDivision = current.ErfSubDivision,
                    ErfUnitNumber = current.ErfUnitNumber,
                    StreetAddress = current.StreetAddress,
                    IndigentSequenceNumber = current.IndigentSequenceNumber,
                    ApplicationDate = current.ApplicationDate,
                    Approved = current.Approved,
                    Active = current.Active,
                    StartDate = current.StartDate,
                    ExpiryDate = current.ExpiryDate,
                    InactivateDate = current.InactivateDate,
                    InactivateReason = current.InactivateReason,
                    IndigentName = current.IndigentName,
                    IndigentIdNumber = current.IndigentIdNumber,
                    NumberOfOccupants = current.NumberOfOccupants,
                    IndigentIncome = current.IndigentIncome,
                    TotalIncome = current.TotalIncome,
                    IndigentPercentCode = current.IndigentPercentCode,
                    IndigentPercentDescription = current.IndigentPercentDescription,
                    ReferenceSystem = current.ReferenceSystem,
                    ReferenceTransactionId = current.ReferenceTransactionId,
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
                ? Response<MunsoftIndigentDto>.NotFound("MunsoftIndigent Not Found")
                : Response<MunsoftIndigentDto>.Success(entity);
        }

        public async Task<Response> CreateAsync(CreateOrUpdateMunsoftIndigentRequest request, CancellationToken cancellationToken)
        {
            MunsoftIndigent entity = new MunsoftIndigent();
            ApplyRequest(entity, request);
            entity.Id = Guid.CreateVersion7();

            await dbContext.MunsoftIndigents.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateAsync(Guid id, CreateOrUpdateMunsoftIndigentRequest request, CancellationToken cancellationToken)
        {
            MunsoftIndigent? entity = await dbContext.MunsoftIndigents
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftIndigent Not Found");
            }

            ApplyRequest(entity, request);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftIndigent? entity = await dbContext.MunsoftIndigents
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftIndigent Not Found");
            }

            dbContext.MunsoftIndigents.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateProcessingAsync(Guid id, UpdateMunsoftIndigentProcessingRequest request, CancellationToken cancellationToken)
        {
            MunsoftIndigent? entity = await dbContext.MunsoftIndigents
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftIndigent Not Found");
            }

            entity.IsProcessed = request.IsProcessed;
            entity.IsUpdated = request.IsUpdated;

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        private static void ApplyRequest(MunsoftIndigent entity, CreateOrUpdateMunsoftIndigentRequest request)
        {
            entity.InstanceId = request.InstanceId;
            entity.MDBCode = request.MDBCode;
            entity.Touched = request.Touched;
            entity.AccountNumber = request.AccountNumber;
            entity.AccountHolderName = request.AccountHolderName;
            entity.ErfExtension = request.ErfExtension;
            entity.ErfLotNumber = request.ErfLotNumber;
            entity.ErfSubDivision = request.ErfSubDivision;
            entity.ErfUnitNumber = request.ErfUnitNumber;
            entity.StreetAddress = request.StreetAddress;
            entity.IndigentSequenceNumber = request.IndigentSequenceNumber;
            entity.ApplicationDate = request.ApplicationDate;
            entity.Approved = request.Approved;
            entity.Active = request.Active;
            entity.StartDate = request.StartDate;
            entity.ExpiryDate = request.ExpiryDate;
            entity.InactivateDate = request.InactivateDate;
            entity.InactivateReason = request.InactivateReason;
            entity.IndigentName = request.IndigentName;
            entity.IndigentIdNumber = request.IndigentIdNumber;
            entity.NumberOfOccupants = request.NumberOfOccupants;
            entity.IndigentIncome = request.IndigentIncome;
            entity.TotalIncome = request.TotalIncome;
            entity.IndigentPercentCode = request.IndigentPercentCode;
            entity.IndigentPercentDescription = request.IndigentPercentDescription;
            entity.ReferenceSystem = request.ReferenceSystem;
            entity.ReferenceTransactionId = request.ReferenceTransactionId;
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

            group.MapGet("/munsoft/indigents", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<MunsoftIndigentDto>> response = await handler.GetAsync(requestInput, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<MunsoftIndigentDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapGet("/munsoft/indigents/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<MunsoftIndigentDto> response = await handler.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<MunsoftIndigentDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapPost("/munsoft/indigents", async (
                    [FromBody] CreateOrUpdateMunsoftIndigentRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Integrations);

            group.MapPatch("/munsoft/indigents/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromBody] CreateOrUpdateMunsoftIndigentRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);

            group.MapDelete("/munsoft/indigents/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Integrations);

            group.MapPatch("/munsoft/indigents/{id:guid}/processing", async (
                    [FromRoute] Guid id,
                    [FromBody] UpdateMunsoftIndigentProcessingRequest request,
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
