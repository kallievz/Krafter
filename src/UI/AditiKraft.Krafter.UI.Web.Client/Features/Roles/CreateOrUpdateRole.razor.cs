using Mapster;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Roles;

public partial class CreateOrUpdateRole(
    DialogService dialogService,
    ApiCallService api,
    IRolesApi rolesApi) : ComponentBase
{
    public class GroupPermissionData
    {
        public string Description { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Resource { get; set; } = default!;
        public string FinalPermission { get; set; } = default!;
        public bool IsBasic { get; set; }
        public bool IsRoot { get; set; }

        public bool IsGroup => Resource != null;
    }

    [Parameter] public RoleDto UserDetails { get; set; } = new();
    private CreateOrUpdateRoleRequest _createUserRequest = new();
    private CreateOrUpdateRoleRequest _originalCreateUserRequest = new();

    public List<PermissionDefinition> AllRoles { get; set; } = default!;

    private IEnumerable<GroupPermissionData> _groupedData = new List<GroupPermissionData>();

    private bool _isBusy = false;
    private bool _permissionsLoaded;
    private bool _isLoadingPermissions;
    private bool _permissionLoadFailed;

    private bool CanSave => !_isBusy &&
        (string.IsNullOrWhiteSpace(UserDetails.Id) || _permissionsLoaded);

    protected override async Task OnInitializedAsync()
    {
        if (UserDetails is not null)
        {
            _createUserRequest = UserDetails.Adapt<CreateOrUpdateRoleRequest>();
            _originalCreateUserRequest = UserDetails.Adapt<CreateOrUpdateRoleRequest>();
            if (!string.IsNullOrWhiteSpace(UserDetails.Id))
            {
                _groupedData = PermissionCatalog.All.GroupBy(c => c.Resource)
                    .SelectMany(i => new GroupPermissionData[] { new() { Resource = i.Key } }
                        .Concat(i.Select(o =>
                            new GroupPermissionData
                            {
                                Description = o.Description,
                                Action = o.Action,
                                IsBasic = o.IsBasic,
                                IsRoot = o.IsRoot,
                                FinalPermission = PermissionDefinition.NameFor(o.Action, o.Resource)
                            }))).ToList();
                await LoadPermissionsAsync();
            }
        }
    }

    private async Task LoadPermissionsAsync()
    {
        _permissionsLoaded = false;
        _permissionLoadFailed = false;
        _isLoadingPermissions = true;
        try
        {
            Response<RoleDto> response = await api.CallAsync(
                () => rolesApi.GetRolePermissionsAsync(UserDetails.Id),
                showErrorNotification: true);
            if (response is not { IsError: false, Data.Permissions: not null })
            {
                _permissionLoadFailed = true;
                return;
            }

            _createUserRequest.Permissions = [.. response.Data.Permissions];
            _originalCreateUserRequest.Permissions = [.. response.Data.Permissions];
            _permissionsLoaded = true;
        }
        finally
        {
            _isLoadingPermissions = false;
        }
    }

    private async Task SubmitAsync(CreateOrUpdateRoleRequest input)
    {
        if (!CanSave)
        {
            return;
        }

        _isBusy = true;
        try
        {
            Response result;
            if (string.IsNullOrWhiteSpace(input.Id))
            {
                result = await api.CallAsync(
                    () => rolesApi.CreateRoleAsync(input),
                    successMessage: "Role created successfully");
            }
            else
            {
                result = await api.CallAsync(
                    () => rolesApi.UpdateRoleAsync(input.Id, input),
                    successMessage: "Role updated successfully");
            }

            if (result is { IsError: false })
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
