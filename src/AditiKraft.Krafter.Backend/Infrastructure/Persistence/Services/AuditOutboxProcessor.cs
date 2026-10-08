using System.Text.Json;
using System.Diagnostics.Metrics;
using AditiKraft.Krafter.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services;

public sealed class AuditOutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditOutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);
    // OpenTelemetry metric instruments
    private static readonly Meter _meter = new("AditiKraft.Krafter.Audit", "1.0");
    private static readonly Counter<long> _processedCounter = _meter.CreateCounter<long>(
        "audit_outbox_processed_total",
        description: "Total number of processed audit outbox items");
    private static long _pendingCount;

    public AuditOutboxProcessor(IServiceScopeFactory scopeFactory, ILogger<AuditOutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        // Register an observable gauge that returns the pending count
        _meter.CreateObservableGauge(
            "audit_outbox_pending",
            () => new[] { new Measurement<long>(GetPendingCount()) },
            description: "Current number of pending audit outbox items");
        // No external retry library - simple retry loop will be used in processing.
    }

    private static long GetPendingCount()
    {
        return System.Threading.Interlocked.Read(ref _pendingCount);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using IServiceScope scope = _scopeFactory.CreateScope();
                AuditDbContext auditDb = scope.ServiceProvider.GetRequiredService<AuditDbContext>();

                List<AuditOutbox> pending = await auditDb.AuditOutbox
                    .Where(o => o.ProcessedOn == null)
                    .OrderBy(o => o.CreatedOn)
                    .Take(100)
                    .ToListAsync(stoppingToken)
                    .ConfigureAwait(false);
                System.Threading.Interlocked.Exchange(ref _pendingCount, pending.Count);

                if (pending.Count == 0)
                {
                    await Task.Delay(_interval, stoppingToken).ConfigureAwait(false);
                    continue;
                }

                List<AuditLog> audits = new();
                List<AuditOutbox> processedItems = new();

                foreach (AuditOutbox item in pending)
                {
                    try
                    {
                        AuditLog? audit = JsonSerializer.Deserialize<AuditLog>(item.Payload);
                        if (audit != null)
                        {
                            audits.Add(audit);
                            processedItems.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize audit outbox item {Id}", item.Id);
                    }
                }

                if (audits.Count == 0)
                {
                    continue;
                }

                // Simple retry logic with exponential backoff and dead-lettering
                int attempts = 0;
                const int maxAttempts = 3;
                bool success = false;
                Exception? lastEx = null;

                while (!success && attempts < maxAttempts && !stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        await auditDb.AuditLogs.AddRangeAsync(audits, stoppingToken).ConfigureAwait(false);
                        await auditDb.SaveChangesAsync(stoppingToken).ConfigureAwait(false);

                        DateTime processedOn = DateTime.UtcNow;
                        foreach (AuditOutbox item in processedItems)
                        {
                            item.ProcessedOn = processedOn;
                            item.FailureCount = 0;
                            item.LastError = null;
                        }

                        auditDb.AuditOutbox.UpdateRange(processedItems);
                        await auditDb.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        _processedCounter.Add(processedItems.Count);

                        success = true;
                    }
                    catch (Exception ex)
                    {
                        attempts++;
                        lastEx = ex;
                        _logger.LogWarning(ex, "Attempt {Attempt} failed to deliver audit batch", attempts);

                        auditDb.ChangeTracker.Clear();

                        // Increment failure counters on outbox rows so they can be dead-lettered
                        foreach (AuditOutbox item in processedItems)
                        {
                            item.FailureCount += 1;
                            item.LastError = ex.Message;
                        }
                        try
                        {
                            auditDb.AuditOutbox.UpdateRange(processedItems);
                            await auditDb.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignore failures updating failure counters to avoid masking original error
                        }

                        if (attempts < maxAttempts)
                        {
                            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempts)), stoppingToken).ConfigureAwait(false);
                        }
                    }
                }

                if (!success && lastEx != null)
                {
                    _logger.LogError(lastEx, "Failed to deliver audit batch after {Attempts} attempts; items will be dead-lettered", attempts);

                    // Mark items as processed to avoid infinite retry; they carry FailureCount/LastError for inspection
                    DateTime processedOn = DateTime.UtcNow;
                    foreach (AuditOutbox item in processedItems)
                    {
                        item.ProcessedOn = processedOn;
                    }
                    try
                    {
                        auditDb.AuditOutbox.UpdateRange(processedItems);
                        await auditDb.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
                    }
                    catch
                    {
                        // If marking processed fails, there's nothing safe we can do here
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuditOutboxProcessor encountered an error");
            }

            await Task.Delay(_interval, stoppingToken).ConfigureAwait(false);
        }
    }
}
