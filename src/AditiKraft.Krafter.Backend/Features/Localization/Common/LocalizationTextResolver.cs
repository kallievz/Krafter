using System.Collections.Concurrent;
using System.Globalization;
using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AditiKraft.Krafter.Backend.Features.Localization.Common;

public interface ILocalizationTextResolver : IScopedService
{
    /// <summary>
    /// Resolves one localized text value, auto-creating the English resource/translation from fallback when required.
    /// </summary>
    public Task<LocalizedTextDto> ResolveAsync(
        LocalizeTextRequest request,
        string preferredLanguageCode,
        CancellationToken cancellationToken);

    /// <summary>
    /// Resolves a list of localized text values.
    /// </summary>
    public Task<List<LocalizedTextDto>> ResolveManyAsync(
        IReadOnlyList<LocalizeTextRequest> requests,
        string preferredLanguageCode,
        CancellationToken cancellationToken);

    /// <summary>
    /// Invalidates cache entries for a resource key.
    /// </summary>
    public void InvalidateByResourceKey(string resourceKey);

    /// <summary>
    /// Invalidates cache entries for a resource id.
    /// </summary>
    public Task InvalidateByResourceIdAsync(string resourceId, CancellationToken cancellationToken);

    /// <summary>
    /// Invalidates all localization cache entries.
    /// </summary>
    public void InvalidateAll();
}

public sealed class LocalizationTextResolver(LocalizationDbContext localizationDbContext, IMemoryCache memoryCache, ILogger<LocalizationTextResolver>? logger = null)
    : ILocalizationTextResolver
{
    private static long _globalVersion;
    private static readonly ConcurrentDictionary<string, long> _keyVersions = new(StringComparer.OrdinalIgnoreCase);

    private static readonly MemoryCacheEntryOptions _cacheOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };

    // Using primary constructor parameters: localizationDbContext, memoryCache, logger

    public async Task<LocalizedTextDto> ResolveAsync(
        LocalizeTextRequest request,
        string preferredLanguageCode,
        CancellationToken cancellationToken)
    {
        string key = NormalizeKey(request.Key);
        string fallbackValue = NormalizeFallbackValue(request.FallbackValue, key);
        string normalizedPreferredLanguageCode = NormalizeLanguageCode(preferredLanguageCode);

        if (string.IsNullOrWhiteSpace(key))
        {
            return new LocalizedTextDto
            {
                Key = string.Empty,
                Value = FormatValue(fallbackValue, request.Arguments),
                LanguageCode = normalizedPreferredLanguageCode,
                UsedFallback = true,
                AutoCreatedEnglishResource = false,
                AutoCreatedEnglishTranslation = false
            };
        }

        CachedTranslation preferredTranslation = await GetTranslationAsync(
            normalizedPreferredLanguageCode,
            key,
            cancellationToken).ConfigureAwait(false);

        CachedTranslation englishTranslation = await GetTranslationAsync(
            "en",
            key,
            cancellationToken).ConfigureAwait(false);

        bool missingInPreferredLanguage = !preferredTranslation.Exists;
        bool missingInEnglish = !englishTranslation.Exists;

        bool autoCreatedEnglishResource = false;
        bool autoCreatedEnglishTranslation = false;

        if (missingInPreferredLanguage || missingInEnglish)
        {
            logger?.LogInformation("Resolving missing localization for key '{Key}'. MissingPreferred: {MissingPreferred}, MissingEnglish: {MissingEnglish}", 
                key, missingInPreferredLanguage, missingInEnglish);

            (autoCreatedEnglishResource, autoCreatedEnglishTranslation) =
                await EnsureEnglishResourceAndTranslationAsync(key, fallbackValue, cancellationToken)
                    .ConfigureAwait(false);

            InvalidateByResourceKey(key);

            // Re-fetch translations after auto-creation to ensure we have the latest data
            preferredTranslation = await GetTranslationAsync(
                normalizedPreferredLanguageCode,
                key,
                cancellationToken).ConfigureAwait(false);

            englishTranslation = await GetTranslationAsync(
                "en",
                key,
                cancellationToken).ConfigureAwait(false);

            logger?.LogInformation("After auto-creation, preferred translation exists: {PrefExists}, English translation exists: {EngExists}", 
                preferredTranslation.Exists, englishTranslation.Exists);
        }

        // If we have a translation (either preferred or English), use it; otherwise use fallback
        string resolvedValue = preferredTranslation.Exists 
            ? preferredTranslation.Value 
            : (englishTranslation.Exists ? englishTranslation.Value : fallbackValue);

        bool usedFallback = !preferredTranslation.Exists && !englishTranslation.Exists;

        return new LocalizedTextDto
        {
            Key = key,
            Value = FormatValue(resolvedValue, request.Arguments),
            LanguageCode = normalizedPreferredLanguageCode,
            UsedFallback = usedFallback,
            AutoCreatedEnglishResource = autoCreatedEnglishResource,
            AutoCreatedEnglishTranslation = autoCreatedEnglishTranslation
        };
    }

    public async Task<List<LocalizedTextDto>> ResolveManyAsync(
        IReadOnlyList<LocalizeTextRequest> requests,
        string preferredLanguageCode,
        CancellationToken cancellationToken)
    {
        List<LocalizedTextDto> responses = new();

        foreach (LocalizeTextRequest request in requests)
        {
            LocalizedTextDto response = await ResolveAsync(request, preferredLanguageCode, cancellationToken)
                .ConfigureAwait(false);
            responses.Add(response);
        }

        return responses;
    }

    public void InvalidateByResourceKey(string resourceKey)
    {
        string normalizedResourceKey = NormalizeKey(resourceKey);
        if (string.IsNullOrWhiteSpace(normalizedResourceKey))
        {
            return;
        }

        _keyVersions.AddOrUpdate(normalizedResourceKey, 1, (_, version) => version + 1);
    }

    public async Task InvalidateByResourceIdAsync(string resourceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            return;
        }

        string? resourceKey = await localizationDbContext.Resources.AsNoTracking()
            .Where(resource => resource.Id == resourceId)
            .Select(resource => resource.Key)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!string.IsNullOrWhiteSpace(resourceKey))
        {
            InvalidateByResourceKey(resourceKey);
        }
    }

    public void InvalidateAll()
    {
        Interlocked.Increment(ref _globalVersion);
    }

    private async Task<(bool AutoCreatedEnglishResource, bool AutoCreatedEnglishTranslation)> EnsureEnglishResourceAndTranslationAsync(
        string key,
        string fallbackValue,
        CancellationToken cancellationToken)
    {
        Language? englishLanguage = await localizationDbContext.Languages
            .FirstOrDefaultAsync(language => language.Code == "en", cancellationToken)
            .ConfigureAwait(false);

        if (englishLanguage is null)
        {
            logger?.LogInformation("English language not found, auto-creating...");
            englishLanguage = new Language
            {
                Id = Guid.CreateVersion7().ToString(),
                Code = "en",
                Name = "English",
                IsActive = true
            };

            await localizationDbContext.Languages.AddAsync(englishLanguage, cancellationToken).ConfigureAwait(false);
        }

        Resource? resource = await localizationDbContext.Resources
            .FirstOrDefaultAsync(localizationResource => localizationResource.Key == key, cancellationToken)
            .ConfigureAwait(false);

        bool autoCreatedEnglishResource = false;
        if (resource is null)
        {
            logger?.LogInformation("Resource key '{Key}' not found, auto-creating with fallback value: '{Value}'", key, fallbackValue);
            resource = new Resource
            {
                Id = Guid.CreateVersion7().ToString(),
                Key = key,
                Description = fallbackValue
            };

            autoCreatedEnglishResource = true;
            await localizationDbContext.Resources.AddAsync(resource, cancellationToken).ConfigureAwait(false);
        }

        ResourceTranslation? translation = await localizationDbContext.ResourceTranslations
            .FirstOrDefaultAsync(resourceTranslation =>
                resourceTranslation.ResourceId == resource.Id && resourceTranslation.LanguageId == englishLanguage.Id,
                cancellationToken)
            .ConfigureAwait(false);

        bool autoCreatedEnglishTranslation = false;
        if (translation is null)
        {
            logger?.LogInformation("Translation for key '{Key}' in English not found, auto-creating translation...", key);
            translation = new ResourceTranslation
            {
                Id = Guid.CreateVersion7().ToString(),
                ResourceId = resource.Id,
                LanguageId = englishLanguage.Id,
                Value = fallbackValue
            };

            autoCreatedEnglishTranslation = true;
            await localizationDbContext.ResourceTranslations.AddAsync(translation, cancellationToken)
                .ConfigureAwait(false);
        }

        if (autoCreatedEnglishResource || autoCreatedEnglishTranslation)
        {
            logger?.LogInformation("Saving auto-created localization resources for key '{Key}'", key);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return (autoCreatedEnglishResource, autoCreatedEnglishTranslation);
    }

    private async Task<CachedTranslation> GetTranslationAsync(
        string languageCode,
        string resourceKey,
        CancellationToken cancellationToken)
    {
        string cacheKey = BuildCacheKey(languageCode, resourceKey);
        if (memoryCache.TryGetValue(cacheKey, out CachedTranslation? cachedTranslation) && cachedTranslation is not null)
        {
            return cachedTranslation;
        }

        string? translationValue = await localizationDbContext.ResourceTranslations.AsNoTracking()
            .Include(rt => rt.Resource)
            .Include(rt => rt.Language)
            .Where(resourceTranslation => resourceTranslation.Resource != null &&
                                          resourceTranslation.Language != null &&
                                          resourceTranslation.Resource.Key == resourceKey &&
                                          resourceTranslation.Language.Code == languageCode)
            .Select(resourceTranslation => resourceTranslation.Value)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        CachedTranslation translation = string.IsNullOrWhiteSpace(translationValue)
            ? new CachedTranslation(false, string.Empty)
            : new CachedTranslation(true, translationValue);

        memoryCache.Set(cacheKey, translation, _cacheOptions);
        return translation;
    }

    private static string BuildCacheKey(string languageCode, string resourceKey)
    {
        long globalVersion = Interlocked.Read(ref _globalVersion);
        long keyVersion = _keyVersions.TryGetValue(resourceKey, out long existingVersion) ? existingVersion : 0;
        return $"localization:{globalVersion}:{keyVersion}:{languageCode}:{resourceKey}";
    }

    private static string NormalizeLanguageCode(string? languageCode)
    {
        return string.IsNullOrWhiteSpace(languageCode)
            ? "en"
            : languageCode.Trim().ToLowerInvariant();
    }

    private static string NormalizeKey(string? key)
    {
        return string.IsNullOrWhiteSpace(key)
            ? string.Empty
            : key.Trim();
    }

    private static string NormalizeFallbackValue(string? fallbackValue, string key)
    {
        if (!string.IsNullOrWhiteSpace(fallbackValue))
        {
            return fallbackValue.Trim();
        }

        return key;
    }

    private static string FormatValue(string format, IReadOnlyList<string>? arguments)
    {
        if (arguments is null || arguments.Count == 0)
        {
            return format;
        }

        object[] values = arguments.Cast<object>().ToArray();

        try
        {
            return string.Format(CultureInfo.InvariantCulture, format, values);
        }
        catch (FormatException)
        {
            return format;
        }
    }

    private sealed record CachedTranslation(bool Exists, string Value);
}
