using AditiKraft.Krafter.Backend.Features.Localization.Common;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using Microsoft.AspNetCore.Mvc;

namespace AditiKraft.Krafter.Backend.Features.Localization;

public sealed class GetLocalizedValues
{
    internal sealed class Handler(
        ILocalizationTextResolver localizationTextResolver,
        IHttpContextAccessor httpContextAccessor) : IScopedHandler
    {
        public async Task<Response<LocalizedTextDto>> ResolveAsync(
            LocalizeTextRequest request,
            CancellationToken cancellationToken)
        {
            string preferredLanguageCode = GetPreferredLanguageCode();
            LocalizedTextDto item = await localizationTextResolver
                .ResolveAsync(request, preferredLanguageCode, cancellationToken)
                .ConfigureAwait(false);
            return Response<LocalizedTextDto>.Success(item);
        }

        public async Task<Response<List<LocalizedTextDto>>> ResolveManyAsync(
            LocalizeBatchRequest request,
            CancellationToken cancellationToken)
        {
            string preferredLanguageCode = GetPreferredLanguageCode();
            IReadOnlyList<LocalizeTextRequest> entries = request.Entries ?? [];
            List<LocalizedTextDto> items = await localizationTextResolver
                .ResolveManyAsync(entries, preferredLanguageCode, cancellationToken)
                .ConfigureAwait(false);

            return Response<List<LocalizedTextDto>>.Success(items);
        }

        private string GetPreferredLanguageCode()
        {
            string? acceptLanguageValue = httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage
                .FirstOrDefault();
            string? headerLanguageCode = acceptLanguageValue?
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault()?
                .Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(headerLanguageCode))
            {
                return headerLanguageCode.Trim().ToLowerInvariant();
            }

            string? claimValue = httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(claim => claim.Type == AppClaimTypes.PreferredLanguageCode)?.Value;

            return string.IsNullOrWhiteSpace(claimValue)
                ? "en"
                : claimValue.Trim().ToLowerInvariant();
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Users)
                .AddFluentValidationFilter();

            group.MapPost("/localization/values/resolve", async (
                    [FromBody] LocalizeTextRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<LocalizedTextDto> response = await handler
                        .ResolveAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<LocalizedTextDto>>()
                .RequireAuthorization();

            group.MapPost("/localization/values/resolve/batch", async (
                    [FromBody] LocalizeBatchRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<List<LocalizedTextDto>> response = await handler
                        .ResolveManyAsync(request, cancellationToken)
                        .ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<List<LocalizedTextDto>>>()
                .RequireAuthorization();
        }
    }
}
