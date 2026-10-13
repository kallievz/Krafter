using AditiKraft.Krafter.Contracts.Contracts.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Localization;

public partial class CreateOrUpdateResource(
    DialogService dialogService,
    ApiCallService api,
    ILocalizationApi localizationApi) : ComponentBase
{
    [Inject]
    private ILocalizer L { get; set; } = null!;
    [Parameter] public ResourceDto ResourceInput { get; set; } = new();

    private CreateResourceRequest _request = new();
    private bool _isBusy;

    protected override void OnInitialized()
    {
        _request = new CreateResourceRequest
        {
            Key = ResourceInput.Key,
            Description = ResourceInput.Description
        };
    }

    private async Task SubmitAsync(CreateResourceRequest input)
    {
        if (_isBusy)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response<ResourceDto> result = string.IsNullOrWhiteSpace(ResourceInput.Id)
                ? await api.CallAsync(
                    () => localizationApi.CreateResourceAsync(input),
                    successMessage: L.Localize("Notification_ResourceCreatedSuccessfully", "Resource created successfully")).ConfigureAwait(false)
                : await api.CallAsync(
                    () => localizationApi.UpdateResourceAsync(ResourceInput.Id, new UpdateResourceRequest
                    {
                        Key = input.Key,
                        Description = input.Description
                    }),
                    successMessage: L.Localize("Notification_ResourceUpdatedSuccessfully", "Resource updated successfully")).ConfigureAwait(false);

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
