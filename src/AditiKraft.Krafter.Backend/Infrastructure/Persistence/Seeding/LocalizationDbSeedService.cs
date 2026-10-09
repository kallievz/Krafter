using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Seeding;

public static class LocalizationDbSeedService
{
    public static async Task SeedLanguagesAsync(LocalizationDbContext dbContext, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        List<Language> existingLanguages = await dbContext.Languages
            .AsNoTracking()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        IEnumerable<Language> missingLanguages = LocalizationLanguageSeedData.GetSeeds()
            .Where(seedLanguage => existingLanguages.All(existingLanguage => existingLanguage.Code != seedLanguage.Code));

        List<Language> languagesToInsert = missingLanguages.ToList();
        if (languagesToInsert.Count() > 0)
        {
            await dbContext.Languages.AddRangeAsync(languagesToInsert, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    public static async Task SeedResourcesAsync(LocalizationDbContext dbContext, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        List<Language> languages = await dbContext.Languages
            .AsNoTracking()
            .Where(language => language.Code == "en")
            .OrderBy(language => language.Code)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        if (languages.Count() == 0)
        {
            return;
        }

        List<Resource> existingResources = await dbContext.Resources
            .AsNoTracking()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        IEnumerable<Resource> missingResources = LocalizationResourceSeedData.GetResources()
            .Where(seedResource => existingResources.All(existingResource => existingResource.Key != seedResource.Key));

        List<Resource> resourcesToInsert = missingResources.ToList();
        if (resourcesToInsert.Count() > 0)
        {
            await dbContext.Resources.AddRangeAsync(resourcesToInsert, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        List<Resource> allResources = await dbContext.Resources
            .AsNoTracking()
            .OrderBy(resource => resource.Key)
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        List<ResourceTranslation> existingTranslations = await dbContext.ResourceTranslations
            .AsNoTracking()
            .ToListAsync(cancellationToken).ConfigureAwait(false);

        IEnumerable<ResourceTranslation> missingTranslations = LocalizationResourceSeedData.GetTranslations(allResources, languages)
            .Where(seedTranslation => existingTranslations.All(existingTranslation =>
                existingTranslation.ResourceId != seedTranslation.ResourceId ||
                existingTranslation.LanguageId != seedTranslation.LanguageId));

        List<ResourceTranslation> translationsToInsert = missingTranslations.ToList();
        if (translationsToInsert.Count() > 0)
        {
            await dbContext.ResourceTranslations.AddRangeAsync(translationsToInsert, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
