using AditiKraft.Krafter.Contracts.Contracts.Grids;
using AditiKraft.Krafter.UI.Web.Client.Features.GridStates;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Layout;

public partial class GridStateManager(
    IGridStatesApi gridStatesApi,
    ApiCallService api,
    DialogService dialogService) : ComponentBase
{
    [Inject]
    private ILocalizer L { get; set; } = null!;
    [Parameter] public string ComponentKey { get; set; } = string.Empty;
    [Parameter] public string CurrentStateJson { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> OnApplyStateJson { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public int ResetVersion { get; set; }

    private List<GridStateDto> _states = [];
    private string? _selectedStateId;
    private string _stateName = string.Empty;
    private bool _isDefault;
    private bool _isBusy;
    private bool _initialStateApplied;
    private int _lastResetVersion;


    protected override async Task OnParametersSetAsync()
    {
        if (_lastResetVersion != ResetVersion)
        {
            _lastResetVersion = ResetVersion;
            _selectedStateId = null;
            _stateName = string.Empty;
            _isDefault = false;
            _initialStateApplied = true;
            await InvokeAsync(StateHasChanged);
            return;
        }

        if (_states.Count > 0 || string.IsNullOrWhiteSpace(ComponentKey))
        {
            return;
        }

        await LoadStatesAsync().ConfigureAwait(false);
    }

    private async Task LoadStatesAsync()
    {
        if (string.IsNullOrWhiteSpace(ComponentKey))
        {
            return;
        }

        _isBusy = true;
        Response<List<GridStateDto>> response = await api.CallAsync(
            () => gridStatesApi.GetGridStatesAsync(ComponentKey),
            errorTitle: L.Localize("Entity_GridStates", "Grid states")).ConfigureAwait(false);

        if (!response.IsError && response.Data is not null)
        {
            _states = response.Data;
            await ApplyDefaultStateOnceAsync().ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(_selectedStateId))
            {
                GridStateDto? selectedState = _states.FirstOrDefault(state => state.Id == _selectedStateId);
                if (selectedState is null)
                {
                    _selectedStateId = null;
                    _stateName = string.Empty;
                    _isDefault = false;
                }
            }
        }

        _isBusy = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ApplyDefaultStateOnceAsync()
    {
        if (_initialStateApplied)
        {
            return;
        }

        GridStateDto? defaultState = _states.FirstOrDefault(state => state.IsDefault);
        if (defaultState is null)
        {
            _initialStateApplied = true;
            return;
        }

        _selectedStateId = defaultState.Id;
        _stateName = defaultState.Name;
        _isDefault = defaultState.IsDefault;

        if (OnApplyStateJson.HasDelegate)
        {
            await OnApplyStateJson.InvokeAsync(defaultState.StateJson).ConfigureAwait(false);
        }

        _initialStateApplied = true;
    }

    private async Task OnSelectionChangedAsync(object value)
    {
        string? selectedId = value?.ToString();
        if (string.IsNullOrWhiteSpace(selectedId))
        {
            return;
        }

        GridStateDto? selectedState = _states.FirstOrDefault(state => state.Id == selectedId);
        if (selectedState is null)
        {
            return;
        }

        _selectedStateId = selectedState.Id;
        _stateName = selectedState.Name;
        _isDefault = selectedState.IsDefault;

        if (OnApplyStateJson.HasDelegate)
        {
            await OnApplyStateJson.InvokeAsync(selectedState.StateJson).ConfigureAwait(false);
        }
    }

    private async Task SaveStateAsync()
    {
        if (_isBusy || string.IsNullOrWhiteSpace(ComponentKey) || string.IsNullOrWhiteSpace(_stateName))
        {
            return;
        }

        _isBusy = true;
        CreateGridStateRequest request = new CreateGridStateRequest
        {
            ComponentKey = ComponentKey,
            Name = _stateName.Trim(),
            StateJson = CurrentStateJson,
            IsDefault = _isDefault
        };

        Response<GridStateDto> response = await api.CallAsync(
            () => gridStatesApi.CreateGridStateAsync(request),
            errorTitle: L.Localize("Entity_GridStates", "Grid states"),
            successMessage: L.Localize("GridStates_SavedSuccessfully", "Grid state saved successfully.")).ConfigureAwait(false);

        if (!response.IsError && response.Data is not null)
        {
            _selectedStateId = response.Data.Id;
            await LoadStatesAsync().ConfigureAwait(false);
        }

        _isBusy = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UpdateStateAsync()
    {
        if (_isBusy || string.IsNullOrWhiteSpace(_selectedStateId) || string.IsNullOrWhiteSpace(_stateName))
        {
            return;
        }

        _isBusy = true;
        UpdateGridStateRequest request = new UpdateGridStateRequest
        {
            Name = _stateName.Trim(),
            StateJson = CurrentStateJson,
            IsDefault = _isDefault
        };

        Response<GridStateDto> response = await api.CallAsync(
            () => gridStatesApi.UpdateGridStateAsync(_selectedStateId, request),
            errorTitle: L.Localize("Entity_GridStates", "Grid states"),
            successMessage: L.Localize("GridStates_UpdatedSuccessfully", "Grid state updated successfully.")).ConfigureAwait(false);

        if (!response.IsError)
        {
            await LoadStatesAsync().ConfigureAwait(false);
        }

        _isBusy = false;
        await InvokeAsync(StateHasChanged);
    }

    private async Task DeleteStateAsync()
    {
        if (_isBusy || string.IsNullOrWhiteSpace(_selectedStateId))
        {
            return;
        }

        bool? confirmed = await dialogService.Confirm(
            L.Localize("GridStates_ConfirmDelete", "Are you sure you want to permanently delete this grid state?"),
            L.Localize("GridStates_DeleteTitle", "Delete grid state"),
            new ConfirmOptions { OkButtonText = L.Localize("Button_Delete", "Delete"), CancelButtonText = L.Localize("Button_Cancel", "Cancel") });

        if (confirmed != true)
        {
            return;
        }

        _isBusy = true;

        Response response = await api.CallAsync(
            () => gridStatesApi.DeleteGridStateAsync(_selectedStateId),
            errorTitle: L.Localize("Entity_GridStates", "Grid states"),
            successMessage: L.Localize("GridStates_DeletedSuccessfully", "Grid state deleted successfully.")).ConfigureAwait(false);

        if (!response.IsError)
        {
            _selectedStateId = null;
            _stateName = string.Empty;
            _isDefault = false;
            await LoadStatesAsync().ConfigureAwait(false);
        }

        _isBusy = false;
        await InvokeAsync(StateHasChanged);
    }

}
