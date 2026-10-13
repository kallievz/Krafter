using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class ResourceTranslations(
    ApiCallService api,
    ILocalizationApi localizationApi,
    DialogService dialogService
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    public const string RoutePath = ApiRoutes.LocalizationResourceTranslations;

    private bool _isLoading = true;

    private Response<List<ResourceTranslationDto>>? _response = new() { Data = [] };

    protected override async Task OnInitializedAsync()
    {
        LocalAppState.CurrentPageTitle = L.Localize("Nav_ResourceTranslations", "Resource Translations");
        await GetListAsync().ConfigureAwait(false);
    }

    private async Task GetListAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _response = await api.CallAsync(
            () => localizationApi.GetResourceTranslationsAsync(),
            errorTitle: L.Localize("Entity_ResourceTranslations", "Resource Translations")).ConfigureAwait(false);

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddResourceTranslationAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateResourceTranslation>(
            L.Localize("Dialog_AddResourceTranslation", "Add Resource Translation"),
            new Dictionary<string, object?> { { "ResourceTranslationInput", new ResourceTranslationDto() } },
            new DialogOptions { Width = "55vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task UpdateResourceTranslationAsync(ResourceTranslationDto resourceTranslation)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateResourceTranslation>(
            L.Localize("Dialog_UpdateResourceTranslation", "Update Resource Translation"),
            new Dictionary<string, object?> { { "ResourceTranslationInput", resourceTranslation } },
            new DialogOptions { Width = "55vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task DeleteResourceTranslationAsync(ResourceTranslationDto resourceTranslation)
    {
        bool? confirmed = await dialogService.Confirm(
            L.Localize("Dialog_ConfirmDeleteResourceTranslation", "Are you sure you want to delete this resource translation?"),
            L.Localize("Dialog_DeleteResourceTranslation", "Delete Resource Translation"),
            new ConfirmOptions
            {
                OkButtonText = L.Localize("Button_Delete", "Delete"),
                CancelButtonText = L.Localize("Button_Cancel", "Cancel")
            });

        if (confirmed == true && !string.IsNullOrWhiteSpace(resourceTranslation.Id))
        {
            Response result = await api.CallAsync(
                () => localizationApi.DeleteResourceTranslationAsync(resourceTranslation.Id),
                successMessage: L.Localize("Notification_ResourceTranslationDeletedSuccessfully", "Resource translation deleted successfully")).ConfigureAwait(false);

            if (!result.IsError)
            {
                await GetListAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, ResourceTranslationDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateResourceTranslationAsync(data).ConfigureAwait(false);
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteResourceTranslationAsync(data).ConfigureAwait(false);
        }
    }
}
