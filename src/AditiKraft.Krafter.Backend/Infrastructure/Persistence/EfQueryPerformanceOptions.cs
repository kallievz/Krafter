namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public sealed class EfQueryPerformanceOptions
{
    public const string SectionName = "EfQueryPerformance";

    public bool EnableCommandTiming { get; set; } = true;

    public int SlowQueryThresholdMs { get; set; } = 200;
}
