using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Integrations;

public interface IIntegrationsApi
{
    [Get("/api/integrations/munsoft/instances")]
    public Task<Response<PaginationResponse<MunsoftInstanceDto>>> GetMunsoftInstancesAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/instances/sync")]
    public Task<Response<MunsoftInstanceSyncResult>> SyncMunsoftInstancesAsync(
        CancellationToken cancellationToken = default);
}
