using System.Diagnostics;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Contracts.Common;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Web.Middleware;

public sealed class EndpointTimingMiddleware(
    ILogger<EndpointTimingMiddleware> logger,
    IOptions<EndpointTimingOptions> options,
    ICurrentUser currentUser,
    ITenantGetterService tenantGetterService) : IMiddleware
{
    private readonly EndpointTimingOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_options.EnableTiming == false)
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        if (_options.ApiOnly && !context.Request.Path.StartsWithSegments($"/{ApiRoutes.ApiPrefix}"))
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            await next(context).ConfigureAwait(false);
        }
        finally
        {
            double elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            string endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? string.Empty;
            string requestId = context.TraceIdentifier;
            int statusCode = context.Response.StatusCode;
            string userId = currentUser.GetUserId();
            string tenantId = tenantGetterService.Tenant.Id ?? string.Empty;
            string tenantName = tenantGetterService.Tenant.Name ?? string.Empty;

            if (elapsedMs >= _options.SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "Slow endpoint detected ({ElapsedMs:F2} ms). Method={Method}, Path={Path}, Endpoint={Endpoint}, StatusCode={StatusCode}, RequestId={RequestId}, UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}",
                    elapsedMs,
                    context.Request.Method,
                    context.Request.Path,
                    endpoint,
                    statusCode,
                    requestId,
                    userId,
                    tenantId,
                    tenantName);
            }
            else
            {
                logger.LogDebug(
                    "Endpoint timing ({ElapsedMs:F2} ms). Method={Method}, Path={Path}, Endpoint={Endpoint}, StatusCode={StatusCode}, RequestId={RequestId}, UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}",
                    elapsedMs,
                    context.Request.Method,
                    context.Request.Path,
                    endpoint,
                    statusCode,
                    requestId,
                    userId,
                    tenantId,
                    tenantName);
            }
        }
    }
}
