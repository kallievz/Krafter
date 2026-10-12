using Mapster;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Tenants;

public partial class ManageTenantSettings(
    ITenantsApi tenantsApi,
    ApiCallService api,
    DialogService dialogService) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    [Parameter] public TenantDto? TenantInput { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = true;

    private List<TenantSettingDto> _settings = [];
    private UpsertTenantSettingRequest _request = new();
    private bool _isLoading;
    private bool _isBusy;
    private bool _isEditing;

    protected override async Task OnInitializedAsync()
    {
        await LoadSettingsAsync();
    }

    private async Task LoadSettingsAsync()
    {
        if (TenantInput?.Id is null)
        {
            return;
        }

        _isLoading = true;
        Response<List<TenantSettingDto>> response =
            await api.CallAsync(() => tenantsApi.GetTenantSettingsAsync(TenantInput.Id), showErrorNotification: true);
        if (!response.IsError && response.Data is not null)
        {
            _settings = response.Data;
        }

        _isLoading = false;
    }

    private Task BeginAddAsync()
    {
        _request = new UpsertTenantSettingRequest();
        _isEditing = true;
        return Task.CompletedTask;
    }

    private Task BeginEditAsync(TenantSettingDto setting)
    {
        _request = setting.Adapt<UpsertTenantSettingRequest>();
        _isEditing = true;
        return Task.CompletedTask;
    }

    private async Task SubmitAsync(UpsertTenantSettingRequest request)
    {
        if (_isBusy || TenantInput?.Id is null)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response result = await api.CallAsync(
                () => tenantsApi.UpsertTenantSettingAsync(TenantInput.Id, request),
                successMessage: "Tenant setting saved successfully");

            if (!result.IsError)
            {
                _isEditing = false;
                _request = new UpsertTenantSettingRequest();
                await LoadSettingsAsync();
            }
        }
        finally
        {
            _isBusy = false;
        }
    }

    private Task CancelEditAsync()
    {
        _isEditing = false;
        _request = new UpsertTenantSettingRequest();
        return Task.CompletedTask;
    }

    private Task CloseAsync()
    {
        dialogService.Close(true);
        return Task.CompletedTask;
    }
}
