using System.Diagnostics.Metrics;

namespace AditiKraft.Krafter.Backend.Infrastructure.Telemetry;

public static class BackendTelemetry
{
    public const string MeterName = "AditiKraft.Krafter.Backend";

    public static readonly Meter Meter = new(MeterName, "1.0");

    public static readonly Histogram<double> EfCommandDuration = Meter.CreateHistogram<double>(
        "backend_ef_command_duration_ms",
        unit: "ms",
        description: "Duration of EF database commands in milliseconds");

    public static readonly Counter<long> EfSlowCommandCount = Meter.CreateCounter<long>(
        "backend_ef_slow_command_total",
        description: "Total number of slow EF database commands");

    public static readonly Counter<long> TickerQEnqueueCount = Meter.CreateCounter<long>(
        "backend_tickerq_enqueue_total",
        description: "Total number of TickerQ job enqueue requests");

    public static readonly Counter<long> TickerQEnqueueFailureCount = Meter.CreateCounter<long>(
        "backend_tickerq_enqueue_failures_total",
        description: "Total number of failed TickerQ job enqueue requests");
}
