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

public sealed class LanguagesCrud
{
    internal sealed class Handler(
        LocalizationDbContext localizationDbContext,
        ILocalizationTextResolver localizationTextResolver) : IScopedHandler
    {
        public async Task<Response<List<LanguageDto>>> GetLanguagesAsync(CancellationToken cancellationToken)
        {
            List<LanguageDto> items = await localizationDbContext.Languages.AsNoTracking()
                .OrderBy(language => language.Name)
                .Select(language => new LanguageDto
                {
                    Id = language.Id,
                    Code = language.Code,
                    Name = language.Name,
                    IsActive = language.IsActive,
                    CreatedOn = language.CreatedOn,
                    UpdatedOn = language.UpdatedOn
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return Response<List<LanguageDto>>.Success(items);
        }

        public async Task<Response<LanguageDto>> CreateLanguageAsync(CreateLanguageRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Response<LanguageDto>.BadRequest("Language code is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Response<LanguageDto>.BadRequest("Language name is required.");
            }

            string code = request.Code.Trim();
            string name = request.Name.Trim();

            int existingCount = await localizationDbContext.Languages.AsNoTracking()
                .CountAsync(language => language.Code == code, cancellationToken)
                .ConfigureAwait(false);

            if (existingCount > 0)
            {
                return Response<LanguageDto>.BadRequest("A language with the same code already exists.");
            }

            Language entity = new()
            {
                Id = Guid.CreateVersion7().ToString(),
                Code = code,
                Name = name,
                IsActive = request.IsActive
            };

            await localizationDbContext.Languages.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateAll();

            LanguageDto dto = new()
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                UpdatedOn = entity.UpdatedOn
            };

            return Response<LanguageDto>.Success(dto);
        }

        public async Task<Response<LanguageDto>> UpdateLanguageAsync(string id, UpdateLanguageRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response<LanguageDto>.BadRequest("Language id is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return Response<LanguageDto>.BadRequest("Language code is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return Response<LanguageDto>.BadRequest("Language name is required.");
            }

            Language? entity = await localizationDbContext.Languages
                .FirstOrDefaultAsync(language => language.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response<LanguageDto>.NotFound("Language not found.");
            }

            string code = request.Code.Trim();
            string name = request.Name.Trim();

            int duplicateCount = await localizationDbContext.Languages.AsNoTracking()
                .CountAsync(language => language.Id != id && language.Code == code, cancellationToken)
                .ConfigureAwait(false);

            if (duplicateCount > 0)
            {
                return Response<LanguageDto>.BadRequest("A language with the same code already exists.");
            }

            entity.Code = code;
            entity.Name = name;
            entity.IsActive = request.IsActive;
            entity.UpdatedOn = DateTime.UtcNow;

            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateAll();

            LanguageDto dto = new()
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                IsActive = entity.IsActive,
                CreatedOn = entity.CreatedOn,
                UpdatedOn = entity.UpdatedOn
            };

            return Response<LanguageDto>.Success(dto);
        }

        public async Task<Response> DeleteLanguageAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Response.BadRequest("Language id is required.");
            }

            Language? entity = await localizationDbContext.Languages
                .FirstOrDefaultAsync(language => language.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("Language not found.");
            }

            localizationDbContext.Languages.Remove(entity);
            await localizationDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            localizationTextResolver.InvalidateAll();

            return Response.Success();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            group.MapGet("/localization/languages", async (
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<LanguageDto>> response = await handler.GetLanguagesAsync(cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<LanguageDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.LocalizationLanguages);

            group.MapPost("/localization/languages", async (
                    [FromBody] CreateLanguageRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<LanguageDto> response = await handler.CreateLanguageAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<LanguageDto>>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.LocalizationLanguages);

            group.MapPut("/localization/languages/{id}", async (
                    [FromRoute] string id,
                    [FromBody] UpdateLanguageRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<LanguageDto> response = await handler.UpdateLanguageAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<LanguageDto>>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.LocalizationLanguages);

            group.MapDelete("/localization/languages/{id}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteLanguageAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.LocalizationLanguages);
        }
    }
}
