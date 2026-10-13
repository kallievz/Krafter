using System.Collections.Concurrent;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class Resources(
    ApiCallService api,
    ILocalizationApi localizationApi,
    DialogService dialogService
) : ComponentBase
{
    [Inject]
    private ILocalizer L { get; set; } = null!;
    public const string RoutePath = ApiRoutes.LocalizationResources;

    private bool _isLoading = true;
    private bool _showOutstandingOnly;
    private int _totalKeysCount;
    private int _localizedKeysCount;
    private int _outstandingKeysCount;
    private List<ResourceDto> _visibleResources = [];
    private readonly ConcurrentDictionary<string, bool> _isResourceFullyLocalizedById =
        new(StringComparer.OrdinalIgnoreCase);

    private Response<List<ResourceDto>>? _response = new() { Data = [] };

    private bool ShowOutstandingOnly
    {
        get => _showOutstandingOnly;
        set
        {
            if (_showOutstandingOnly == value)
            {
                return;
            }

            _showOutstandingOnly = value;
            RefreshVisibleResources();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        LocalAppState.CurrentPageTitle = L.Localize("Nav_Resources", "Resources");
        await GetListAsync().ConfigureAwait(false);
    }

    private async Task GetListAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _response = await api.CallAsync(
            () => localizationApi.GetResourcesAsync(),
            errorTitle: L.Localize("Entity_Resources", "Resources")).ConfigureAwait(false);

        await LoadLocalizationProgressAsync().ConfigureAwait(false);

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadLocalizationProgressAsync()
    {
        List<ResourceDto> resources = _response?.Data ?? [];
        _totalKeysCount = resources.Count;
        _isResourceFullyLocalizedById.Clear();

        if (_totalKeysCount == 0)
        {
            _localizedKeysCount = 0;
            _outstandingKeysCount = 0;
            RefreshVisibleResources();
            return;
        }

        Response<List<LanguageDto>> languagesResponse = await api.CallAsync(
            () => localizationApi.GetLanguagesAsync(),
            showErrorNotification: false).ConfigureAwait(false);

        List<LanguageDto> activeTargetLanguages = languagesResponse.Data?
            .Where(language => language.IsActive && !string.Equals(language.Code, "en", StringComparison.OrdinalIgnoreCase))
            .ToList() ?? [];

        if (activeTargetLanguages.Count == 0)
        {
            _localizedKeysCount = _totalKeysCount;
            _outstandingKeysCount = 0;

            foreach (ResourceDto resource in resources)
            {
                if (!string.IsNullOrWhiteSpace(resource.Id))
                {
                    _isResourceFullyLocalizedById[resource.Id] = true;
                }
            }

            RefreshVisibleResources();
            return;
        }

        HashSet<string> targetLanguageIds = activeTargetLanguages
            .Select(language => language.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Response<List<ResourceTranslationDto>> translationsResponse = await api.CallAsync(
            () => localizationApi.GetResourceTranslationsAsync(),
            showErrorNotification: false).ConfigureAwait(false);

        List<ResourceTranslationDto> translations = translationsResponse.Data ?? [];

        Dictionary<string, HashSet<string>> translatedLanguageIdsByResourceId = translations
            .Where(translation =>
                !string.IsNullOrWhiteSpace(translation.ResourceId) &&
                !string.IsNullOrWhiteSpace(translation.LanguageId) &&
                !string.IsNullOrWhiteSpace(translation.Value) &&
                targetLanguageIds.Contains(translation.LanguageId))
            .GroupBy(translation => translation.ResourceId)
            .ToDictionary(
                grouping => grouping.Key,
                grouping => grouping.Select(item => item.LanguageId).ToHashSet(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);

        int localizedCount = 0;
        foreach (ResourceDto resource in resources)
        {
            bool isFullyLocalized =
                !string.IsNullOrWhiteSpace(resource.Id) &&
                translatedLanguageIdsByResourceId.TryGetValue(resource.Id, out HashSet<string>? translatedLanguageIds) &&
                translatedLanguageIds is not null &&
                targetLanguageIds.All(targetLanguageId => translatedLanguageIds.Contains(targetLanguageId));

            if (!string.IsNullOrWhiteSpace(resource.Id))
            {
                _isResourceFullyLocalizedById[resource.Id] = isFullyLocalized;
            }

            if (isFullyLocalized)
            {
                localizedCount++;
            }
        }

        _localizedKeysCount = localizedCount;
        _outstandingKeysCount = _totalKeysCount - _localizedKeysCount;
        RefreshVisibleResources();
    }

    private bool IsResourceFullyLocalized(ResourceDto resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Id))
        {
            return false;
        }

        return _isResourceFullyLocalizedById.TryGetValue(resource.Id, out bool isLocalized) && isLocalized;
    }

    private string GetResourceLocalizationStatusText(ResourceDto resource)
    {
        return IsResourceFullyLocalized(resource)
            ? L.Localize("Resource_LocalizationStatus_Localized", "Localized")
            : L.Localize("Resource_LocalizationStatus_Outstanding", "Outstanding");
    }

    private BadgeStyle GetResourceLocalizationStatusBadgeStyle(ResourceDto resource)
    {
        return IsResourceFullyLocalized(resource)
            ? BadgeStyle.Success
            : BadgeStyle.Warning;
    }

    private void RefreshVisibleResources()
    {
        List<ResourceDto> resources = _response?.Data ?? [];

        _visibleResources = _showOutstandingOnly
            ? resources.Where(resource => !IsResourceFullyLocalized(resource)).ToList()
            : resources;
    }

    private async Task AddResourceAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateResource>(
            L.Localize("Dialog_AddResource", "Add Resource"),
            new Dictionary<string, object?> { { "ResourceInput", new ResourceDto() } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task UpdateResourceAsync(ResourceDto resource)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateResource>(
            L.Localize("Dialog_UpdateResource", "Update Resource {0}", resource.Key),
            new Dictionary<string, object?> { { "ResourceInput", resource } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task DeleteResourceAsync(ResourceDto resource)
    {
        bool? confirmed = await dialogService.Confirm(
            L.Localize("Dialog_ConfirmDeleteResource", "Are you sure you want to delete resource '{0}'?", resource.Key),
            L.Localize("Dialog_DeleteResource", "Delete Resource"),
            new ConfirmOptions
            {
                OkButtonText = L.Localize("Button_Delete", "Delete"),
                CancelButtonText = L.Localize("Button_Cancel", "Cancel")
            });

        if (confirmed == true && !string.IsNullOrWhiteSpace(resource.Id))
        {
            Response result = await api.CallAsync(
                () => localizationApi.DeleteResourceAsync(resource.Id),
                successMessage: L.Localize("Notification_ResourceDeletedSuccessfully", "Resource deleted successfully")).ConfigureAwait(false);

            if (!result.IsError)
            {
                await GetListAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task GenerateMissingTranslationsAsync(ResourceDto resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Id))
        {
            return;
        }

        _ = await api.CallAsync(
            () => localizationApi.GenerateMissingResourceTranslationsAsync(resource.Id),
            successMessage: L.Localize("Notification_ResourceTranslationGenerationQueued", "Translation generation queued.")).ConfigureAwait(false);
    }

    private async Task CheckResourceTranslationStatusAsync(ResourceDto resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Id))
        {
            return;
        }

        Response<LocalizationTranslationJobStatusDto> result = await api.CallAsync(
            () => localizationApi.GetResourceTranslationJobStatusAsync(resource.Id),
            errorTitle: L.Localize("Dialog_TranslationStatus", "Translation Status")).ConfigureAwait(false);

        if (result.IsError || result.Data is null)
        {
            return;
        }

        LocalizationTranslationJobStatusDto status = result.Data;
        string message = L.Localize(
            "Dialog_TranslationStatusBody",
            "State: {0}\nUpdated (UTC): {1}\nError: {2}",
            status.State,
            status.UpdatedOnUtc.ToString("u"),
            string.IsNullOrWhiteSpace(status.ErrorMessage) ? "-" : status.ErrorMessage);

        _ = await dialogService.Alert(
            message,
            L.Localize("Dialog_TranslationStatus", "Translation Status"));
    }

    private async Task CheckTranslationJobsHealthAsync()
    {
        Response<LocalizationTranslationJobHealthDto> result = await api.CallAsync(
            () => localizationApi.GetLocalizationTranslationJobsHealthAsync(),
            errorTitle: L.Localize("Dialog_TranslationJobHealth", "Translation Job Health")).ConfigureAwait(false);

        if (result.IsError || result.Data is null)
        {
            return;
        }

        LocalizationTranslationJobHealthDto health = result.Data;
        string message = L.Localize(
            "Dialog_TranslationJobHealthBody",
            "Tracked: {0}\nEnqueued: {1}\nExecuting: {2}\nCompleted: {3}\nFailed: {4}\nUpdated (UTC): {5}",
            health.TotalTracked,
            health.EnqueuedCount,
            health.ExecutingCount,
            health.CompletedCount,
            health.FailedCount,
            health.UpdatedOnUtc.ToString("u"));

        _ = await dialogService.Alert(
            message,
            L.Localize("Dialog_TranslationJobHealth", "Translation Job Health"));
    }

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, ResourceDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateResourceAsync(data).ConfigureAwait(false);
            return;
        }

        if (item is { Value: PermissionAction.Delete })
        {
            await DeleteResourceAsync(data).ConfigureAwait(false);
            return;
        }

        if (string.Equals(item?.Value?.ToString(), "generate-translations", StringComparison.OrdinalIgnoreCase))
        {
            await GenerateMissingTranslationsAsync(data).ConfigureAwait(false);
            return;
        }

        if (string.Equals(item?.Value?.ToString(), "translation-status", StringComparison.OrdinalIgnoreCase))
        {
            await CheckResourceTranslationStatusAsync(data).ConfigureAwait(false);
        }
    }
}
