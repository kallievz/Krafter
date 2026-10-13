using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Features.Localization.Entities;
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

public sealed class ResourceTranslationsCrud
{
    internal sealed class Handler(
        LocalizationDbContext localizationDbContext,
        ILocalizationTextResolver localizationTextResolver) : IScopedHandler
    {
        public async Task<Response<List<ResourceTranslationDto>>> GetResourceTranslationsAsync(CancellationToken cancellationToken)
        {
            List<ResourceTranslationDto> items = await (
                from resourceTranslation in localizationDbContext.ResourceTranslations.AsNoTracking()
                join resource in localizationDbContext.Resources.AsNoTracking()
                    on resourceTranslation.ResourceId equals resource.Id
                join language in localizationDbContext.Languages.AsNoTracking()
                    on resourceTranslation.LanguageId equals language.Id
                orderby resource.Key, language.Code
                select new ResourceTranslationDto
                {
                    Id = resourceTranslation.Id,
                    ResourceId = resourceTranslation.ResourceId,
                    LanguageId = resourceTranslation.LanguageId,
                    Value = resourceTranslation.Value,
                    CreatedOn = resourceTranslation.CreatedOn,
                    UpdatedOn = resourceTranslation.UpdatedOn,
                    Resource = new ResourceDto
                    {
                        Id = resource.Id,
                        Key = resource.Key,
                        Description = resource.Description,
                        CreatedOn = resource.CreatedOn,
                        UpdatedOn = resource.UpdatedOn
                    },
                    Language = new LanguageDto
                    {
                        Id = language.Id,
                        Code = language.Code,
                        Name = language.Name,
                        IsActive = language.IsActive,
                        CreatedOn = language.CreatedOn,
                        UpdatedOn = language.UpdatedOn
                    }
                }).ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<ResourceTranslationDto>>.Success(items);
        }

        public async Task<Response<ResourceTranslationDto>> CreateResourceTranslationAsync(CreateResourceTranslationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ResourceId))
            {
                return Response<ResourceTranslationDto>.BadRequest("Resource id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.LanguageId))
            {
                return Response<ResourceTranslationDto>.BadRequest("Language id is required.");
            }

            bool resourceExists = await localizationDbContext.Resources.AsNoTracking()
                .CountAsync(resource => resource.Id == request.ResourceId, cancellationToken)
                .ConfigureAwait(false) > 0;
            if (!resourceExists)
            {
                return Response<ResourceTranslationDto>.BadRequest("Resource not found.");
            }

            bool languageExists = await localizationDbContext.Languages.AsNoTracking()
                .CountAsync(language => language.Id == request.LanguageId, cancellationToken)
                .ConfigureAwait(false) > 0;
            if (!languageExists)
            {
                return Response<ResourceTranslationDto>.BadRequest("Language not found.");
            }

            int existingCount = await localizationDbContext.ResourceTranslations.AsNoTracking()
                .CountAsync(resourceTranslation => resourceTranslation.ResourceId == request.ResourceId &&
                                                   resourceTranslation.LanguageId == request.LanguageId,
                    cancellationToken)
                .ConfigureAwait(false);

            if (existingCount > 0)
            {
                return Response<ResourceTranslationDto>.BadRequest("A translation for this resource and language already exists.");
            }

            ResourceTranslation entity = new()
            {
                Id = Guid.CreateVersion7().ToString(),
                ResourceId = request.ResourceId,
                LanguageId = request.LanguageId,
                Value = request.Value?.Trim() ?? string.Empty
            };

            await localizationDbContext.ResourceTranslations.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await localizationTextResolver.InvalidateByResourceIdAsync(entity.ResourceId, cancellationToken)
                .ConfigureAwait(false);

            ResourceTranslationDto? dto = await GetByIdAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            if (dto is null)
            {
                return Response<ResourceTranslationDto>.NotFound("Resource translation not found.");
            }

            return Response<ResourceTranslationDto>.Success(dto);
        }

        public async Task<Response<ResourceTranslationDto>> UpdateResourceTranslationAsync(string id, UpdateResourceTranslationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response<ResourceTranslationDto>.BadRequest("Resource translation id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.ResourceId))
            {
                return Response<ResourceTranslationDto>.BadRequest("Resource id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.LanguageId))
            {
                return Response<ResourceTranslationDto>.BadRequest("Language id is required.");
            }

            ResourceTranslation? entity = await localizationDbContext.ResourceTranslations
                .FirstOrDefaultAsync(resourceTranslation => resourceTranslation.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response<ResourceTranslationDto>.NotFound("Resource translation not found.");
            }

            bool resourceExists = await localizationDbContext.Resources.AsNoTracking()
                .CountAsync(resource => resource.Id == request.ResourceId, cancellationToken)
                .ConfigureAwait(false) > 0;
            if (!resourceExists)
            {
                return Response<ResourceTranslationDto>.BadRequest("Resource not found.");
            }

            bool languageExists = await localizationDbContext.Languages.AsNoTracking()
                .CountAsync(language => language.Id == request.LanguageId, cancellationToken)
                .ConfigureAwait(false) > 0;
            if (!languageExists)
            {
                return Response<ResourceTranslationDto>.BadRequest("Language not found.");
            }

            int duplicateCount = await localizationDbContext.ResourceTranslations.AsNoTracking()
                .CountAsync(resourceTranslation => resourceTranslation.Id != id &&
                                                   resourceTranslation.ResourceId == request.ResourceId &&
                                                   resourceTranslation.LanguageId == request.LanguageId,
                    cancellationToken)
                .ConfigureAwait(false);

            if (duplicateCount > 0)
            {
                return Response<ResourceTranslationDto>.BadRequest("A translation for this resource and language already exists.");
            }

            string previousResourceId = entity.ResourceId;
            entity.ResourceId = request.ResourceId;
            entity.LanguageId = request.LanguageId;
            entity.Value = request.Value?.Trim() ?? string.Empty;
            entity.UpdatedOn = DateTime.UtcNow;

            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await localizationTextResolver.InvalidateByResourceIdAsync(previousResourceId, cancellationToken)
                .ConfigureAwait(false);
            await localizationTextResolver.InvalidateByResourceIdAsync(entity.ResourceId, cancellationToken)
                .ConfigureAwait(false);

            ResourceTranslationDto? dto = await GetByIdAsync(entity.Id, cancellationToken).ConfigureAwait(false);
            if (dto is null)
            {
                return Response<ResourceTranslationDto>.NotFound("Resource translation not found.");
            }

            return Response<ResourceTranslationDto>.Success(dto);
        }

        private async Task<ResourceTranslationDto?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await (
                from resourceTranslation in localizationDbContext.ResourceTranslations.AsNoTracking()
                join resource in localizationDbContext.Resources.AsNoTracking()
                    on resourceTranslation.ResourceId equals resource.Id
                join language in localizationDbContext.Languages.AsNoTracking()
                    on resourceTranslation.LanguageId equals language.Id
                where resourceTranslation.Id == id
                select new ResourceTranslationDto
                {
                    Id = resourceTranslation.Id,
                    ResourceId = resourceTranslation.ResourceId,
                    LanguageId = resourceTranslation.LanguageId,
                    Value = resourceTranslation.Value,
                    CreatedOn = resourceTranslation.CreatedOn,
                    UpdatedOn = resourceTranslation.UpdatedOn,
                    Resource = new ResourceDto
                    {
                        Id = resource.Id,
                        Key = resource.Key,
                        Description = resource.Description,
                        CreatedOn = resource.CreatedOn,
                        UpdatedOn = resource.UpdatedOn
                    },
                    Language = new LanguageDto
                    {
                        Id = language.Id,
                        Code = language.Code,
                        Name = language.Name,
                        IsActive = language.IsActive,
                        CreatedOn = language.CreatedOn,
                        UpdatedOn = language.UpdatedOn
                    }
                }).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<Response> DeleteResourceTranslationAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.BadRequest("Resource translation id is required.");
            }

            ResourceTranslation? entity = await localizationDbContext.ResourceTranslations
                .FirstOrDefaultAsync(resourceTranslation => resourceTranslation.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("Resource translation not found.");
            }

            string deletedResourceId = entity.ResourceId;
            localizationDbContext.ResourceTranslations.Remove(entity);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await localizationTextResolver.InvalidateByResourceIdAsync(deletedResourceId, cancellationToken)
                .ConfigureAwait(false);

            return Response.Success();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            group.MapGet("/localization/resource-translations", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<ResourceTranslationDto>> response = await handler.GetResourceTranslationsAsync(cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<ResourceTranslationDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.LocalizationResourceTranslations);

            group.MapPost("/localization/resource-translations", async (
                    [FromBody] CreateResourceTranslationRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<ResourceTranslationDto> response = await handler.CreateResourceTranslationAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<ResourceTranslationDto>>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.LocalizationResourceTranslations);

            group.MapPut("/localization/resource-translations/{id}", async (
                    [FromRoute] string id,
                    [FromBody] UpdateResourceTranslationRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<ResourceTranslationDto> response = await handler.UpdateResourceTranslationAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<ResourceTranslationDto>>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.LocalizationResourceTranslations);

            group.MapDelete("/localization/resource-translations/{id}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteResourceTranslationAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.LocalizationResourceTranslations);
        }
    }
}
