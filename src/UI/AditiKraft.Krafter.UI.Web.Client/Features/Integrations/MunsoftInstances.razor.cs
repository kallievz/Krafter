using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Integrations;

public partial class MunsoftInstances(
    ApiCallService api,
    IIntegrationsApi integrationsApi
) : ComponentBase
{
    [Inject]
    private Radzen.ILocalizer L { get; set; } = null!;
    public const string RoutePath = $"{ApiRoutes.Integrations}/munsoft";

    private readonly GetRequestInput _requestInput = new();
    private bool _isLoading = true;
    private string? _errorMessage;

    private IReadOnlyList<LookupOption<int>> _idLookupOptions = [];
    private IReadOnlyList<LookupOption<string>> _nameLookupOptions = [];
    private IReadOnlyList<LookupOption<string>> _municipalityLookupOptions = [];
    private IReadOnlyList<LookupOption<string>> _demarcationLookupOptions = [];

    private Response<PaginationResponse<MunsoftInstanceDto>>? _response =
        new() { Data = new PaginationResponse<MunsoftInstanceDto>() };

    protected override async Task OnInitializedAsync()
    {
        LocalAppState.CurrentPageTitle = L.Localize("Nav_MunsoftInstances", "Munsoft Instances") ;
        await GetListAsync().ConfigureAwait(false);
    }

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;

        await Task.Yield();

        _requestInput.SkipCount = args.Skip ?? 0;
        _requestInput.MaxResultCount = args.Top ?? 10;
        _requestInput.Filter = args.Filter;
        _requestInput.OrderBy = args.OrderBy;

        await GetListAsync().ConfigureAwait(false);
    }

    private async Task GetListAsync(bool resetPaginationData = false)
    {
        _isLoading = true;
        if (resetPaginationData)
        {
            _requestInput.SkipCount = 0;
        }

        _response = await api.CallAsync(
            () => integrationsApi.GetMunsoftInstancesAsync(_requestInput),
            errorTitle: "Munsoft").ConfigureAwait(false);

        _errorMessage = _response.IsError ? _response.Message : null;
        if (!_response.IsError)
        {
            UpdateFilterLookups(_response?.Data?.Items ?? []);
        }

        _isLoading = false;

        await InvokeAsync(StateHasChanged);
    }

    private async Task SyncAsync()
    {
        _isLoading = true;
        await InvokeAsync(StateHasChanged);

        Response<MunsoftInstanceSyncResult> syncResponse = await api.CallAsync(
            () => integrationsApi.SyncMunsoftInstancesAsync(),
            errorTitle: "Munsoft Sync",
            successMessage: "Munsoft instances synchronized successfully.").ConfigureAwait(false);

        if (!syncResponse.IsError)
        {
            _errorMessage = null;
            await GetListAsync(resetPaginationData: true).ConfigureAwait(false);
            return;
        }

        _errorMessage = syncResponse.Message;
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }

    private Task OnGridFilterClearedAsync()
    {
        return Task.CompletedTask;
    }

    private Task OnGridResetAsync()
    {
        _requestInput.SkipCount = 0;
        _requestInput.Filter = null;
        _requestInput.OrderBy = null;
        return Task.CompletedTask;
    }

    private void UpdateFilterLookups(IEnumerable<MunsoftInstanceDto> items)
    {
        List<MunsoftInstanceDto> itemList = items.ToList();

        _idLookupOptions = itemList
            .Select(instance => instance.Id)
            .Distinct()
            .OrderBy(id => id)
            .Select(id => new LookupOption<int>(id, id.ToString()))
            .ToList();

        _nameLookupOptions = itemList
            .Select(instance => instance.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .Select(name => new LookupOption<string>(name, name))
            .ToList();

        _municipalityLookupOptions = itemList
            .Select(instance => instance.MunicipalityName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .Select(name => new LookupOption<string>(name, name))
            .ToList();

        _demarcationLookupOptions = itemList
            .Select(instance => instance.DemarcationCode)
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(code => code)
            .Select(code => new LookupOption<string>(code, code))
            .ToList();
    }

    private sealed record LookupOption<TValue>(TValue Value, string Text);
}
