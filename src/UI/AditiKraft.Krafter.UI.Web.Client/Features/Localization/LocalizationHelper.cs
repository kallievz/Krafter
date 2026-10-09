using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Localization helper utilities for Blazor components.
/// Provides extension methods and utilities for resolving localized text.
/// </summary>
public static class LocalizationHelper
{
    /// <summary>
    /// Resolves a single localized text value.
    /// </summary>
    public static async Task<string> LocalizeAsync(
        this ILocalizationService service,
        string key,
        string fallbackValue,
        IReadOnlyList<string>? arguments = null,
        CancellationToken cancellationToken = default)
    {
        return await service.ResolveAsync(key, fallbackValue, arguments, cancellationToken);
    }

    /// <summary>
    /// Resolves a localized text value with a single format argument.
    /// </summary>
    public static async Task<string> LocalizeAsync(
        this ILocalizationService service,
        string key,
        string fallbackValue,
        string arg,
        CancellationToken cancellationToken = default)
    {
        return await service.ResolveAsync(key, fallbackValue, [arg], cancellationToken);
    }

    /// <summary>
    /// Resolves a localized text value with multiple format arguments.
    /// </summary>
    public static async Task<string> LocalizeAsync(
        this ILocalizationService service,
        string key,
        string fallbackValue,
        params string[] args)
    {
        return await service.ResolveAsync(key, fallbackValue, args);
    }

    /// <summary>
    /// Resolves multiple localized text values in a batch request.
    /// </summary>
    public static async Task<List<LocalizedTextDto>> LocalizeBatchAsync(
        this ILocalizationService service,
        IReadOnlyList<LocalizeTextRequest> requests,
        CancellationToken cancellationToken = default)
    {
        return await service.ResolveManyAsync(requests, cancellationToken);
    }

    /// <summary>
    /// Creates a localization request for use in batch operations.
    /// </summary>
    public static LocalizeTextRequest CreateRequest(
        string key,
        string fallbackValue,
        IReadOnlyList<string>? arguments = null)
    {
        return new LocalizeTextRequest
        {
            Key = key,
            FallbackValue = fallbackValue,
            Arguments = arguments?.ToList() ?? []
        };
    }
}
