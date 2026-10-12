using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class Languages(
    ApiCallService api,
    ILocalizationApi localizationApi,
    DialogService dialogService
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;

    public const string RoutePath = ApiRoutes.LocalizationLanguages;

    private bool _isLoading = true;

    private Response<List<LanguageDto>>? _response = new() { Data = [] };

    protected override async Task OnInitializedAsync()
    {
        LocalAppState.CurrentPageTitle = L.Localize("Nav_Languages", "Languages");
        await GetListAsync().ConfigureAwait(false);
    }

    private async Task GetListAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        _response = await api.CallAsync(
            () => localizationApi.GetLanguagesAsync(),
            errorTitle: L.Localize("Entity_Languages", "Languages")).ConfigureAwait(false);

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddLanguageAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateLanguage>(
            L.Localize("Dialog_AddLanguage", "Add Language"),
            new Dictionary<string, object?> { { "LanguageInput", new LanguageDto { IsActive = true } } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task UpdateLanguageAsync(LanguageDto language)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateLanguage>(
            L.Localize("Dialog_UpdateLanguage", "Update Language {0}", language.Name),
            new Dictionary<string, object?> { { "LanguageInput", language } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });

        if (result is true)
        {
            await GetListAsync().ConfigureAwait(false);
        }
    }

    private async Task DeleteLanguageAsync(LanguageDto language)
    {
        bool? confirmed = await dialogService.Confirm(
            L.Localize("Dialog_ConfirmDeleteLanguage", "Are you sure you want to delete language '{0}'?", language.Name),
            L.Localize("Dialog_DeleteLanguage", "Delete Language"),
            new ConfirmOptions
            {
                OkButtonText = L.Localize("Button_Delete", "Delete"),
                CancelButtonText = L.Localize("Button_Cancel", "Cancel")
            });

        if (confirmed == true && !string.IsNullOrWhiteSpace(language.Id))
        {
            Response result = await api.CallAsync(
                () => localizationApi.DeleteLanguageAsync(language.Id),
                successMessage: L.Localize("Notification_LanguageDeletedSuccessfully", "Language deleted successfully")).ConfigureAwait(false);

            if (!result.IsError)
            {
                await GetListAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, LanguageDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateLanguageAsync(data).ConfigureAwait(false);
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteLanguageAsync(data).ConfigureAwait(false);
        }
    }
}
