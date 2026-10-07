using System.Diagnostics;
using AditiKraft.Krafter.Contracts.Common;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Web.Middleware;

public sealed class EndpointTimingMiddleware(
    ILogger<EndpointTimingMiddleware> logger,
    IOptions<EndpointTimingOptions> options) : IMiddleware
{
    private readonly EndpointTimingOptions _options = options.Value;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (_options.EnableTiming == false)
        {
            await next(context);
            return;
        }

        if (_options.ApiOnly && !context.Request.Path.StartsWithSegments($"/{ApiRoutes.ApiPrefix}"))
        {
            await next(context);
            return;
        }

        long startTimestamp = Stopwatch.GetTimestamp();

        try
        {
            await next(context);
        }
        finally
        {
            double elapsedMs = Stopwatch.GetElapsedTime(startTimestamp).TotalMilliseconds;
            string endpoint = context.GetEndpoint()?.DisplayName ?? context.Request.Path.Value ?? string.Empty;
            string requestId = context.TraceIdentifier;
            int statusCode = context.Response.StatusCode;

            if (elapsedMs >= _options.SlowRequestThresholdMs)
            {
                logger.LogWarning(
                    "Slow endpoint detected ({ElapsedMs:F2} ms). Method={Method}, Path={Path}, Endpoint={Endpoint}, StatusCode={StatusCode}, RequestId={RequestId}",
                    elapsedMs,
                    context.Request.Method,
                    context.Request.Path,
                    endpoint,
                    statusCode,
                    requestId);
            }
            else
            {
                logger.LogDebug(
                    "Endpoint timing ({ElapsedMs:F2} ms). Method={Method}, Path={Path}, Endpoint={Endpoint}, StatusCode={StatusCode}, RequestId={RequestId}",
                    elapsedMs,
                    context.Request.Method,
                    context.Request.Path,
                    endpoint,
                    statusCode,
                    requestId);
            }
        }
    }
}
