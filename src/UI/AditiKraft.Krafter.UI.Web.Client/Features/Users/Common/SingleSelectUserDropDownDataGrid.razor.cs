namespace AditiKraft.Krafter.UI.Web.Client.Features.Users.Common;

public partial class SingleSelectUserDropDownDataGrid(
    ApiCallService api,
    IUsersApi usersApi
) : ComponentBase
{
    private RadzenDropDownDataGrid<string> _dropDownGrid = default!;
    private int _totalCount = 0;
    private bool _isLoading = true;
    private IEnumerable<UserInfo>? _data;
    [Parameter] public GetRequestInput GetRequestInput { get; set; } = new();

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public List<string> IdsToDisable { get; set; } = new();
    [Parameter] public string? RoleId { get; set; }

    private async Task LoadProcessesAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await Task.Yield();
        GetRequestInput.SkipCount = args.Skip ?? 0;
        GetRequestInput.MaxResultCount = args.Top ?? 10;
        GetRequestInput.Filter = args.Filter;
        GetRequestInput.OrderBy = args.OrderBy;
        _isLoading = true;
        if (!string.IsNullOrWhiteSpace(RoleId))
        {
            Response<PaginationResponse<UserInfo>> response = await api.CallAsync(() => usersApi.GetUsersByRoleAsync(
                RoleId, GetRequestInput), showErrorNotification: true);
            if (response is { Data.Items: not null })
            {
                _data = response.Data.Items.Where(c => !IdsToDisable.Contains(c.Id ?? "")).ToList();
                _totalCount = response.Data.TotalCount;
            }
        }
        else
        {
            Response<PaginationResponse<UserDto>> response = await api.CallAsync(() => usersApi.GetUsersAsync(
                GetRequestInput), showErrorNotification: true);
            if (response is { Data.Items: not null })
            {
                _totalCount = response.Data.TotalCount;
                _data = response.Data.Items.Where(c => !IdsToDisable.Contains(c.Id ?? "")).Select(c => new UserInfo
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    CreatedOn = c.CreatedOn
                }).ToList();
            }
        }

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnValueChangedAsync(object newValue)
    {
        if (newValue is string newValueEnumerable)
        {
            Value = newValueEnumerable;
            await ValueChanged.InvokeAsync(newValueEnumerable);
        }
        else
        {
            Console.WriteLine("Invalid value type");
        }
    }
}
