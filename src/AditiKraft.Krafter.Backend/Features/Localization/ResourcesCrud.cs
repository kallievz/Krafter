using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Features.Localization;

public sealed class ResourcesCrud
{
    internal sealed class Handler(
        LocalizationDbContext localizationDbContext,
        ILocalizationTextResolver localizationTextResolver,
        IJobService jobService,
        ILocalizationTranslationJobTracker localizationTranslationJobTracker) : IScopedHandler
    {
        public async Task<Response<List<ResourceDto>>> GetResourcesAsync(CancellationToken cancellationToken)
        {
            List<ResourceDto> items = await localizationDbContext.Resources.AsNoTracking()
                .OrderBy(resource => resource.Key)
                .Select(resource => new ResourceDto
                {
                    Id = resource.Id,
                    Key = resource.Key,
                    Description = resource.Description,
                    CreatedOn = resource.CreatedOn,
                    UpdatedOn = resource.UpdatedOn
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<ResourceDto>>.Success(items);
        }

        public async Task<Response<ResourceDto>> CreateResourceAsync(CreateResourceRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Key))
            {
                return Response<ResourceDto>.BadRequest("Resource key is required.");
            }

            string key = request.Key.Trim();
            string description = request.Description?.Trim() ?? string.Empty;

            int existingCount = await localizationDbContext.Resources.AsNoTracking()
                .CountAsync(resource => resource.Key == key, cancellationToken)
                .ConfigureAwait(false);

            if (existingCount > 0)
            {
                return Response<ResourceDto>.BadRequest("A resource with the same key already exists.");
            }

            Resource entity = new()
            {
                Id = Guid.CreateVersion7().ToString(),
                Key = key,
                Description = description
            };

            await localizationDbContext.Resources.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateByResourceKey(entity.Key);

            ResourceDto dto = new()
            {
                Id = entity.Id,
                Key = entity.Key,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                UpdatedOn = entity.UpdatedOn
            };

            return Response<ResourceDto>.Success(dto);
        }

        public async Task<Response<ResourceDto>> UpdateResourceAsync(string id, UpdateResourceRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response<ResourceDto>.BadRequest("Resource id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Key))
            {
                return Response<ResourceDto>.BadRequest("Resource key is required.");
            }

            Resource? entity = await localizationDbContext.Resources
                .FirstOrDefaultAsync(resource => resource.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response<ResourceDto>.NotFound("Resource not found.");
            }

            string key = request.Key.Trim();
            string description = request.Description?.Trim() ?? string.Empty;
            string previousKey = entity.Key;

            int duplicateCount = await localizationDbContext.Resources.AsNoTracking()
                .CountAsync(resource => resource.Id != id && resource.Key == key, cancellationToken)
                .ConfigureAwait(false);

            if (duplicateCount > 0)
            {
                return Response<ResourceDto>.BadRequest("A resource with the same key already exists.");
            }

            entity.Key = key;
            entity.Description = description;
            entity.UpdatedOn = DateTime.UtcNow;

            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateByResourceKey(previousKey);
            localizationTextResolver.InvalidateByResourceKey(entity.Key);

            ResourceDto dto = new()
            {
                Id = entity.Id,
                Key = entity.Key,
                Description = entity.Description,
                CreatedOn = entity.CreatedOn,
                UpdatedOn = entity.UpdatedOn
            };

            return Response<ResourceDto>.Success(dto);
        }

        public async Task<Response> DeleteResourceAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.BadRequest("Resource id is required.");
            }

            Resource? entity = await localizationDbContext.Resources
                .FirstOrDefaultAsync(resource => resource.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("Resource not found.");
            }

            string deletedKey = entity.Key;
            localizationDbContext.Resources.Remove(entity);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateByResourceKey(deletedKey);

            return Response.Success();
        }

        public async Task<Response> TriggerBackgroundTranslationAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.BadRequest("Resource id is required.");
            }

            Resource? entity = await localizationDbContext.Resources.AsNoTracking()
                .FirstOrDefaultAsync(resource => resource.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("Resource not found.");
            }

            await jobService.EnqueueAsync(
                new GenerateMissingLocalizationTranslationsRequestInput
                {
                    ResourceId = entity.Id
                },
                nameof(Jobs.GenerateMissingLocalizationTranslationsJobAsync),
                cancellationToken).ConfigureAwait(false);

            localizationTranslationJobTracker.MarkEnqueued(entity.Id);

            return Response.Success();
        }

        public Response<LocalizationTranslationJobStatusDto> GetTranslationJobStatus(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response<LocalizationTranslationJobStatusDto>.BadRequest("Resource id is required.");
            }

            Resource? entity = localizationDbContext.Resources.AsNoTracking()
                .FirstOrDefault(resource => resource.Id == id);

            if (entity is null)
            {
                return Response<LocalizationTranslationJobStatusDto>.NotFound("Resource not found.");
            }

            LocalizationTranslationJobStatus status = localizationTranslationJobTracker.GetStatus(entity.Id);
            LocalizationTranslationJobStatusDto dto = new()
            {
                ResourceId = status.ResourceId,
                State = status.State.ToString(),
                UpdatedOnUtc = status.UpdatedOnUtc,
                ErrorMessage = status.ErrorMessage
            };

            return Response<LocalizationTranslationJobStatusDto>.Success(dto);
        }

        public Response<LocalizationTranslationJobHealthDto> GetTranslationJobHealth()
        {
            LocalizationTranslationJobHealth health = localizationTranslationJobTracker.GetHealth();
            LocalizationTranslationJobHealthDto dto = new()
            {
                TotalTracked = health.TotalTracked,
                EnqueuedCount = health.EnqueuedCount,
                ExecutingCount = health.ExecutingCount,
                CompletedCount = health.CompletedCount,
                FailedCount = health.FailedCount,
                UpdatedOnUtc = health.UpdatedOnUtc
            };

            return Response<LocalizationTranslationJobHealthDto>.Success(dto);
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            group.MapGet("/localization/resources", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<ResourceDto>> response = await handler.GetResourcesAsync(cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<ResourceDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.LocalizationResources);

            group.MapPost("/localization/resources", async (
                    [FromBody] CreateResourceRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<ResourceDto> response = await handler.CreateResourceAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<ResourceDto>>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.LocalizationResources);

            group.MapPut("/localization/resources/{id}", async (
                    [FromRoute] string id,
                    [FromBody] UpdateResourceRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<ResourceDto> response = await handler.UpdateResourceAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<ResourceDto>>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.LocalizationResources);

            group.MapDelete("/localization/resources/{id}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteResourceAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.LocalizationResources);

            group.MapPost("/localization/resources/{id}/generate-translations", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.TriggerBackgroundTranslationAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.LocalizationResources);

            group.MapGet("/localization/resources/{id}/translation-job-status", (
                    [FromRoute] string id,
                    [FromServices] Handler handler) =>
                {
                    Response<LocalizationTranslationJobStatusDto> response = handler.GetTranslationJobStatus(id);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<LocalizationTranslationJobStatusDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.LocalizationResources);

            group.MapGet("/localization/jobs/health", (
                    [FromServices] Handler handler) =>
                {
                    Response<LocalizationTranslationJobHealthDto> response = handler.GetTranslationJobHealth();
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<LocalizationTranslationJobHealthDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.LocalizationResources);
        }
    }
}
