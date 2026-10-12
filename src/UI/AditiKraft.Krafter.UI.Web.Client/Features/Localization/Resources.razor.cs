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
    private Radzen.ILocalizer L { get; set; } = null!;

    public const string RoutePath = ApiRoutes.LocalizationResources;

    private bool _isLoading = true;

    private Response<List<ResourceDto>>? _response = new() { Data = [] };

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

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
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

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, ResourceDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateResourceAsync(data).ConfigureAwait(false);
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteResourceAsync(data).ConfigureAwait(false);
        }
    }
}
