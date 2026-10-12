using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class CreateOrUpdateResourceTranslation(
    DialogService dialogService,
    ApiCallService api,
    ILocalizationApi localizationApi) : ComponentBase
{
    [Inject] private Radzen.ILocalizer L { get; set; } = null!;

    [Parameter] public ResourceTranslationDto ResourceTranslationInput { get; set; } = new();

    private CreateResourceTranslationRequest _request = new();
    private List<ResourceDto> _resources = [];
    private List<LanguageDto> _languages = [];
    private bool _isBusy;
    private bool _isLookupLoading = true;

    protected override async Task OnInitializedAsync()
    {
        _request = new CreateResourceTranslationRequest
        {
            ResourceId = ResourceTranslationInput.ResourceId,
            LanguageId = ResourceTranslationInput.LanguageId,
            Value = ResourceTranslationInput.Value
        };

        await LoadLookupsAsync().ConfigureAwait(false);
    }

    private async Task LoadLookupsAsync()
    {
        _isLookupLoading = true;
        try
        {
            Response<List<ResourceDto>> resourcesResponse = await api.CallAsync(
                () => localizationApi.GetResourcesAsync(),
                showErrorNotification: true).ConfigureAwait(false);
            Response<List<LanguageDto>> languagesResponse = await api.CallAsync(
                () => localizationApi.GetLanguagesAsync(),
                showErrorNotification: true).ConfigureAwait(false);

            _resources = resourcesResponse.Data ?? [];
            _languages = (languagesResponse.Data ?? []).Where(language => language.IsActive).ToList();
        }
        finally
        {
            _isLookupLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task SubmitAsync(CreateResourceTranslationRequest input)
    {
        if (_isBusy || _isLookupLoading)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response<ResourceTranslationDto> result = string.IsNullOrWhiteSpace(ResourceTranslationInput.Id)
                ? await api.CallAsync(
                    () => localizationApi.CreateResourceTranslationAsync(input),
                    successMessage: L.Localize("Notification_ResourceTranslationCreatedSuccessfully", "Resource translation created successfully")).ConfigureAwait(false)
                : await api.CallAsync(
                    () => localizationApi.UpdateResourceTranslationAsync(ResourceTranslationInput.Id, new UpdateResourceTranslationRequest
                    {
                        ResourceId = input.ResourceId,
                        LanguageId = input.LanguageId,
                        Value = input.Value
                    }),
                    successMessage: L.Localize("Notification_ResourceTranslationUpdatedSuccessfully", "Resource translation updated successfully")).ConfigureAwait(false);

            if (!result.IsError)
            {
                dialogService.Close(true);
            }
        }
        finally
        {
            _isBusy = false;
        }
    }

    private void Cancel() => dialogService.Close(false);
}
