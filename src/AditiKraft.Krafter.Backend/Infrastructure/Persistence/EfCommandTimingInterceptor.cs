using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public sealed class EfCommandTimingInterceptor(
    ILogger<EfCommandTimingInterceptor> logger,
    IHttpContextAccessor httpContextAccessor,
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

        string? requestId = httpContextAccessor.HttpContext?.TraceIdentifier;
        string? path = httpContextAccessor.HttpContext?.Request.Path.Value;

        if (elapsedMs >= _options.SlowQueryThresholdMs)
        {
            logger.LogWarning(
                "EF slow query detected ({ElapsedMs:F2} ms). CommandType={CommandType}, DataSource={DataSource}, Database={Database}, RequestId={RequestId}, Path={Path}, Sql={Sql}",
                elapsedMs,
                command.CommandType,
                command.Connection?.DataSource,
                command.Connection?.Database,
                requestId,
                path,
                command.CommandText);
            return;
        }

        logger.LogDebug(
            "EF query ({ElapsedMs:F2} ms). CommandType={CommandType}, DataSource={DataSource}, Database={Database}, RequestId={RequestId}, Path={Path}",
            elapsedMs,
            command.CommandType,
            command.Connection?.DataSource,
            command.Connection?.Database,
            requestId,
            path);
    }
}
