using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Roles;

public partial class Roles(
    ApiCallService api,
    IRolesApi rolesApi,
    DialogService dialogService) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
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
        string title = L.Localize("Dialog_AddNewRole", "Add New Role");
        object? result = await dialogService.OpenAsync<CreateOrUpdateRole>(title,
            new Dictionary<string, object?> { { "UserDetails", new RoleDto() } },
            new DialogOptions { Width = "50vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task UpdateRoleAsync(RoleDto user)
    {
        string title = L.Localize("Dialog_UpdateRole", "Update Role {0}", user.Name ?? string.Empty);
        object? result = await dialogService.OpenAsync<CreateOrUpdateRole>(title,
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
            L.Localize("Dialog_ConfirmDeleteRole", "Are you sure you want to delete role '{0}'?", roleDto.Name ?? string.Empty),
            L.Localize("Dialog_DeleteRoleTitle", "Delete Role"),
            new ConfirmOptions
            {
                OkButtonText = L.Localize("Button_Delete", "Delete"),
                CancelButtonText = L.Localize("Button_Cancel", "Cancel")
            });

        if (confirmed == true)
        {
            Response result = await api.CallAsync(
                () => rolesApi.DeleteRoleAsync(roleDto.Id),
                successMessage: L.Localize("Notification_RoleDeletedSuccessfully", "Role deleted successfully"));

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
