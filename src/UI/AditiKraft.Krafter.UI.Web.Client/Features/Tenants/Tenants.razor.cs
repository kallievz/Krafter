using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Enums;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Tenants;

public partial class Tenants(
    ApiCallService api,
    ITenantsApi tenantsApi,
    DialogService dialogService,
    NavigationManager navigationManager,
    AppUrls urls
) : ComponentBase
{
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

    private async Task AddAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateTenant>($"Add New Tenant",
            new Dictionary<string, object?> { { "TenantInput", new TenantDto() } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task UpdateAsync(TenantDto tenant)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateTenant>($"Update Tenant {tenant.Name}",
            new Dictionary<string, object?> { { "TenantInput", tenant } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task DeleteAsync(TenantDto input)
    {
        bool? confirmed = await dialogService.Confirm(
            $"Are you sure you want to delete tenant '{input.Name}'?",
            "Delete Tenant",
            new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" });

        if (confirmed == true && !string.IsNullOrWhiteSpace(input.Id))
        {
            Response result = await api.CallAsync(
                () => tenantsApi.DeleteTenantAsync(input.Id),
                successMessage: "Tenant deleted successfully");

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
            await UpdateAsync(data);
        }
        else if (item is { Value: PermissionAction.Create })
        {
            await AddAsync();
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteAsync(data);
        }
    }
}
