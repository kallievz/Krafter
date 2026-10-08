using System.Collections.Concurrent;
using System.Reflection;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services;

public sealed class AuditMetadata
{
    public string[] KeyProperties { get; init; } = Array.Empty<string>();
    public PropertyInfo[] AuditableProperties { get; init; } = Array.Empty<PropertyInfo>();
}

public sealed class AuditMetadataCache
{
    private readonly ConcurrentDictionary<Type, AuditMetadata> _cache = new();

    public AuditMetadata GetOrAdd(Type clrType, Func<Type, AuditMetadata> factory)
    {
        return _cache.GetOrAdd(clrType, factory);
    }
}
