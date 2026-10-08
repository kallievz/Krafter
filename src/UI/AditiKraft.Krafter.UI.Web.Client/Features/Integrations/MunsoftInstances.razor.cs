using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Integrations;

public partial class MunsoftInstances(
    ApiCallService api,
    IIntegrationsApi integrationsApi
) : ComponentBase
{
    public const string RoutePath = $"{ApiRoutes.Integrations}/munsoft";

    private readonly GetRequestInput _requestInput = new();
    private bool _isLoading = true;
    private string? _errorMessage;

    private Response<PaginationResponse<MunsoftInstanceDto>>? _response =
        new() { Data = new PaginationResponse<MunsoftInstanceDto>() };

    protected override async Task OnInitializedAsync()
    {
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
            await GetListAsync().ConfigureAwait(false);
            return;
        }

        _errorMessage = syncResponse.Message;
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }
}
