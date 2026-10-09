using System.Runtime.InteropServices;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.AppInfo;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.AppInfo;

public sealed class AppInfoCrud
{
    internal sealed class Handler : IScopedHandler
    {
        public Task<Response<string>> GetAppInfoAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string message =
                $"AditiKraft.Krafter.Backend version {BuildInfo.Build}, built on {BuildInfo.DateTimeUtc}, running on {RuntimeInformation.FrameworkDescription}";

            return Task.FromResult(Response<string>.Success(message));
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder routeGroupBuilder = endpointRouteBuilder.MapGroup(ApiRoutes.AppInfo)
                .AddFluentValidationFilter();

            routeGroupBuilder.MapGet("/", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<string> response = await handler.GetAppInfoAsync(cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<string>>();
        }
    }
}
