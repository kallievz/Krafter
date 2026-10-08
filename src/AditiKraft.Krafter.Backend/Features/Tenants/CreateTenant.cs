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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;

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
            catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: TenantDbContext.TenantIdentifierIndexName })
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
                    Key = TenantSettingKeys.NotificationsProvider,
                    Value = "smtp"
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpHost,
                    Value = smtpEmailSettings.Host
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpPort,
                    Value = smtpEmailSettings.Port.ToString()
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpUserName,
                    Value = smtpEmailSettings.UserName
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpPassword,
                    Value = smtpEmailSettings.Password,
                    IsSensitive = true
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpSenderEmail,
                    Value = smtpEmailSettings.SenderEmail
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpSenderName,
                    Value = smtpEmailSettings.SenderName
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSmtpReplyToEmail,
                    Value = smtpEmailSettings.ReplyToEmail
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSendGridApiKey,
                    Value = sendGridEmailSettings.ApiKey,
                    IsSensitive = true
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSendGridSenderEmail,
                    Value = sendGridEmailSettings.SenderEmail
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSendGridSenderName,
                    Value = sendGridEmailSettings.SenderName
                },
                new TenantSetting
                {
                    Id = Guid.CreateVersion7().ToString(),
                    TenantId = tenantId,
                    Key = TenantSettingKeys.NotificationsSendGridReplyToEmail,
                    Value = sendGridEmailSettings.ReplyToEmail
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
