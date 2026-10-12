using AditiKraft.Krafter.Contracts.Common;
using Mapster;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Tenants;

public partial class CreateOrUpdateTenant(
    DialogService dialogService,
    ApiCallService api,
    ITenantsApi tenantsApi) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    [Parameter] public TenantDto? TenantInput { get; set; } = new();
    private CreateOrUpdateTenantRequest _createRequest = new();
    private bool _isBusy;
    private string? _dateError;
    private bool IsRoot => TenantInput?.Id == DefaultTenantConstants.Identifier;

    protected override void OnInitialized()
    {
        _createRequest = TenantInput?.Adapt<CreateOrUpdateTenantRequest>() ?? new();
        if (string.IsNullOrWhiteSpace(_createRequest.Id))
        {
            _createRequest.ValidUpto = null;
        }
    }

    private void ChangeLocalExpiry(DateTime? value)
    {
        if (IsRoot)
        {
            return;
        }

        _dateError = null;
        // Keep the original Local value, including its daylight-saving offset, when unchanged.
        if (value == _createRequest.ValidUpto)
        {
            return;
        }

        if (value is { } local && (TimeZoneInfo.Local.IsInvalidTime(local) || TimeZoneInfo.Local.IsAmbiguousTime(local)))
        {
            _createRequest.ValidUpto = null;
            _dateError = "This time is skipped or repeated when the clocks change. Choose another time.";
            return;
        }
        _createRequest.ValidUpto = value;
    }

    private async Task SubmitAsync(CreateOrUpdateTenantRequest input)
    {
        if (_isBusy || _dateError is not null)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response result = string.IsNullOrWhiteSpace(input.Id)
                ? await api.CallAsync(() => tenantsApi.CreateTenantAsync(input), successMessage: "Tenant created successfully")
                : await api.CallAsync(() => tenantsApi.UpdateTenantAsync(input.Id, input), successMessage: "Tenant updated successfully");
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
