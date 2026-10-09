using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AditiKraft.Krafter.Tests.Authorization;

public sealed class EndpointPermissionCoverageTests
{
    [Fact]
    public void EveryGatedEndpointHasPermissionPolicyOrExplicitAuthOnlyExemption()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddRouteDiscovery();

        using WebApplication app = builder.Build();
        app.MapDiscoveredRoutes();

        EndpointDataSource endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();
        List<RouteEndpoint> routeEndpoints = endpointDataSource.Endpoints.OfType<RouteEndpoint>().ToList();

        List<string> missingPermissionPolicy = routeEndpoints
            .Where(IsGatedEndpoint)
            .SelectMany(GetEndpointMethodSignaturesWithoutPermissionPolicy)
            .ToList();

        string details = string.Join(Environment.NewLine, missingPermissionPolicy);

        Assert.True(
            missingPermissionPolicy.Count == 0,
            string.IsNullOrWhiteSpace(details)
                ? "One or more gated endpoints are missing permission policy wiring."
                : $"One or more gated endpoints are missing permission policy wiring:{Environment.NewLine}{details}");
    }

    private static bool IsGatedEndpoint(RouteEndpoint endpoint)
    {
        IReadOnlyList<IAllowAnonymous> anonymousMetadata = endpoint.Metadata.OfType<IAllowAnonymous>().ToList();
        if (anonymousMetadata.Count > 0)
        {
            return false;
        }

        IReadOnlyList<IAuthorizeData> authorizeMetadata = endpoint.Metadata.OfType<IAuthorizeData>().ToList();
        return authorizeMetadata.Count > 0;
    }

    private static IEnumerable<string> GetEndpointMethodSignaturesWithoutPermissionPolicy(RouteEndpoint endpoint)
    {
        IReadOnlyList<IAuthorizeData> authorizeMetadata = endpoint.Metadata.OfType<IAuthorizeData>().ToList();
        bool hasPermissionPolicy = authorizeMetadata
            .Where(metadata => !string.IsNullOrWhiteSpace(metadata.Policy))
            .Select(metadata => metadata.Policy!)
            .Any(policy => policy.StartsWith(AppClaimTypes.Permission, StringComparison.OrdinalIgnoreCase));

        string route = NormalizeRoute(endpoint.RoutePattern.RawText);
        IReadOnlyList<string> methods = endpoint.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods?.ToList()
            ?? ["ANY"];

        return methods
            .Select(method => EndpointSignature.Create(method, route))
            .Where(signature => !hasPermissionPolicy && !_authOnlyExemptions.Contains(signature))
            .Select(signature => $"{signature.HttpMethod} {signature.Route}");
    }

    private static string NormalizeRoute(string? routePatternRawText)
    {
        return (routePatternRawText ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static readonly HashSet<EndpointSignature> _authOnlyExemptions =
    [
        EndpointSignature.Create("GET", "/api/users/permissions"),
        EndpointSignature.Create("POST", "/api/users/change-password"),
        EndpointSignature.Create("POST", "/api/users/profile/language"),
        EndpointSignature.Create("POST", "/api/users/localization/values/resolve"),
        EndpointSignature.Create("POST", "/api/users/localization/values/resolve/batch"),
        EndpointSignature.Create("GET", "/api/users/grid-states"),
        EndpointSignature.Create("POST", "/api/users/grid-states"),
        EndpointSignature.Create("PUT", "/api/users/grid-states/{id}"),
        EndpointSignature.Create("DELETE", "/api/users/grid-states/{id}")
    ];

    private sealed record EndpointSignature(string HttpMethod, string Route)
    {
        public static EndpointSignature Create(string httpMethod, string route)
        {
            return new EndpointSignature(httpMethod.Trim().ToUpperInvariant(), route.Trim().ToLowerInvariant());
        }
    }
}
