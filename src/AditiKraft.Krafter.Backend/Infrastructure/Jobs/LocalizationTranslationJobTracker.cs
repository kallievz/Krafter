using System.Collections.Concurrent;

namespace AditiKraft.Krafter.Backend.Infrastructure.Jobs;

public enum LocalizationTranslationJobState
{
    Unknown = 0,
    Enqueued = 1,
    Executing = 2,
    Completed = 3,
    Failed = 4
}

public sealed class LocalizationTranslationJobStatus
{
    public string ResourceId { get; init; } = string.Empty;
    public LocalizationTranslationJobState State { get; init; }
    public DateTime UpdatedOnUtc { get; init; }
    public string? ErrorMessage { get; init; }
}

public sealed class LocalizationTranslationJobHealth
{
    public int TotalTracked { get; init; }
    public int EnqueuedCount { get; init; }
    public int ExecutingCount { get; init; }
    public int CompletedCount { get; init; }
    public int FailedCount { get; init; }
    public DateTime UpdatedOnUtc { get; init; }
}

public interface ILocalizationTranslationJobTracker
{
    public void MarkEnqueued(string resourceId);
    public void MarkExecuting(string resourceId);
    public void MarkCompleted(string resourceId);
    public void MarkFailed(string resourceId, string? errorMessage);
    public LocalizationTranslationJobStatus GetStatus(string resourceId);
    public LocalizationTranslationJobHealth GetHealth();
}

public sealed class LocalizationTranslationJobTracker : ILocalizationTranslationJobTracker
{
    private readonly ConcurrentDictionary<string, LocalizationTranslationJobStatus> _statusByResourceId =
        new(StringComparer.OrdinalIgnoreCase);

    public void MarkEnqueued(string resourceId)
    {
        Upsert(resourceId, LocalizationTranslationJobState.Enqueued, null);
    }

    public void MarkExecuting(string resourceId)
    {
        Upsert(resourceId, LocalizationTranslationJobState.Executing, null);
    }

    public void MarkCompleted(string resourceId)
    {
        Upsert(resourceId, LocalizationTranslationJobState.Completed, null);
    }

    public void MarkFailed(string resourceId, string? errorMessage)
    {
        Upsert(resourceId, LocalizationTranslationJobState.Failed, string.IsNullOrWhiteSpace(errorMessage) ? null : errorMessage.Trim());
    }

    public LocalizationTranslationJobStatus GetStatus(string resourceId)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return new LocalizationTranslationJobStatus
            {
                ResourceId = string.Empty,
                State = LocalizationTranslationJobState.Unknown,
                UpdatedOnUtc = DateTime.UtcNow
            };
        }

        string key = resourceId.Trim();
        if (_statusByResourceId.TryGetValue(key, out LocalizationTranslationJobStatus? status))
        {
            return status;
        }

        return new LocalizationTranslationJobStatus
        {
            ResourceId = key,
            State = LocalizationTranslationJobState.Unknown,
            UpdatedOnUtc = DateTime.UtcNow
        };
    }

    public LocalizationTranslationJobHealth GetHealth()
    {
        LocalizationTranslationJobStatus[] statuses = _statusByResourceId.Values.ToArray();
        DateTime updatedOn = statuses.Length == 0
            ? DateTime.UtcNow
            : statuses.Max(item => item.UpdatedOnUtc);

        return new LocalizationTranslationJobHealth
        {
            TotalTracked = statuses.Length,
            EnqueuedCount = statuses.Count(item => item.State == LocalizationTranslationJobState.Enqueued),
            ExecutingCount = statuses.Count(item => item.State == LocalizationTranslationJobState.Executing),
            CompletedCount = statuses.Count(item => item.State == LocalizationTranslationJobState.Completed),
            FailedCount = statuses.Count(item => item.State == LocalizationTranslationJobState.Failed),
            UpdatedOnUtc = updatedOn
        };
    }

    private void Upsert(string resourceId, LocalizationTranslationJobState state, string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return;
        }

        string key = resourceId.Trim();
        LocalizationTranslationJobStatus status = new()
        {
            ResourceId = key,
            State = state,
            UpdatedOnUtc = DateTime.UtcNow,
            ErrorMessage = errorMessage
        };

        _statusByResourceId.AddOrUpdate(key, status, (_, __) => status);
    }
}
