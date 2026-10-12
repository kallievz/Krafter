using Mapster;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Users;

public partial class CreateOrUpdateUser(
    DialogService dialogService,
    ApiCallService api,
    IUsersApi usersApi
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    [Parameter] public UserDto UserInput { get; set; } = new();

    private CreateUserRequest _createUserRequest = new();
    private CreateUserRequest _originalCreateUserRequest = new();
    public Response<List<UserRoleDto>>? UserRoles { get; set; }
    private bool _isBusy = false;

    protected override async Task OnInitializedAsync()
    {
        if (UserInput is not null)
        {
            _createUserRequest = UserInput.Adapt<CreateUserRequest>();
            _originalCreateUserRequest = UserInput.Adapt<CreateUserRequest>();
            if (_createUserRequest.Roles is null)
            {
                _createUserRequest.Roles = new List<string>();
                _originalCreateUserRequest.Roles = new List<string>();
            }

            if (!string.IsNullOrWhiteSpace(UserInput.Id))
            {
                UserRoles = await api.CallAsync(() => usersApi.GetUserRolesAsync(UserInput.Id),
                    showErrorNotification: true);
                _createUserRequest.Roles = UserRoles?
                    .Data?
                    .Where(c => !string.IsNullOrEmpty(c.RoleId))
                    .Select(c => c.RoleId!).ToList() ?? new List<string>();
                _originalCreateUserRequest.Roles = _createUserRequest.Roles;
            }
        }
    }

    private async Task SubmitAsync(CreateUserRequest input)
    {
        if (UserInput is not null)
        {
            _isBusy = true;
            Response result;
            if (string.IsNullOrWhiteSpace(input.Id))
            {
                result = await api.CallAsync(
                    () => usersApi.CreateUserAsync(input),
                    successMessage: "User created successfully");
            }
            else
            {
                result = await api.CallAsync(
                    () => usersApi.UpdateUserAsync(input.Id, input),
                    successMessage: "User updated successfully");
            }
            _isBusy = false;
            StateHasChanged();
            if (result is { IsError: false })
            {
                dialogService.Close(true);
            }
        }
        else
        {
            dialogService.Close(false);
        }
    }

    private void Cancel() => dialogService.Close();
}
