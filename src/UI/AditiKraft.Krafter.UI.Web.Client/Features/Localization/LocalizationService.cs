using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

/// <summary>
/// Service for resolving localized text values from the backend.
/// Handles error scenarios and provides convenience methods for single/batch requests.
/// </summary>
public interface ILocalizationService
{    /// Resolves a single localized text value asynchronously.
     /// </summary>
     /// <param name="key">The resource key to resolve</param>
     /// <param name="fallbackValue">The English fallback value if key doesn't exist</param>
     /// <param name="arguments">Optional format arguments for string.Format substitution</param>
     /// <param name="cancellationToken">Cancellation token</param>
     /// <returns>The localized text value, or fallback if not found</returns>
    public Task<string> ResolveAsync(
        string key,
        string fallbackValue,
        IReadOnlyList<string>? arguments = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves multiple localized text values in a single batch request.
    /// </summary>
    /// <param name="requests">List of localization requests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of resolved localized text values</returns>
    public Task<List<LocalizedTextDto>> ResolveManyAsync(
        IReadOnlyList<LocalizeTextRequest> requests,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Implementation of ILocalizationService that wraps the ILocalizationApi Refit client.
/// </summary>
public class LocalizationService(ILocalizationApi localizationApi) : ILocalizationService
{
    /// <inheritdoc />
    public async Task<string> ResolveAsync(
        string key,
        string fallbackValue,
        IReadOnlyList<string>? arguments = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(fallbackValue);

        var request = new LocalizeTextRequest
        {
            Key = key.Trim(),
            FallbackValue = fallbackValue.Trim(),
            Arguments = arguments?.ToList() ?? []
        };

        try
        {
            Response<LocalizedTextDto> response = await localizationApi.ResolveAsync(request, cancellationToken);

            if (!response.IsError && response.Data is not null)
            {
                return response.Data.Value;
            }

            return fallbackValue;
        }
        catch (HttpRequestException)
        {
            return fallbackValue;
        }
        catch (OperationCanceledException)
        {
            return fallbackValue;
        }
    }

    /// <inheritdoc />
    public async Task<List<LocalizedTextDto>> ResolveManyAsync(
        IReadOnlyList<LocalizeTextRequest> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            return [];
        }

        var batchRequest = new LocalizeBatchRequest
        {
            Entries = requests.ToList()
        };

        try
        {
            Response<List<LocalizedTextDto>> response = await localizationApi.ResolveBatchAsync(batchRequest, cancellationToken);

            if (!response.IsError && response.Data is not null)
            {
                return response.Data;
            }

            return [];
        }
        catch (HttpRequestException)
        {
            return [];
        }
        catch (OperationCanceledException)
        {
            return [];
        }
    }
}
