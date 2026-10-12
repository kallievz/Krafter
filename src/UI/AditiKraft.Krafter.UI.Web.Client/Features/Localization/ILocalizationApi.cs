using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Refit client for backend localization resolve endpoints.
/// </summary>
public interface ILocalizationApi
{
    [Get("/api/users/localization/languages")]
    public Task<Response<List<LanguageDto>>> GetLanguagesAsync(CancellationToken cancellationToken = default);

    [Post("/api/users/localization/languages")]
    public Task<Response<LanguageDto>> CreateLanguageAsync([Body] CreateLanguageRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/users/localization/languages/{id}")]
    public Task<Response<LanguageDto>> UpdateLanguageAsync(string id, [Body] UpdateLanguageRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/users/localization/languages/{id}")]
    public Task<Response> DeleteLanguageAsync(string id, CancellationToken cancellationToken = default);

    [Get("/api/users/localization/resources")]
    public Task<Response<List<ResourceDto>>> GetResourcesAsync(CancellationToken cancellationToken = default);

    [Post("/api/users/localization/resources")]
    public Task<Response<ResourceDto>> CreateResourceAsync([Body] CreateResourceRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/users/localization/resources/{id}")]
    public Task<Response<ResourceDto>> UpdateResourceAsync(string id, [Body] UpdateResourceRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/users/localization/resources/{id}")]
    public Task<Response> DeleteResourceAsync(string id, CancellationToken cancellationToken = default);

    [Get("/api/users/localization/resource-translations")]
    public Task<Response<List<ResourceTranslationDto>>> GetResourceTranslationsAsync(CancellationToken cancellationToken = default);

    [Post("/api/users/localization/resource-translations")]
    public Task<Response<ResourceTranslationDto>> CreateResourceTranslationAsync([Body] CreateResourceTranslationRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/users/localization/resource-translations/{id}")]
    public Task<Response<ResourceTranslationDto>> UpdateResourceTranslationAsync(string id, [Body] UpdateResourceTranslationRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/users/localization/resource-translations/{id}")]
    public Task<Response> DeleteResourceTranslationAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves a single localized text value.
    /// </summary>
    [Post("/api/users/localization/values/resolve")]
    public Task<Response<LocalizedTextDto>> ResolveAsync(
        [Body] LocalizeTextRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves multiple localized text values in a single batch request.
    /// </summary>
    [Post("/api/users/localization/values/resolve/batch")]
    public Task<Response<List<LocalizedTextDto>>> ResolveBatchAsync(
        [Body] LocalizeBatchRequest request,
        CancellationToken cancellationToken = default);
}
