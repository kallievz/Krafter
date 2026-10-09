using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.Tenants;

public sealed class TenantSeedDataCrud
{
    internal sealed class Handler(DataSeedService tenantSeedService) : IScopedHandler
    {
        public Task<Response> SeedAsync(SeedDataRequest request)
        {
            return tenantSeedService.SeedBasicDataAsync(request);
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder tenantGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tenants)
                .AddFluentValidationFilter();

            tenantGroup.MapPost($"/{RouteSegment.SeedData}", async (
                    [FromBody] SeedDataRequest request,
                    [FromServices] Handler handler) =>
                {
                    Response response = await handler.SeedAsync(request).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>();
        }
    }
}
