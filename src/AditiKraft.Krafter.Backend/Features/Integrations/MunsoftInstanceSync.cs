using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Integrations;

public sealed class MunsoftInstanceSync
{
    internal sealed class Handler(
        IExternalTokenService externalTokenService,
        ITenantSettingService tenantSettingService,
        ApplicationDbContext applicationDbContext,
        IHttpClientFactory httpClientFactory) : IScopedHandler
    {
        public async Task<Response<MunsoftInstanceSyncResult>> SyncAsync(CancellationToken cancellationToken)
        {
            try
            {
                string token = await externalTokenService
                    .GetTokenAsync(ExternalClient.Munsoft, cancellationToken).ConfigureAwait(false);

                string baseAddress = await tenantSettingService.GetStringValueOrDefaultAsync(
                    TenantSettingKeys.MunsoftBaseAddess,
                    string.Empty,
                    cancellationToken).ConfigureAwait(false);

                string instancesPath = await tenantSettingService.GetStringValueOrDefaultAsync(
                    TenantSettingKeys.MunsoftInstances,
                    string.Empty,
                    cancellationToken).ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(baseAddress))
                {
                    return Response<MunsoftInstanceSyncResult>.BadRequest("Munsoft base address is not configured.");
                }

                if (string.IsNullOrWhiteSpace(instancesPath))
                {
                    return Response<MunsoftInstanceSyncResult>.BadRequest("Munsoft instances path is not configured.");
                }

                HttpClient httpClient = httpClientFactory.CreateClient(ExternalIntegrationHttpClientNames.MunsoftTokenClient);
                httpClient.BaseAddress = new Uri(baseAddress, UriKind.Absolute);

                using HttpRequestMessage requestMessage = new(HttpMethod.Get, instancesPath);
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                using HttpResponseMessage response = await httpClient.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
                string responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return Response<MunsoftInstanceSyncResult>.BadRequest(
                        $"Munsoft instances request failed with status {(int)response.StatusCode}: {responseContent}");
                }

                MunsoftInstancesResponse? munsoftResponse = JsonSerializer.Deserialize<MunsoftInstancesResponse>(responseContent);
                if (munsoftResponse?.Instances is null)
                {
                    return Response<MunsoftInstanceSyncResult>.BadRequest("Munsoft instances response did not include an instances collection.");
                }

                List<MunsoftInstancePayload> payloadInstances = munsoftResponse.Instances
                    .Where(instance => !string.IsNullOrWhiteSpace(instance.Name))
                    .GroupBy(instance => instance.Id)
                    .Select(group => group.First())
                    .ToList();

                List<int> payloadIds = payloadInstances.Select(instance => instance.Id).ToList();
                List<MunsoftInstance> existingInstances = await applicationDbContext.MunsoftInstances
                    .Where(instance => payloadIds.Contains(instance.Id))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<int, MunsoftInstance> existingById = existingInstances.ToDictionary(instance => instance.Id);

                int insertedCount = 0;
                int updatedCount = 0;

                foreach (MunsoftInstancePayload payloadInstance in payloadInstances)
                {
                    if (!existingById.TryGetValue(payloadInstance.Id, out MunsoftInstance? existingInstance))
                    {
                        MunsoftInstance newInstance = new MunsoftInstance
                        {
                            Id = payloadInstance.Id,
                            Name = payloadInstance.Name ?? string.Empty,
                            MunicipalityName = payloadInstance.MunicipalityName ?? string.Empty,
                            DemarcationCode = payloadInstance.DemarcationCode ?? string.Empty,
                            IsDeleted = false,
                            DeleteReason = null
                        };

                        await applicationDbContext.MunsoftInstances.AddAsync(newInstance, cancellationToken).ConfigureAwait(false);
                        insertedCount++;
                        continue;
                    }

                    bool hasChanges = existingInstance.Name != (payloadInstance.Name ?? string.Empty)
                        || existingInstance.MunicipalityName != (payloadInstance.MunicipalityName ?? string.Empty)
                        || existingInstance.DemarcationCode != (payloadInstance.DemarcationCode ?? string.Empty)
                        || existingInstance.IsDeleted;

                    if (!hasChanges)
                    {
                        continue;
                    }

                    existingInstance.Name = payloadInstance.Name ?? string.Empty;
                    existingInstance.MunicipalityName = payloadInstance.MunicipalityName ?? string.Empty;
                    existingInstance.DemarcationCode = payloadInstance.DemarcationCode ?? string.Empty;
                    existingInstance.IsDeleted = false;
                    existingInstance.DeleteReason = null;
                    updatedCount++;
                }

                await applicationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Response<MunsoftInstanceSyncResult>.Success(new MunsoftInstanceSyncResult
                {
                    TotalReceived = payloadInstances.Count,
                    Inserted = insertedCount,
                    Updated = updatedCount
                });
            }
            catch (InvalidOperationException exception)
            {
                return Response<MunsoftInstanceSyncResult>.BadRequest(exception.Message);
            }
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder routeGroupBuilder = endpointRouteBuilder.MapGroup(ApiRoutes.Integrations).AddFluentValidationFilter();
            routeGroupBuilder.MapPost("/munsoft/instances/sync", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<MunsoftInstanceSyncResult> result = await handler.SyncAsync(cancellationToken).ConfigureAwait(false);
                    return Results.Json(result, statusCode: result.StatusCode);
                })
                .Produces<Response<MunsoftInstanceSyncResult>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Tenants);
        }
    }

    private sealed record MunsoftInstancesResponse(
        [property: JsonPropertyName("instances")] List<MunsoftInstancePayload>? Instances);

    private sealed record MunsoftInstancePayload(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string? Name,
        [property: JsonPropertyName("municipality_name")] string? MunicipalityName,
        [property: JsonPropertyName("demarcation_code")] string? DemarcationCode);
}
