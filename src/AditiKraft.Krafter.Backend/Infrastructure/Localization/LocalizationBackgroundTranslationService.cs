using AditiKraft.Krafter.Backend.Common;
using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Features.Localization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Infrastructure.Localization;

public interface ILocalizationBackgroundTranslationService : IScopedService
{
    public Task GenerateMissingTranslationsAsync(string resourceId, CancellationToken cancellationToken);
}

public sealed class LocalizationBackgroundTranslationService(
    LocalizationDbContext localizationDbContext,
    IOllamaLocalizationTranslator ollamaLocalizationTranslator,
    IOptions<LocalizationOllamaOptions> localizationOptions,
    ILocalizationTextResolver localizationTextResolver,
    ILogger<LocalizationBackgroundTranslationService> logger) : ILocalizationBackgroundTranslationService
{
    private const string EnglishLanguageCode = "en";

    public async Task GenerateMissingTranslationsAsync(string resourceId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            throw new ArgumentException("Resource id is required.", nameof(resourceId));
        }

        LocalizationOllamaOptions options = localizationOptions.Value;
        if (!options.Enabled)
        {
            logger.LogInformation("Skipping localization background translation for resource '{ResourceId}' because Localization:Ollama:Enabled is false.", resourceId);
            return;
        }

        string endpoint = string.IsNullOrWhiteSpace(options.Endpoint)
            ? "http://localhost:11434"
            : options.Endpoint.Trim();

        string model = string.IsNullOrWhiteSpace(options.Model)
            ? "llama3.1"
            : options.Model.Trim();

        logger.LogInformation("Starting background localization translation for resource '{ResourceId}' using model '{Model}' at '{Endpoint}'.", resourceId, model, endpoint);

        bool modelIsServed = await ollamaLocalizationTranslator
            .IsModelServedAsync(endpoint, model, cancellationToken)
            .ConfigureAwait(false);

        if (!modelIsServed)
        {
            logger.LogWarning("Skipping localization background translation for resource '{ResourceId}' because model '{Model}' is not served at '{Endpoint}'.", resourceId, model, endpoint);
            return;
        }

        ResourceTranslation? englishTranslation = await localizationDbContext.ResourceTranslations
            .Include(resourceTranslation => resourceTranslation.Resource)
            .Include(resourceTranslation => resourceTranslation.Language)
            .FirstOrDefaultAsync(
                resourceTranslation =>
                    resourceTranslation.ResourceId == resourceId &&
                    resourceTranslation.Language != null &&
                    resourceTranslation.Language.Code == EnglishLanguageCode,
                cancellationToken)
            .ConfigureAwait(false);

        if (englishTranslation is null || string.IsNullOrWhiteSpace(englishTranslation.Value))
        {
            logger.LogInformation("Skipping background localization translation for resource '{ResourceId}' because no English source translation exists.", resourceId);
            return;
        }

        List<Language> allTargetLanguages = await localizationDbContext.Languages
            .AsNoTracking()
            .Where(language => language.Code != EnglishLanguageCode)
            .OrderBy(language => language.Code)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (allTargetLanguages.Count == 0)
        {
            logger.LogInformation("Skipping background localization translation for resource '{ResourceId}' because no non-English languages were found.", resourceId);
            return;
        }

        HashSet<string> existingLanguageIds = await localizationDbContext.ResourceTranslations
            .AsNoTracking()
            .Where(resourceTranslation => resourceTranslation.ResourceId == resourceId)
            .Select(resourceTranslation => resourceTranslation.LanguageId)
            .ToHashSetAsync(cancellationToken)
            .ConfigureAwait(false);

        int insertedTranslations = 0;

        foreach (Language targetLanguage in allTargetLanguages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (existingLanguageIds.Contains(targetLanguage.Id))
            {
                continue;
            }

            string? translatedValue = await ollamaLocalizationTranslator.TranslateAsync(
                englishTranslation.Value,
                EnglishLanguageCode,
                "English",
                targetLanguage.Code,
                targetLanguage.Name,
                endpoint,
                model,
                cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(translatedValue))
            {
                continue;
            }

            ResourceTranslation translation = new()
            {
                Id = Guid.CreateVersion7().ToString(),
                ResourceId = resourceId,
                LanguageId = targetLanguage.Id,
                Value = translatedValue.Trim()
            };

            localizationDbContext.ResourceTranslations.Add(translation);

            try
            {
                _ = await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                existingLanguageIds.Add(targetLanguage.Id);
                insertedTranslations++;
            }
            catch (DbUpdateException ex) when (IsDuplicateResourceLanguageTranslation(ex))
            {
                localizationDbContext.Entry(translation).State = EntityState.Detached;
            }
        }

        if (insertedTranslations > 0 && englishTranslation.Resource is not null)
        {
            localizationTextResolver.InvalidateByResourceKey(englishTranslation.Resource.Key);
        }

        logger.LogInformation("Background localization translation completed for resource '{ResourceId}'. Inserted translations: {InsertedTranslations}.", resourceId, insertedTranslations);
    }

    private static bool IsDuplicateResourceLanguageTranslation(DbUpdateException exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        string message = $"{exception.Message} {exception.InnerException?.Message}";

        return message.Contains("IX_ResourceTranslations_ResourceId_LanguageId", StringComparison.OrdinalIgnoreCase)
            || message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
            || message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase);
    }
}
