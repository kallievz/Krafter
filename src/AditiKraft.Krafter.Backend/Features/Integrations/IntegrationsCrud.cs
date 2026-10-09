using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Net.Http.Headers;
using System.Text.Json;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Integration.Munsoft;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Integrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Integrations;

public sealed class IntegrationsCrud
{
    internal sealed class Handler(
        IExternalTokenService externalTokenService,
        ITenantSettingService tenantSettingService,
        MunsoftDbContext munsoftDbContext,
        IHttpClientFactory httpClientFactory) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<MunsoftInstanceDto>>> GetMunsoftInstancesAsync(
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            IQueryable<MunsoftInstanceDto> query = munsoftDbContext.MunsoftInstances
                .AsNoTracking()
                .Where(instance => !instance.IsDeleted)
                .Select(instance => new MunsoftInstanceDto
                {
                    Id = instance.Id,
                    Name = instance.Name,
                    MunicipalityName = instance.MunicipalityName,
                    DemarcationCode = instance.DemarcationCode
                });

            if (!string.IsNullOrWhiteSpace(requestInput.Filter))
            {
                query = query.Where(requestInput.Filter);
            }

            if (!string.IsNullOrWhiteSpace(requestInput.OrderBy))
            {
                try
                {
                    query = query.OrderBy(requestInput.OrderBy);
                }
                catch (ParseException)
                {
                    query = query.OrderBy(instance => instance.Name);
                }
            }
            else
            {
                query = query.OrderBy(instance => instance.Name);
            }

            List<MunsoftInstanceDto> items = await query.PageBy(requestInput)
                .ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            return Response<PaginationResponse<MunsoftInstanceDto>>.Success(
                new PaginationResponse<MunsoftInstanceDto>(items, totalCount, requestInput.SkipCount,
                    requestInput.MaxResultCount));
        }

        public async Task<Response<MunsoftInstanceSyncResult>> PatchMunsoftInstancesAsync(CancellationToken cancellationToken)
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

                List<MunsoftInstancePayload> parsedPayloadInstances = ExtractPayloadInstances(responseContent);
                if (parsedPayloadInstances.Count == 0)
                {
                    return Response<MunsoftInstanceSyncResult>.BadRequest("Munsoft instances response did not include a valid instances collection.");
                }

                List<MunsoftInstancePayload> payloadInstances = parsedPayloadInstances
                    .Where(instance => !string.IsNullOrWhiteSpace(instance.Name))
                    .GroupBy(instance => instance.Id)
                    .Select(group => group.First())
                    .ToList();

                List<int> invalidInstanceIds = payloadInstances
                    .Where(instance => string.IsNullOrWhiteSpace(instance.GetMunicipalityName()) ||
                                       string.IsNullOrWhiteSpace(instance.GetDemarcationCode()))
                    .Select(instance => instance.Id)
                    .ToList();

                if (invalidInstanceIds.Count > 0)
                {
                    string invalidIds = string.Join(", ", invalidInstanceIds);
                    return Response<MunsoftInstanceSyncResult>.BadRequest(
                        $"Munsoft payload is invalid. municipality_name and demarcation_code are required for instances: {invalidIds}.");
                }

                List<int> payloadIds = payloadInstances.Select(instance => instance.Id).ToList();
                List<MunsoftInstance> existingInstances = await munsoftDbContext.MunsoftInstances
                    .Where(instance => payloadIds.Contains(instance.Id))
                    .ToListAsync(cancellationToken).ConfigureAwait(false);

                Dictionary<int, MunsoftInstance> existingById = existingInstances.ToDictionary(instance => instance.Id);

                int insertedCount = 0;
                int updatedCount = 0;

                foreach (MunsoftInstancePayload payloadInstance in payloadInstances)
                {
                    if (!existingById.TryGetValue(payloadInstance.Id, out MunsoftInstance? existingInstance) ||
                        existingInstance is null)
                    {
                        MunsoftInstance newInstance = new MunsoftInstance
                        {
                            Id = payloadInstance.Id,
                            Name = payloadInstance.Name,
                            MunicipalityName = payloadInstance.GetMunicipalityName(),
                            DemarcationCode = payloadInstance.GetDemarcationCode(),
                            IsDeleted = false,
                            DeleteReason = null
                        };

                        await munsoftDbContext.MunsoftInstances.AddAsync(newInstance, cancellationToken).ConfigureAwait(false);
                        insertedCount++;
                        continue;
                    }

                    string mappedMunicipalityName = payloadInstance.GetMunicipalityName();
                    string mappedDemarcationCode = payloadInstance.GetDemarcationCode();

                    bool hasChanges = existingInstance.Name != payloadInstance.Name
                                      || existingInstance.MunicipalityName != mappedMunicipalityName
                                      || existingInstance.DemarcationCode != mappedDemarcationCode
                                      || existingInstance.IsDeleted;

                    if (!hasChanges)
                    {
                        continue;
                    }

                    existingInstance.Name = payloadInstance.Name;
                    existingInstance.MunicipalityName = mappedMunicipalityName;
                    existingInstance.DemarcationCode = mappedDemarcationCode;
                    existingInstance.IsDeleted = false;
                    existingInstance.DeleteReason = null;
                    existingInstance.UpdatedOn = DateTime.UtcNow;
                    updatedCount++;
                }

                await munsoftDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

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

    private static List<MunsoftInstancePayload> ExtractPayloadInstances(string responseContent)
    {
        using JsonDocument payloadDocument = JsonDocument.Parse(responseContent);
        JsonElement root = payloadDocument.RootElement;

        if (!root.TryGetProperty("instances", out JsonElement instancesNode) ||
            instancesNode.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        List<MunsoftInstancePayload> payloadInstances = [];

        foreach (JsonElement instanceNode in instancesNode.EnumerateArray())
        {
            if (instanceNode.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            int id = GetIntProperty(instanceNode, "id");
            string name = GetStringProperty(instanceNode, "name");
            string municipalityName = GetStringProperty(instanceNode, "municipality_name");
            string demarcationCode = GetStringProperty(instanceNode, "demarcation_code");

            payloadInstances.Add(new MunsoftInstancePayload
            {
                Id = id,
                Name = name,
                MunicipalityName = municipalityName,
                DemarcationCode = demarcationCode
            });
        }

        return payloadInstances;
    }

    private static int GetIntProperty(JsonElement node, string propertyName)
    {
        if (!node.TryGetProperty(propertyName, out JsonElement propertyValue))
        {
            return 0;
        }

        return propertyValue.ValueKind == JsonValueKind.Number && propertyValue.TryGetInt32(out int parsedValue)
            ? parsedValue
            : 0;
    }

    private static string GetStringProperty(JsonElement node, string propertyName)
    {
        if (!node.TryGetProperty(propertyName, out JsonElement propertyValue))
        {
            return string.Empty;
        }

        if (propertyValue.ValueKind != JsonValueKind.String)
        {
            return string.Empty;
        }

        return propertyValue.GetString()?.Trim() ?? string.Empty;
    }

    private sealed class MunsoftInstancePayload
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MunicipalityName { get; set; } = string.Empty;
        public string DemarcationCode { get; set; } = string.Empty;

        public string GetMunicipalityName()
        {
            return MunicipalityName;
        }

        public string GetDemarcationCode()
        {
            return DemarcationCode;
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder routeGroupBuilder = endpointRouteBuilder.MapGroup(ApiRoutes.Integrations)
                .AddFluentValidationFilter();

            routeGroupBuilder.MapGet("/munsoft/instances", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<MunsoftInstanceDto>> result = await handler
                        .GetMunsoftInstancesAsync(requestInput, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(result, statusCode: result.StatusCode);
                })
                .Produces<Response<PaginationResponse<MunsoftInstanceDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            routeGroupBuilder.MapPatch("/munsoft/instances/sync", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<MunsoftInstanceSyncResult> result = await handler
                        .PatchMunsoftInstancesAsync(cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(result, statusCode: result.StatusCode);
                })
                .Produces<Response<MunsoftInstanceSyncResult>>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);
        }
    }
}
