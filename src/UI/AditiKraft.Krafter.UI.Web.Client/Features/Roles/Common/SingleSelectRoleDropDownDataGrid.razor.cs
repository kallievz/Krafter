namespace AditiKraft.Krafter.UI.Web.Client.Features.Roles.Common;

public partial class SingleSelectRoleDropDownDataGrid(
    ApiCallService api,
    IRolesApi rolesApi
) : ComponentBase
{
    [Inject] private Radzen.ILocalizer L { get; set; } = null!;

    private RadzenDropDownDataGrid<string> _dropDownGrid = null!;
    private Response<PaginationResponse<RoleDto>>? _response;
    private bool _isLoading = true;
    private IEnumerable<RoleDto>? _data;
    [Parameter] public GetRequestInput GetRequestInput { get; set; } = new();

    [Parameter] public string? Value { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public List<string> IdsToDisable { get; set; } = new();

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;
        await Task.Yield();
        GetRequestInput.SkipCount = args.Skip ?? 0;
        GetRequestInput.MaxResultCount = args.Top ?? 10;
        GetRequestInput.Filter = args.Filter;
        GetRequestInput.OrderBy = args.OrderBy;
        _isLoading = true;
        _response = await api.CallAsync(() => rolesApi.GetRolesAsync(GetRequestInput), showErrorNotification: true);
        if (_response is { Data.Items: not null })
        {
            _data = _response.Data.Items.Where(c => !IdsToDisable.Contains(c.Id)).ToList();
        }

        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private int GetCount()
    {
        if (_response is { Data: { Items: not null, TotalCount: var totalCount } })
        {
            return totalCount;
        }

        return 0;
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
