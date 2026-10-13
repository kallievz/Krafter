using System.Linq.Dynamic.Core;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Features.Users.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web;
using AditiKraft.Krafter.Backend.Web.Authorization;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Auth.Permissions;
using AditiKraft.Krafter.Contracts.Common.Enums;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using FluentValidation.Results;
using LinqKit;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Features.Tenants;

public sealed class TenantsCrud
{
    internal sealed class Handler(
        TenantDbContext dbContext,
        IServiceProvider serviceProvider,
        ICurrentUser currentUser,
        AppUrls urls,
        IOptions<SecuritySettings> securitySettings,
        SMTPEmailSettings smtpEmailSettings,
        SendGridEmailSettings sendGridEmailSettings) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<TenantDto>>> GetAsync(
            GetRequestInput requestInput,
            CancellationToken cancellationToken)
        {
            if (requestInput.History)
            {
                return Response<PaginationResponse<TenantDto>>.BadRequest(
                    "Tenant history is not supported. Request current or deleted tenants instead.");
            }

            ExpressionStarter<Tenant>? predicate = PredicateBuilder.New<Tenant>(true);
            if (!string.IsNullOrWhiteSpace(requestInput.Id))
            {
                predicate = predicate.And(tenant => tenant.Id == requestInput.Id);
            }

            IQueryable<Tenant> tenants = requestInput.IsDeleted
                ? dbContext.Tenants.IgnoreQueryFilters().Where(tenant => tenant.IsDeleted)
                : dbContext.Tenants;

            IQueryable<TenantDto> query = tenants.Where(predicate).Select(tenant => new TenantDto
            {
                Id = tenant.Id,
                Name = tenant.Name,
                Identifier = tenant.Identifier,
                AdminEmail = tenant.AdminEmail,
                ValidUpto = tenant.ValidUpto,
                IsActive = tenant.IsActive,
                CreatedById = tenant.CreatedById,
                IsDeleted = tenant.IsDeleted,
                CreatedOn = tenant.CreatedOn,
                DeleteReason = tenant.DeleteReason
            });

            if (!string.IsNullOrEmpty(requestInput.Filter))
            {
                query = query.Where(requestInput.Filter);
            }

            if (!string.IsNullOrEmpty(requestInput.OrderBy))
            {
                query = query.OrderBy(requestInput.OrderBy);
            }

            List<TenantDto> data = await query.PageBy(requestInput).ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            PaginationResponse<TenantDto> response = new PaginationResponse<TenantDto>(
                data,
                totalCount,
                requestInput.SkipCount,
                requestInput.MaxResultCount);

            return Response<PaginationResponse<TenantDto>>.Success(response);
        }

        public async Task<Response> CreateAsync(CreateOrUpdateTenantRequest request, CancellationToken cancellationToken)
        {
            if (AditiKraft.Krafter.Contracts.Common.TenantSettings.TenancyMode == TenancyMode.Single)
            {
                return Response.BadRequest("Tenant creation is not allowed in single-tenant mode.");
            }

            request.Id = null;
            ValidationResult validation = await new CreateOrUpdateTenantRequestValidator(urls)
                .ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                return Response.BadRequest(string.Join(" ", validation.Errors.Select(error => error.ErrorMessage)));
            }

            if (request.ValidUpto!.Value.Kind != DateTimeKind.Utc)
            {
                return Response.BadRequest("Send the expiry time in UTC with a Z suffix.");
            }

            bool identifierExists = await dbContext.Tenants.AsNoTracking()
                .AnyAsync(tenant => tenant.Identifier.ToLower() == request.Identifier, cancellationToken)
                .ConfigureAwait(false);
            if (identifierExists)
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            request.Id = request.Identifier;
            Tenant entity = request.Adapt<Tenant>();
            entity.ValidUpto = request.ValidUpto.Value;
            entity.CreatedOn = DateTime.UtcNow;
            entity.CreatedById = currentUser.GetUserId();

            dbContext.Tenants.Add(entity);
            dbContext.TenantSettings.AddRange(CreateDefaultSettings(
                entity.Id!,
                securitySettings.Value,
                smtpEmailSettings,
                sendGridEmailSettings));

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException exception) when (IsDuplicateIdentifierConstraintViolation(exception))
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            using IServiceScope scope = serviceProvider.CreateScope();
            ITenantSetterService tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetterService>();
            CurrentTenantDetails currentTenantDetails = entity.Adapt<CurrentTenantDetails>();
            currentTenantDetails.TenantLink = TenantLinkBuilder.GetTenantLink(urls, request.Identifier);
            tenantSetter.SetTenant(currentTenantDetails);

            DataSeedService seedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
            await seedService.SeedBasicDataAsync(new SeedDataRequest()).ConfigureAwait(false);

            return Response.Success();
        }

        public async Task<Response> PatchAsync(
            string id,
            CreateOrUpdateTenantRequest request,
            CancellationToken cancellationToken)
        {
            request.Id = id;
            Tenant? tenant = await dbContext.Tenants.FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                .ConfigureAwait(false);
            if (tenant is null)
            {
                return Response.BadRequest("Unable to find tenant, please try again later or contact support.");
            }

            if (tenant.Id == SeedDataConstants.RootTenant.Id)
            {
                if (request.Identifier != tenant.Identifier)
                {
                    return Response.Forbidden("Root tenant identifier cannot be modified.");
                }

                if (request.AdminEmail != tenant.AdminEmail)
                {
                    return Response.Forbidden("Root tenant admin email cannot be modified.");
                }

                if (request.IsActive.HasValue && request.IsActive.Value != tenant.IsActive)
                {
                    return Response.Forbidden("Root tenant cannot be deactivated.");
                }

                if (request.ValidUpto.HasValue && request.ValidUpto.Value != tenant.ValidUpto)
                {
                    return Response.Forbidden("Root tenant validity date cannot be modified.");
                }
            }

            ValidationResult validation = await new CreateOrUpdateTenantRequestValidator(urls)
                .ValidateAsync(request, cancellationToken).ConfigureAwait(false);
            if (!validation.IsValid)
            {
                return Response.BadRequest(string.Join(" ", validation.Errors.Select(error => error.ErrorMessage)));
            }

            if (request.ValidUpto!.Value.Kind != DateTimeKind.Utc
                && !(tenant.Id == DefaultTenantConstants.Identifier && request.ValidUpto == DateTime.MaxValue))
            {
                return Response.BadRequest("Send the expiry time in UTC with a Z suffix.");
            }

            string normalizedIdentifier = request.Identifier.ToUpperInvariant();
            bool identifierExists = await dbContext.Tenants.AsNoTracking()
                .AnyAsync(c => c.Id != tenant.Id && c.Identifier.ToUpper() == normalizedIdentifier, cancellationToken)
                .ConfigureAwait(false);
            if (identifierExists)
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            if (request.Name != tenant.Name)
            {
                tenant.Name = request.Name;
            }

            if (request.Identifier != tenant.Identifier)
            {
                tenant.Identifier = request.Identifier;
            }

            if (request.AdminEmail != tenant.AdminEmail)
            {
                using IServiceScope scope = serviceProvider.CreateScope();

                ITenantSetterService tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetterService>();
                CurrentTenantDetails currentTenantDetails = tenant.Adapt<CurrentTenantDetails>();
                currentTenantDetails.TenantLink = TenantLinkBuilder.GetTenantLink(urls, request.Identifier);
                tenantSetter.SetTenant(currentTenantDetails);

                UserManager<ApplicationUser> userManager =
                    scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                IUserMutationService userService = scope.ServiceProvider.GetRequiredService<IUserMutationService>();

                ApplicationUser? user = await userManager.Users.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.NormalizedEmail == tenant.AdminEmail.ToUpper(), cancellationToken)
                    .ConfigureAwait(false);

                if (user is not null)
                {
                    Response userResult = await userService.UpdateEmailAsync(user.Id, request.AdminEmail, cancellationToken)
                        .ConfigureAwait(false);
                    if (userResult.IsError)
                    {
                        return userResult;
                    }
                }

                tenant.AdminEmail = request.AdminEmail;
            }

            if (request.IsActive != tenant.IsActive)
            {
                tenant.IsActive = request.IsActive ?? tenant.IsActive;
            }

            if (request.ValidUpto != tenant.ValidUpto)
            {
                tenant.ValidUpto = request.ValidUpto.Value;
            }

            try
            {
                await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateException exception) when (IsDuplicateIdentifierConstraintViolation(exception))
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            return Response.Success();
        }

        public async Task<Response> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            Tenant? tenant = await dbContext.Tenants.IgnoreQueryFilters().AsNoTracking()
                .FirstOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);
            if (tenant is null || tenant.IsDeleted)
            {
                return Response.BadRequest("Unable to find tenant, please try again later or contact support.");
            }

            if (tenant.Id == SeedDataConstants.RootTenant.Id)
            {
                return Response.Forbidden("You cannot delete the root tenant.");
            }

            await dbContext.Tenants
                .Where(current => current.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(current => current.IsDeleted, true), cancellationToken)
                .ConfigureAwait(false);

            return Response.Success();
        }

        private static bool IsDuplicateIdentifierConstraintViolation(DbUpdateException exception)
        {
            if (exception.InnerException is SqlException { Number: 2601 or 2627 })
            {
                return true;
            }

            return exception.InnerException?.Message.Contains(
                TenantDbContext.TenantIdentifierIndexName,
                StringComparison.OrdinalIgnoreCase) == true;
        }

        private static List<TenantSetting> CreateDefaultSettings(
            string tenantId,
            SecuritySettings securitySettings,
            SMTPEmailSettings smtpEmailSettings,
            SendGridEmailSettings sendGridEmailSettings)
        {
            return
            [
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.SecurityPasswordReusePreventionWindowInDays,
                    Value = securitySettings.PasswordReusePreventionWindowInDays.ToString()
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.SecurityPasswordExpiryInDays,
                    Value = securitySettings.PasswordExpiryInDays.ToString()
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.SecurityPasswordExpiryNotificationCooldownInDays,
                    Value = securitySettings.PasswordExpiryNotificationCooldownInDays.ToString()
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailProvider,
                    Value = "smtp"
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSmtpHost,
                    Value = smtpEmailSettings.Host
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSmtpPort,
                    Value = smtpEmailSettings.Port.ToString()
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSmtpUserName,
                    Value = smtpEmailSettings.UserName
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSmtpPassword,
                    Value = smtpEmailSettings.Password,
                    IsSensitive = true
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSenderEmail,
                    Value = smtpEmailSettings.SenderEmail
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSenderName,
                    Value = smtpEmailSettings.SenderName
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailReplyToEmail,
                    Value = smtpEmailSettings.ReplyToEmail
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.EmailSendGridApiKey,
                    Value = sendGridEmailSettings.ApiKey,
                    IsSensitive = true
                }
            ];
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder tenantGroup = endpointRouteBuilder.MapGroup(ApiRoutes.Tenants)
                .AddEndpointFilter((context, next) =>
                {
                    CreateOrUpdateTenantRequest? request = context.Arguments
                        .OfType<CreateOrUpdateTenantRequest>()
                        .FirstOrDefault();

                    request?.Id = context.HttpContext.Request.RouteValues["id"]?.ToString();

                    return next(context);
                })
                .AddFluentValidationFilter();

            tenantGroup.MapGet("/", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<TenantDto>> response =
                        await handler.GetAsync(requestInput, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<TenantDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Tenants);

            tenantGroup.MapPost("/", async (
                    [FromBody] CreateOrUpdateTenantRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Tenants);

            tenantGroup.MapPatch($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromBody] CreateOrUpdateTenantRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.PatchAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Tenants);

            tenantGroup.MapDelete($"/{RouteSegment.ById}", async (
                    [FromRoute] string id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Tenants);
        }
    }
}
