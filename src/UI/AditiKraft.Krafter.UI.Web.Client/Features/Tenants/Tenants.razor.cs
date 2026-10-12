using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Enums;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Tenants;

public partial class Tenants(
    ApiCallService api,
    ITenantsApi tenantsApi,
    DialogService dialogService,
    NavigationManager navigationManager,
    AppUrls urls
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    public const string RoutePath = ApiRoutes.Tenants;
    private bool _isLoading = true;
    private readonly GetRequestInput _requestInput = new();

    private Response<PaginationResponse<TenantDto>>? _response = new() { Data = new PaginationResponse<TenantDto>() };

    protected override async Task OnInitializedAsync()
    {
        if (TenantSettings.TenancyMode == TenancyMode.Single)
        {
            navigationManager.NavigateTo("/not-found");
            return;
        }

        LocalAppState.CurrentPageTitle = $"Tenants";

        await GetListAsync();
    }

    private async Task GetListAsync(bool resetPaginationData = false)
    {
        _isLoading = true;
        if (resetPaginationData)
        {
            _requestInput.SkipCount = 0;
        }

        _response = await api.CallAsync(() => tenantsApi.GetTenantsAsync(_requestInput));
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddTenantAsync()
    {
        string title = L.Localize("Dialog_AddNewTenant", "Add New Tenant");
        object? result = await dialogService.OpenAsync<CreateOrUpdateTenant>(title,
            new Dictionary<string, object?> { { "TenantInput", new TenantDto() } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task UpdateTenantAsync(TenantDto tenant)
    {
        string title = L.Localize("Dialog_UpdateTenant", "Update Tenant {0}", tenant.Name ?? string.Empty);
        object? result = await dialogService.OpenAsync<CreateOrUpdateTenant>(title,
            new Dictionary<string, object?> { { "TenantInput", tenant } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task ManageSettingsAsync(TenantDto tenant)
    {
        string title = L.Localize("Dialog_TenantSettings", "Tenant Settings - {0}", tenant.Name ?? string.Empty);
        _ = await dialogService.OpenAsync<ManageTenantSettings>(title,
            new Dictionary<string, object?>
            {
                { "TenantInput", tenant },
                { "ShowCloseButton", true }
            },
            new DialogOptions { Width = "65vw", Resizable = true, Draggable = true, Top = "5vh" });
    }

    private void NavigateToSettingsPage(TenantDto tenant)
    {
        if (string.IsNullOrWhiteSpace(tenant.Id))
        {
            return;
        }

        navigationManager.NavigateTo($"/{ApiRoutes.Tenants}/{tenant.Id}/settings");
    }

    private async Task DeleteTenantAsync(TenantDto input)
    {
        bool? confirmed = await dialogService.Confirm(
            L.Localize("Dialog_ConfirmDeleteTenant", "Are you sure you want to delete tenant '{0}'?", input.Name ?? string.Empty),
            L.Localize("Dialog_DeleteTenantTitle", "Delete Tenant"),
            new ConfirmOptions
            {
                OkButtonText = L.Localize("Button_Delete", "Delete"),
                CancelButtonText = L.Localize("Button_Cancel", "Cancel")
            });

        if (confirmed == true && !string.IsNullOrWhiteSpace(input.Id))
        {
            Response result = await api.CallAsync(
                () => tenantsApi.DeleteTenantAsync(input.Id),
                successMessage: L.Localize("Notification_TenantDeletedSuccessfully", "Tenant deleted successfully"));

            if (!result.IsError)
            {
                await GetListAsync();
            }
        }
    }

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await Task.Yield();
        _requestInput.SkipCount = args.Skip ?? 0;
        _requestInput.MaxResultCount = args.Top ?? 10;
        _requestInput.Filter = args.Filter;
        _requestInput.OrderBy = args.OrderBy;
        await GetListAsync();
    }

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, TenantDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateTenantAsync(data);
        }
        else if (item is { Value: PermissionAction.Create })
        {
            await AddTenantAsync();
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteTenantAsync(data);
        }
        else if (item is { Value: "settings" })
        {
            await ManageSettingsAsync(data);
        }
        else if (item is { Value: "settings-page" })
        {
            NavigateToSettingsPage(data);
        }
    }
}
