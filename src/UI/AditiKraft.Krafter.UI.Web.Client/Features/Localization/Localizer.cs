using System.Collections.Concurrent;
using System.Globalization;
using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Implements Radzen ILocalizer to provide custom translations.
/// Uses a local cache to enable synchronous @L(key, fallback) syntax.
/// Supports inline fire-and-forget background auto-seeding when keys are missing.
/// </summary>
public class Localizer : ILocalizer
{
    private readonly ILocalizationService _service;
    private readonly ConcurrentDictionary<string, string> _cache = new();
    private readonly ConcurrentHashSet<string> _pendingResolutions = new();

    public Localizer(ILocalizationService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// Get localized string for Radzen components.
    /// Returns cached value or null to fall back to Radzen's built-in default.
    /// </summary>
    public string? Get(string key, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        // Check cache; return cached value or null
        return _cache.TryGetValue(key, out var value) ? value : null;
    }

    /// <summary>
    /// Get localized text synchronously with fallback and fire-and-forget auto-seeding.
    /// Used by Razor components via @L(key, fallbackValue) inline in markup.
    /// 
    /// Behavior:
    /// - If cached: returns cached value immediately
    /// - If not cached: returns fallback immediately AND fires background task to resolve and auto-seed from server
    /// - No blocking; no await required in Razor markup
    /// - Background resolution result is cached for next render
    /// </summary>
    public string GetOrDefault(string key, string fallbackValue)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(fallbackValue);

        // Return cached value immediately if available
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }

        // Key not cached: fire background resolution (fire-and-forget)
        // This will POST to server, trigger auto-seeding if missing, and cache the result
        _ = ResolveAndCacheInBackgroundAsync(key, fallbackValue);

        // Return fallback immediately (non-blocking)
        return fallbackValue;
    }

    /// <summary>
    /// Background task that resolves a key asynchronously and caches the result.
    /// Called by GetOrDefault when a key is not in the local cache.
    /// Does not block; result is cached for the next component render.
    /// </summary>
    private async Task ResolveAndCacheInBackgroundAsync(string key, string fallbackValue)
    {
        // Avoid duplicate concurrent resolution attempts for the same key
        if (!_pendingResolutions.Add(key))
        {
            return;
        }

        try
        {
            // Call backend to resolve and auto-seed if missing
            // This makes an HTTP POST request and triggers server-side auto-creation
            var result = await _service.ResolveAsync(
                key,
                fallbackValue,
                cancellationToken: CancellationToken.None).ConfigureAwait(false);

            // Cache the resolved value for future renders
            _cache.AddOrUpdate(key, result, (_, __) => result);
        }
        catch
        {
            // On error, cache the fallback value to avoid repeated failed attempts
            _cache.AddOrUpdate(key, fallbackValue, (_, __) => fallbackValue);
        }
        finally
        {
            // Remove from pending set
            _pendingResolutions.TryRemove(key);
        }
    }

    /// <summary>
    /// Get localized text with optional format arguments.
    /// </summary>
    public string Format(string key, string fallbackValue, params object[] args)
    {
        string text = GetOrDefault(key, fallbackValue);

        try
        {
            return args.Length > 0 ? string.Format(text, args) : text;
        }
        catch
        {
            return text;
        }
    }

    /// <summary>
    /// Explicitly resolve and cache a key asynchronously.
    /// Use this in component initialization (OnInitializedAsync) when you need the value before rendering.
    /// For inline use in markup, GetOrDefault handles background resolution automatically.
    /// </summary>
    public async Task<string> ResolveAndCacheAsync(string key, string fallbackValue, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(fallbackValue);

        // If already cached, return immediately
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            return cachedValue;
        }

        // Avoid duplicate concurrent resolution attempts for the same key
        if (!_pendingResolutions.Add(key))
        {
            // Already being resolved; return fallback for now
            return fallbackValue;
        }

        try
        {
            // Call backend to resolve and auto-seed if missing
            var result = await _service.ResolveAsync(key, fallbackValue, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Cache the resolved value
            _cache.AddOrUpdate(key, result, (_, __) => result);

            return result;
        }
        catch
        {
            // On error, return fallback
            return fallbackValue;
        }
        finally
        {
            // Remove from pending set
            _pendingResolutions.TryRemove(key);
        }
    }

    /// <summary>
    /// Preload localized text into cache. Called during component initialization.
    /// This triggers auto-seeding for missing keys via batch request.
    /// </summary>
    public async Task PreloadAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        if (keys == null || keys.Length == 0)
        {
            return;
        }

        try
        {
            var requests = keys.Select(key => new LocalizeTextRequest
            {
                Key = key,
                FallbackValue = key
            }).ToList();
            var results = await _service.ResolveManyAsync(requests, cancellationToken).ConfigureAwait(false);

            foreach (var result in results)
            {
                _cache.AddOrUpdate(result.Key, result.Value, (_, __) => result.Value);
            }
        }
        catch
        {
            // Preload failed; cache remains as-is with already cached values
        }
    }

    /// <summary>
    /// Invalidate cache when language changes.
    /// </summary>
    public void InvalidateCache()
    {
        _cache.Clear();
        _pendingResolutions.Clear();
    }
}

/// <summary>
/// Thread-safe HashSet for tracking pending resolutions.
/// </summary>
internal class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _map = new();

    public bool Add(T item) => _map.TryAdd(item, 0);
    public bool TryRemove(T item) => _map.TryRemove(item, out _);
    public void Clear() => _map.Clear();
}
