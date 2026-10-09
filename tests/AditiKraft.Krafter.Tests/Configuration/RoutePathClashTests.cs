using AditiKraft.Krafter.Backend.Web.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;

namespace AditiKraft.Krafter.Tests.Configuration;

public sealed class RoutePathClashTests
{
    [Fact]
    public void WhenRoutesAreDiscoveredThenNoHttpMethodPathClashesExist()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddRouteDiscovery();

        using WebApplication app = builder.Build();
        app.MapDiscoveredRoutes();

        EndpointDataSource endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
        List<RouteEndpoint> routeEndpoints = endpointDataSource.Endpoints
            .OfType<RouteEndpoint>()
            .ToList();

        IEnumerable<RouteEndpointSignature> signatures = routeEndpoints
            .SelectMany(CreateRouteSignatures);

        List<IGrouping<RouteEndpointSignature, RouteEndpointSignature>> duplicates = signatures
            .GroupBy(signature => signature)
            .Where(group => group.Count() > 1)
            .ToList();

        string details = string.Join(
            Environment.NewLine,
            duplicates.Select(group => $"{group.Key.HttpMethod} {group.Key.RoutePattern}: {group.Count()} registrations"));

        Assert.True(
            duplicates.Count == 0,
            string.IsNullOrWhiteSpace(details)
                ? "Duplicate route registrations were detected."
                : $"Duplicate route registrations were detected:{Environment.NewLine}{details}");
    }

    private static IEnumerable<RouteEndpointSignature> CreateRouteSignatures(RouteEndpoint endpoint)
    {
        HttpMethodMetadata? httpMethodMetadata = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault();
        string routePattern = NormalizeRoutePattern(endpoint.RoutePattern);

        if (httpMethodMetadata is null || httpMethodMetadata.HttpMethods.Count == 0)
        {
            return [new RouteEndpointSignature("ANY", routePattern)];
        }

        return httpMethodMetadata.HttpMethods
            .Select(httpMethod => new RouteEndpointSignature(httpMethod.ToUpperInvariant(), routePattern));
    }

    private static string NormalizeRoutePattern(RoutePattern routePattern)
    {
        string rawText = routePattern.RawText ?? string.Empty;
        return rawText.Trim().ToLowerInvariant();
    }

    private sealed record RouteEndpointSignature(string HttpMethod, string RoutePattern);
}
