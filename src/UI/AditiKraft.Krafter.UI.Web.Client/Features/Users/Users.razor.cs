using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Users;

public partial class Users(
    DialogService dialogService,
    ApiCallService api,
    IUsersApi usersApi
) : ComponentBase
{
    public const string RoutePath = ApiRoutes.Users;
    private readonly GetRequestInput _requestInput = new();

    private Response<PaginationResponse<UserDto>>? _response = new() { Data = new PaginationResponse<UserDto>() };

    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        LocalAppState.CurrentPageTitle = "Users";

        await GetListAsync();
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

    private async Task GetListAsync(bool resetPaginationData = false)
    {
        _isLoading = true;
        if (resetPaginationData)
        {
            _requestInput.SkipCount = 0;
        }

        _response = await api.CallAsync(() => usersApi.GetUsersAsync(_requestInput));
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task AddUserAsync()
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateUser>($"Add New User",
            new Dictionary<string, object?> { { "UserInput", new UserDto() } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task UpdateUserAsync(UserDto user)
    {
        object? result = await dialogService.OpenAsync<CreateOrUpdateUser>($"Update User {user.FirstName}",
            new Dictionary<string, object?> { { "UserInput", user } },
            new DialogOptions { Width = "40vw", Resizable = true, Draggable = true, Top = "5vh" });
        if (result is true)
        {
            await GetListAsync();
        }
    }

    private async Task DeleteUserAsync(UserDto user)
    {
        bool? confirmed = await dialogService.Confirm(
            $"Are you sure you want to delete user '{user.FirstName} {user.LastName}'?",
            "Delete User",
            new ConfirmOptions { OkButtonText = "Delete", CancelButtonText = "Cancel" });

        if (confirmed == true)
        {
            Response result = await api.CallAsync(
                () => usersApi.DeleteUserAsync(user.Id),
                successMessage: "User deleted successfully");

            if (!result.IsError)
            {
                await GetListAsync();
            }
        }
    }

    private async Task ActionClickedAsync(RadzenSplitButtonItem? item, UserDto data)
    {
        if (item is { Value: PermissionAction.Update })
        {
            await UpdateUserAsync(data);
        }
        else if (item is { Value: PermissionAction.Create })
        {
            await AddUserAsync();
        }
        else if (item is { Value: PermissionAction.Delete })
        {
            await DeleteUserAsync(data);
        }
    }
}
