namespace AditiKraft.Krafter.Backend.Web.Middleware;

public sealed class EndpointTimingOptions
{
    public const string SectionName = "EndpointPerformance";

    public bool EnableTiming { get; set; } = true;

    public int SlowRequestThresholdMs { get; set; } = 400;

    public bool ApiOnly { get; set; } = true;
}
