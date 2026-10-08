using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Integrations;

public sealed class GetMunsoftInstances
{
    internal sealed class Handler(ApplicationDbContext applicationDbContext) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<MunsoftInstanceDto>>> GetAsync(
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            IQueryable<MunsoftInstanceDto> query = applicationDbContext.MunsoftInstances
                .AsNoTracking()
                .Where(instance => !instance.IsDeleted)
                .Select(instance => new MunsoftInstanceDto
                {
                    Id = instance.Id,
                    Name = instance.Name,
                    MunicipalityName = instance.MunicipalityName,
                    DemarcationCode = instance.DemarcationCode
                });

            if (!string.IsNullOrWhiteSpace(requestInput.Filter))
            {
                query = query.Where(requestInput.Filter);
            }

            if (!string.IsNullOrWhiteSpace(requestInput.OrderBy))
            {
                try
                {
                    query = query.OrderBy(requestInput.OrderBy);
                }
                catch (ParseException)
                {
                    query = query.OrderBy(instance => instance.Name);
                }
            }
            else
            {
                query = query.OrderBy(instance => instance.Name);
            }

            List<MunsoftInstanceDto> items = await query.PageBy(requestInput)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            return Response<PaginationResponse<MunsoftInstanceDto>>.Success(
                new PaginationResponse<MunsoftInstanceDto>(items, totalCount, requestInput.SkipCount, requestInput.MaxResultCount));
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder routeGroupBuilder = endpointRouteBuilder.MapGroup(ApiRoutes.Integrations).AddFluentValidationFilter();
            routeGroupBuilder.MapGet("/munsoft/instances", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<MunsoftInstanceDto>> result = await handler.GetAsync(requestInput, cancellationToken).ConfigureAwait(false);
                    return Results.Json(result, statusCode: result.StatusCode);
                })
                .Produces<Response<PaginationResponse<MunsoftInstanceDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Tenants);
        }
    }
}
