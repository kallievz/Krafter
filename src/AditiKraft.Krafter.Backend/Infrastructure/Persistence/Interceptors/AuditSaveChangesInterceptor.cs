using System.Text.Json;
using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Common.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors;

public sealed class AuditSaveChangesInterceptor(ICurrentUser currentUser, ITenantGetterService tenantGetterService, AuditMetadataCache metadataCache) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        // Build audit entries and enqueue them in the current DbContext's outbox.
        _ = BuildAuditEntries(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        // Build audit entries and enqueue them in the current DbContext's outbox.
        _ = BuildAuditEntries(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken).ConfigureAwait(false);
    }

    private List<AuditLog> BuildAuditEntries(DbContext? context)
    {
        var result = new List<AuditLog>();
        if (context is null)
        {
            return result;
        }

        foreach (EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
            {
                continue; // don't audit audits
            }

            // Skip auditing for types marked with AuditIgnoreAttribute
            Type clrType = entry.Entity.GetType();
            bool classIgnore = clrType.GetCustomAttributes(false).Any(a => a.GetType().Name == "AuditIgnoreAttribute");
            if (classIgnore)
            {
                continue;
            }

            if (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                // Soft-delete handling
                if (entry.State == EntityState.Deleted && entry.Metadata.FindProperty("IsDeleted") != null)
                {
                    entry.State = EntityState.Modified;
                    entry.CurrentValues["IsDeleted"] = true;
                }

                // Tenant
                if (entry.Entity is ITenant && tenantGetterService?.Tenant is not null)
                {
                    entry.CurrentValues["TenantId"] = tenantGetterService.Tenant.Id;
                }

                // History
                if (entry.Entity is not IHistory)
                {
                }
                else
                {
                    string? userId = currentUser?.GetUserId();
                    if (entry.State == EntityState.Added)
                    {
                        entry.CurrentValues["CreatedOn"] = DateTime.UtcNow;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            entry.CurrentValues["CreatedById"] = userId;
                        }
                    }

                    if (entry.State == EntityState.Modified)
                    {
                        entry.CurrentValues["UpdatedOn"] = DateTime.UtcNow;
                        if (!string.IsNullOrWhiteSpace(userId))
                        {
                            entry.CurrentValues["UpdatedById"] = userId;
                        }
                    }
                }

                // Build audit log
                var audit = new AuditLog
                {
                    TableName = entry.Metadata.GetTableName() ?? entry.Metadata.Name,
                    Action = entry.State.ToString(),
                    TenantId = tenantGetterService?.Tenant?.Id,
                    UserId = currentUser?.GetUserId()
                };

                // Keys - use metadata cache
                AuditMetadata meta = metadataCache.GetOrAdd(clrType, t =>
                {
                    string[] keys = entry.Metadata.FindPrimaryKey()?.Properties.Select(p => p.Name).ToArray() ?? Array.Empty<string>();
                    PropertyInfo[] auditable = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(pi => !pi.GetCustomAttributes(false).Any(a => a.GetType().Name == "AuditIgnoreAttribute"))
                        .ToArray();

                    return new AuditMetadata { KeyProperties = keys, AuditableProperties = auditable };
                });

                var keyValues = new Dictionary<string, object?>();
                foreach (string keyName in meta.KeyProperties)
                {
                    PropertyEntry? prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == keyName);
                    keyValues[keyName] = prop?.CurrentValue;
                }
                audit.KeyValues = JsonSerializer.Serialize(keyValues);

                // Old / New values
                if (entry.State == EntityState.Added)
                {
                    var newValues = new Dictionary<string, object?>();
                    foreach (PropertyInfo propInfo in meta.AuditableProperties)
                    {
                        PropertyEntry? prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propInfo.Name);
                        newValues[propInfo.Name] = prop?.CurrentValue;
                    }
                    audit.NewValues = JsonSerializer.Serialize(newValues, AuditSerializer.Options);
                }
                else if (entry.State == EntityState.Modified)
                {
                    var oldValues = new Dictionary<string, object?>();
                    var newValues = new Dictionary<string, object?>();
                    foreach (PropertyInfo propInfo in meta.AuditableProperties)
                    {
                        PropertyEntry? prop = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propInfo.Name);
                        if (prop != null && prop.IsModified)
                        {
                            oldValues[propInfo.Name] = prop.OriginalValue;
                            newValues[propInfo.Name] = prop.CurrentValue;
                        }
                    }
                    audit.OldValues = JsonSerializer.Serialize(oldValues, AuditSerializer.Options);
                    audit.NewValues = JsonSerializer.Serialize(newValues, AuditSerializer.Options);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    var oldValues = new Dictionary<string, object?>();
                    foreach (PropertyEntry prop in entry.Properties)
                    {
                        PropertyInfo? propInfo = clrType.GetProperty(prop.Metadata.Name);
                        if (propInfo != null && propInfo.GetCustomAttributes(false).Any(a => a.GetType().Name == "AuditIgnoreAttribute"))
                        {
                            continue;
                        }

                        oldValues[prop.Metadata.Name] = prop.OriginalValue;
                    }
                    audit.OldValues = JsonSerializer.Serialize(oldValues);
                }

                result.Add(audit);
            }
        }

        // Convert AuditLog entries into outbox entries so they are saved in the same transaction
        var outboxEntries = new List<AuditOutbox>();
        foreach (AuditLog a in result)
        {
            outboxEntries.Add(new AuditOutbox { Payload = JsonSerializer.Serialize(a), CreatedOn = DateTime.UtcNow });
        }

        if (outboxEntries.Count > 0)
        {
            // If the current DbContext has an AuditOutbox entity, add there so it participates in the
            // current transaction. Otherwise attempt to resolve AuditDbContext from the current
            // context's service provider and persist the outbox entries there.
            var modelHasOutbox = context.Model.FindEntityType(typeof(AuditOutbox)) != null;
            if (modelHasOutbox)
            {
                context.AddRange(outboxEntries);
            }
            else
            {
                // Resolve AuditDbContext from the current DbContext's internal service provider
                try
                {
                    var serviceProvider = ((Microsoft.EntityFrameworkCore.Infrastructure.IInfrastructure<IServiceProvider>)context).Instance;
                    var auditDb = serviceProvider.GetService(typeof(AuditDbContext)) as AuditDbContext;
                    if (auditDb != null)
                    {
                        auditDb.AddRange(outboxEntries);
                        // Persist immediately since we cannot participate in the caller's transaction
                        auditDb.SaveChanges();
                    }
                }
                catch
                {
                    // If resolution fails, swallow to avoid breaking the primary save operation.
                }
            }
        }

        return result;
    }
}
