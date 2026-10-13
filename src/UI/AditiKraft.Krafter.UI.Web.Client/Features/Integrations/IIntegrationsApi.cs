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

    [Get("/api/integrations/munsoft/debtors")]
    public Task<Response<PaginationResponse<MunsoftDebtorDto>>> GetMunsoftDebtorsAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Get("/api/integrations/munsoft/debtors/{id}")]
    public Task<Response<MunsoftDebtorDto>> GetMunsoftDebtorByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Post("/api/integrations/munsoft/debtors")]
    public Task<Response> CreateMunsoftDebtorAsync(
        [Body] CreateOrUpdateMunsoftDebtorRequest request,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/debtors/{id}")]
    public Task<Response> UpdateMunsoftDebtorAsync(
        Guid id,
        [Body] CreateOrUpdateMunsoftDebtorRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/integrations/munsoft/debtors/{id}")]
    public Task<Response> DeleteMunsoftDebtorAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/debtors/{id}/processing")]
    public Task<Response> UpdateMunsoftDebtorProcessingAsync(
        Guid id,
        [Body] UpdateMunsoftDebtorProcessingRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/integrations/munsoft/ageings")]
    public Task<Response<PaginationResponse<MunsoftAgeingDto>>> GetMunsoftAgeingsAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Get("/api/integrations/munsoft/ageings/{id}")]
    public Task<Response<MunsoftAgeingDto>> GetMunsoftAgeingByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Post("/api/integrations/munsoft/ageings")]
    public Task<Response> CreateMunsoftAgeingAsync(
        [Body] CreateOrUpdateMunsoftAgeingRequest request,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/ageings/{id}")]
    public Task<Response> UpdateMunsoftAgeingAsync(
        Guid id,
        [Body] CreateOrUpdateMunsoftAgeingRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/integrations/munsoft/ageings/{id}")]
    public Task<Response> DeleteMunsoftAgeingAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/ageings/{id}/processing")]
    public Task<Response> UpdateMunsoftAgeingProcessingAsync(
        Guid id,
        [Body] UpdateMunsoftAgeingProcessingRequest request,
        CancellationToken cancellationToken = default);

    [Get("/api/integrations/munsoft/indigents")]
    public Task<Response<PaginationResponse<MunsoftIndigentDto>>> GetMunsoftIndigentsAsync(
        [Query] GetRequestInput request,
        CancellationToken cancellationToken = default);

    [Get("/api/integrations/munsoft/indigents/{id}")]
    public Task<Response<MunsoftIndigentDto>> GetMunsoftIndigentByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Post("/api/integrations/munsoft/indigents")]
    public Task<Response> CreateMunsoftIndigentAsync(
        [Body] CreateOrUpdateMunsoftIndigentRequest request,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/indigents/{id}")]
    public Task<Response> UpdateMunsoftIndigentAsync(
        Guid id,
        [Body] CreateOrUpdateMunsoftIndigentRequest request,
        CancellationToken cancellationToken = default);

    [Delete("/api/integrations/munsoft/indigents/{id}")]
    public Task<Response> DeleteMunsoftIndigentAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    [Patch("/api/integrations/munsoft/indigents/{id}/processing")]
    public Task<Response> UpdateMunsoftIndigentProcessingAsync(
        Guid id,
        [Body] UpdateMunsoftIndigentProcessingRequest request,
        CancellationToken cancellationToken = default);
}
