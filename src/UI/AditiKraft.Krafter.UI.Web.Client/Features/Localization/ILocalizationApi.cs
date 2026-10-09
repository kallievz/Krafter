using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Refit client for backend localization resolve endpoints.
/// </summary>
public interface ILocalizationApi
{
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
