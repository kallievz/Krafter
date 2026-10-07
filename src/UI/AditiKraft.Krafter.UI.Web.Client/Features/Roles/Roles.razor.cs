using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Roles;

public partial class Roles(
    ApiCallService api,
    IRolesApi rolesApi,
    DialogService dialogService) : ComponentBase
{
    public const string RoutePath = ApiRoutes.Roles;
    private bool _isLoading = true;

    private readonly GetRequestInput _requestInput = new();
    public string IdentifierBasedOnPlacement = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        IdentifierBasedOnPlacement = nameof(Roles);

        LocalAppState.CurrentPageTitle = $"Roles";

        await GetListAsync();
    }

    private Response<PaginationResponse<RoleDto>>? _response = new() { Data = new PaginationResponse<RoleDto>() };

    private async Task GetListAsync(bool resetPaginationData = false)
    {
        _isLoading = true;
        if (resetPaginationData)
        {
            _requestInput.SkipCount = 0;
        }

        _response = await api.CallAsync(() => rolesApi.GetRolesAsync(_requestInput));

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddRoleAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateRole>($"Add New Role",
            new Dictionary<string, object?> { { "UserDetails", new RoleDto() } },
            new DialogOptions { Width = "50vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task UpdateRoleAsync(RoleDto user)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateRole>($"Update Role {user.Name}",
            new Dictionary<string, object?> { { "UserDetails", user } },
            new DialogOptions { Width = "50vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task DeleteRoleAsync(RoleDto roleDto)
    {
        bool? confirmed = await dialogService.Confirm(
            $"Are you sure you want to delete role '{roleDto.Name}'?",
            "Delete Role",
            new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" });

        if (confirmed == true)
        {
            Response result = await api.CallAsync(
                () => rolesApi.DeleteRoleAsync(roleDto.Id),
                successMessage: "Role deleted successfully");

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

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, RoleDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateRoleAsync(data);
        }
        else if (item is { Value: PermissionAction.Create })
        {
            await AddRoleAsync();
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteRoleAsync(data);
        }
    }
}
