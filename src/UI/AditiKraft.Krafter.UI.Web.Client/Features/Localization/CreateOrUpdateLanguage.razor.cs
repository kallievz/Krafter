using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class CreateOrUpdateLanguage(
    DialogService dialogService,
    ApiCallService api,
    ILocalizationApi localizationApi) : ComponentBase
{
    [Inject]
    private ILocalizer L { get; set; } = null!;
    [Parameter] public LanguageDto LanguageInput { get; set; } = new();

    private CreateLanguageRequest _request = new();
    private bool _isBusy;

    protected override void OnInitialized()
    {
        _request = new CreateLanguageRequest
        {
            Code = LanguageInput.Code,
            Name = LanguageInput.Name,
            IsActive = string.IsNullOrWhiteSpace(LanguageInput.Id) || LanguageInput.IsActive
        };
    }

    private async Task SubmitAsync(CreateLanguageRequest input)
    {
        if (_isBusy)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response<LanguageDto> result = string.IsNullOrWhiteSpace(LanguageInput.Id)
                ? await api.CallAsync(
                    () => localizationApi.CreateLanguageAsync(input),
                    successMessage: L.Localize("Notification_LanguageCreatedSuccessfully", "Language created successfully")).ConfigureAwait(false)
                : await api.CallAsync(
                    () => localizationApi.UpdateLanguageAsync(LanguageInput.Id, new UpdateLanguageRequest
                    {
                        Code = input.Code,
                        Name = input.Name,
                        IsActive = input.IsActive
                    }),
                    successMessage: L.Localize("Notification_LanguageUpdatedSuccessfully", "Language updated successfully")).ConfigureAwait(false);

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
