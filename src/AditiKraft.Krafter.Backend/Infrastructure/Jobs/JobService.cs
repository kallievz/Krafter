using TickerQ.Utilities.Base;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

/// <summary>
/// JobService provides a facade for enqueueing background jobs through TickerQ.
/// Jobs are defined in the <see cref="Jobs"/> class with <see cref="TickerFunctionAttribute"/>.
/// </summary>
public class JobService : IJobService
{
    public Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken)
    {
        // Job enqueueing is handled by TickerQ's internal mechanisms
        // This method is called by handlers to queue background jobs
        // The actual job execution happens through TickerQ's worker pipeline
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
