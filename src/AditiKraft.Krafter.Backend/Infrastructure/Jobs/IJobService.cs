namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public interface IJobService
{
    public Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken);

    public Task EnqueueByLogIdAsync(string emailLogId, string methodName, CancellationToken cancellationToken);
}
