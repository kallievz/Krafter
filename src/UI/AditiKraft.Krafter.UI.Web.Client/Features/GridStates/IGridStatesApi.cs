using AditiKraft.Krafter.Contracts.Contracts.Grids;
using Refit;

namespace AditiKraft.Krafter.UI.Web.Client.Features.GridStates;

public interface IGridStatesApi
{
    [Get("/api/users/grid-states")]
    public Task<Response<List<GridStateDto>>> GetGridStatesAsync(
        [Query] string componentKey,
        CancellationToken cancellationToken = default);

    [Post("/api/users/grid-states")]
    public Task<Response<GridStateDto>> CreateGridStateAsync(
        [Body] CreateGridStateRequest request,
        CancellationToken cancellationToken = default);

    [Put("/api/users/grid-states/{id}")]
    public Task<Response<GridStateDto>> UpdateGridStateAsync(
        string id,
        [Body] UpdateGridStateRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/users/grid-states/{id}")]
    public Task<Response> DeleteGridStateAsync(
        string id,
        CancellationToken cancellationToken = default);
}
