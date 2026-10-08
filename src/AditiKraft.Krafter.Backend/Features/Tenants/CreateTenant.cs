using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
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
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Backend.Features.Tenants;

public sealed class CreateTenant
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
        public async Task<Response> CreateAsync(CreateOrUpdateTenantRequest request, CancellationToken cancellationToken)
        {
            if (AditiKraft.Krafter.Contracts.Common.TenantSettings.TenancyMode == TenancyMode.Single)
            {
                return Response.BadRequest("Tenant creation is not allowed in single-tenant mode.");
            }

            request.Id = null;
            ValidationResult validation = await new CreateOrUpdateTenantRequestValidator(urls).ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return Response.BadRequest(string.Join(" ", validation.Errors.Select(error => error.ErrorMessage)));
            }

            if (request.ValidUpto!.Value.Kind != DateTimeKind.Utc)
            {
                return Response.BadRequest("Send the expiry time in UTC with a Z suffix.");
            }

            bool identifierExists = await dbContext.Tenants.AsNoTracking()
                .AnyAsync(c => c.Identifier.ToLower() == request.Identifier, cancellationToken);
            if (identifierExists)
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            request.Id = request.Identifier; // Guid.CreateVersion7().ToString();
            Tenant entity = request.Adapt<Tenant>();
            entity.ValidUpto = request.ValidUpto!.Value;
            entity.CreatedOn = DateTime.UtcNow;
            entity.CreatedById = currentUser.GetUserId();

            dbContext.Tenants.Add(entity);
            dbContext.TenantSettings.AddRange(CreateDefaultSettings(entity.Id!, securitySettings.Value, smtpEmailSettings,
                sendGridEmailSettings));
            try
            {
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException exception) when (IsDuplicateIdentifierConstraintViolation(exception))
            {
                return Response.Conflict("Identifier already exists, please try a different identifier.");
            }

            using IServiceScope scope = serviceProvider.CreateScope();
            ITenantSetterService tenantSetter = scope.ServiceProvider.GetRequiredService<ITenantSetterService>();
            CurrentTenantDetails currentTenantDetails = entity.Adapt<CurrentTenantDetails>();
            currentTenantDetails.TenantLink =
                TenantLinkBuilder.GetTenantLink(urls, request.Identifier);
            tenantSetter.SetTenant(currentTenantDetails);

            DataSeedService seedService = scope.ServiceProvider.GetRequiredService<DataSeedService>();
            await seedService.SeedBasicDataAsync(new SeedDataRequest());

            return new Response();
        }

        private static bool IsDuplicateIdentifierConstraintViolation(DbUpdateException exception)
        {
            if (exception.InnerException is SqlException { Number: 2601 or 2627 })
            {
                return true;
            }

            return exception.InnerException?.Message.Contains(TenantDbContext.TenantIdentifierIndexName,
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
                .AddFluentValidationFilter();

            tenantGroup.MapPost("/", async (
                    [FromBody] CreateOrUpdateTenantRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response res = await handler.CreateAsync(request, cancellationToken);
                    return Results.Json(res, statusCode: res.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Tenants);
        }
    }
}
