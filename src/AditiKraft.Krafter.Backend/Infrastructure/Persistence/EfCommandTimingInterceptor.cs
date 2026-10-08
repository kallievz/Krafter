using System.Data.Common;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Telemetry;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public sealed class EfCommandTimingInterceptor(
    ILogger<EfCommandTimingInterceptor> logger,
    IHttpContextAccessor httpContextAccessor,
    ICurrentUser currentUser,
    ITenantGetterService tenantGetterService,
    IOptions<EfQueryPerformanceOptions> options) : DbCommandInterceptor
{
    private readonly EfQueryPerformanceOptions _options = options.Value;

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        LogCommand(command, eventData);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void LogCommand(DbCommand command, CommandExecutedEventData eventData)
    {
        if (_options.EnableCommandTiming == false)
        {
            return;
        }

        double elapsedMs = eventData.Duration.TotalMilliseconds;
        BackendTelemetry.EfCommandDuration.Record(elapsedMs);

        HttpContext? httpContext = httpContextAccessor.HttpContext;
        string? requestId = httpContext?.TraceIdentifier;
        string? path = httpContext?.Request.Path.Value;
        string? userId = currentUser.GetUserId();
        string? tenantId = tenantGetterService.Tenant?.Id;
        string? tenantName = tenantGetterService.Tenant?.Name;

        if (elapsedMs >= _options.SlowQueryThresholdMs)
        {
            BackendTelemetry.EfSlowCommandCount.Add(1);
            logger.LogWarning(
                "EF slow query detected ({ElapsedMs:F2} ms). CommandType={CommandType}, DataSource={DataSource}, Database={Database}, RequestId={RequestId}, Path={Path}, UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}, Sql={Sql}",
                elapsedMs,
                command.CommandType,
                command.Connection?.DataSource,
                command.Connection?.Database,
                requestId,
                path,
                userId,
                tenantId,
                tenantName,
                command.CommandText);
            return;
        }

        logger.LogDebug(
            "EF query ({ElapsedMs:F2} ms). CommandType={CommandType}, DataSource={DataSource}, Database={Database}, RequestId={RequestId}, Path={Path}, UserId={UserId}, TenantId={TenantId}, TenantName={TenantName}",
            elapsedMs,
            command.CommandType,
            command.Connection?.DataSource,
            command.Connection?.Database,
            requestId,
            path,
            userId,
            tenantId,
            tenantName);
    }
}
