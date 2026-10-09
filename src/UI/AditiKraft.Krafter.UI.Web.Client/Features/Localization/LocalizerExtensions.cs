namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Extension methods for Radzen.ILocalizer to provide unified localization functionality.
/// </summary>
public static class LocalizerExtensions
{
    /// <summary>
    /// Gets a localized string for the given key with optional formatting.
    /// Inline synchronous usage for Razor markup: @L.Localize("key", "fallback")
    /// 
    /// Behavior (fire-and-forget auto-seeding):
    /// - If cached: returns cached value immediately
    /// - If not cached: returns fallback immediately, fires background HTTP POST to server for auto-seeding
    /// - Background result is cached for next render
    /// - Non-blocking; designed for inline use in Razor @expressions
    /// 
    /// Note: Formatting is applied only if value is available (cached or auto-seeded previously).
    /// On first render with missing key, formatted fallback is returned; after background resolution,
    /// next render will use formatted localized value.
    /// </summary>
    /// <param name="localizer">The localizer instance</param>
    /// <param name="key">The localization key</param>
    /// <param name="fallbackValue">The fallback value if key not found</param>
    /// <param name="args">Optional format arguments. If null or empty, formatting is skipped.</param>
    /// <returns>Localized string (cached or from fallback), optionally formatted with args</returns>
    public static string Localize(this Radzen.ILocalizer localizer, string key, string fallbackValue, params object?[]? args)
    {
        if (localizer is null)
            return fallbackValue;

        string value;
        if (localizer is Localizer concreteLocalizer)
        {
            value = concreteLocalizer.GetOrDefault(key, fallbackValue);
        }
        else
        {
            // Fallback to calling Get with current culture
            var result = localizer.Get(key, System.Globalization.CultureInfo.CurrentCulture);
            value = string.IsNullOrEmpty(result) ? fallbackValue : result;
        }

        // Only format if args are provided and non-empty
        if (args != null && args.Length > 0)
        {
            try
            {
                return string.Format(value, args);
            }
            catch
            {
                // If formatting fails, return unformatted value
                return value;
            }
        }

        return value;
    }

    /// <summary>
    /// Explicitly resolve and cache a localized string asynchronously.
    /// Use this in component initialization (OnInitializedAsync) when you need the localized value BEFORE rendering.
    /// 
    /// This method will wait for the backend response, ensuring the result is cached before the component renders.
    /// For inline Razor markup use, use Localize() instead (which fires background resolution automatically).
    /// </summary>
    /// <param name="localizer">The localizer instance</param>
    /// <param name="key">The localization key</param>
    /// <param name="fallbackValue">The fallback value if key not found</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The localized text value or fallback</returns>
    public static async Task<string> ResolveAndCacheAsync(
        this Radzen.ILocalizer localizer,
        string key,
        string fallbackValue,
        CancellationToken cancellationToken = default)
    {
        if (localizer is null)
            return fallbackValue;

        if (localizer is not Localizer concreteLocalizer)
        {
            // Can't resolve without our concrete implementation
            var result = localizer.Get(key, System.Globalization.CultureInfo.CurrentCulture);
            return string.IsNullOrEmpty(result) ? fallbackValue : result;
        }

        return await concreteLocalizer.ResolveAndCacheAsync(key, fallbackValue, cancellationToken).ConfigureAwait(false);
    }
}
