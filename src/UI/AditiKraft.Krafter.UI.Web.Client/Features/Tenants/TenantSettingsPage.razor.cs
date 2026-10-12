using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Tenants;

public partial class TenantSettingsPage(
    ITenantsApi tenantsApi,
    ApiCallService api,
    NavigationManager navigationManager) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    public const string RoutePath = "/tenants/{tenantId}/settings";

    [Parameter] public string TenantId { get; set; } = string.Empty;

    private bool _isLoading = true;
    private TenantDto? _tenant;

    protected override async Task OnParametersSetAsync()
    {
        _isLoading = true;

        if (!string.IsNullOrWhiteSpace(TenantId))
        {
            Response<PaginationResponse<TenantDto>> response = await api.CallAsync(
                () => tenantsApi.GetTenantsAsync(new GetRequestInput
                {
                    Id = TenantId,
                    MaxResultCount = 1,
                    SkipCount = 0
                }),
                showErrorNotification: true);

            _tenant = response.Data?.Items?.FirstOrDefault();
            if (_tenant is not null)
            {
                LocalAppState.CurrentPageTitle = $"Tenant Settings - {_tenant.Name}";
            }
        }

        _isLoading = false;
    }

    private Task BackAsync()
    {
        navigationManager.NavigateTo($"/{ApiRoutes.Tenants}");
        return Task.CompletedTask;
    }
}
