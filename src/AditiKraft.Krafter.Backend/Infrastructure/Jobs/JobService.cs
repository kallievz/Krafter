using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Telemetry;
using TickerQ.Utilities;
using TickerQ.Utilities.Entities;
using TickerQ.Utilities.Interfaces.Managers;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

/// <summary>
/// JobService provides a facade for enqueueing background jobs through TickerQ.
/// Jobs are defined in the <see cref="Jobs"/> class with <see cref="TickerQ.Utilities.Base.TickerFunctionAttribute"/>.
/// </summary>
public class JobService(ITimeTickerManager<TimeTickerEntity> timeTickerManager) : IJobService
{
    public async Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            _ = await timeTickerManager.AddAsync(new TimeTickerEntity
            {
                Function = methodName,
                Request = TickerHelper.CreateTickerRequest(requestInput),
                ExecutionTime = DateTime.UtcNow
            }, cancellationToken).ConfigureAwait(false);

            BackendTelemetry.TickerQEnqueueCount.Add(1, new KeyValuePair<string, object?>("method", methodName));
        }
        catch (Exception ex)
        {
            BackendTelemetry.TickerQEnqueueFailureCount.Add(1, new KeyValuePair<string, object?>("method", methodName));
            throw new InvalidOperationException($"Failed to enqueue TickerQ job '{methodName}'.", ex);
        }
    }

    public async Task EnqueueByLogIdAsync(string emailLogId, string methodName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(emailLogId))
        {
            throw new ArgumentException("Email log id is required.", nameof(emailLogId));
        }

        await EnqueueAsync(new SendEmailLogRequestInput { EmailLogId = emailLogId }, methodName, cancellationToken).ConfigureAwait(false);
    }
}
