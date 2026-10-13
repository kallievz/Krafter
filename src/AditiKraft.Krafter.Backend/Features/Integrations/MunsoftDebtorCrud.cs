using System.Linq.Dynamic.Core;
using AditiKraft.Krafter.Backend.Common.Extensions;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
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

public sealed class MunsoftDebtorCrud
{
    internal sealed class Handler(MunsoftDbContext dbContext) : IScopedHandler
    {
        public async Task<Response<PaginationResponse<MunsoftDebtorDto>>> GetAsync(GetRequestInput requestInput, CancellationToken cancellationToken)
        {
            IQueryable<MunsoftDebtorDto> query = dbContext.MunsoftDebtors
                .AsNoTracking()
                .Where(entity => entity.IsDeleted == requestInput.IsDeleted)
                .Select(entity => new MunsoftDebtorDto
                {
                    Id = entity.Id,
                    InstanceId = entity.InstanceId,
                    MDBCode = entity.MDBCode,
                    AccountNumber = entity.AccountNumber,
                    AccountHolderName = entity.AccountHolderName,
                    ErfExtension = entity.ErfExtension,
                    ErfLotNumber = entity.ErfLotNumber,
                    ErfSubDivision = entity.ErfSubDivision,
                    ErfUnitNumber = entity.ErfUnitNumber,
                    StreetAddress = entity.StreetAddress,
                    Surname = entity.Surname,
                    Name = entity.Name,
                    Initials = entity.Initials,
                    PostAddress1 = entity.PostAddress1,
                    PostAddress2 = entity.PostAddress2,
                    PostAddress3 = entity.PostAddress3,
                    PostAddress4 = entity.PostAddress4,
                    PostAddress5 = entity.PostAddress5,
                    PostAddress6 = entity.PostAddress6,
                    PostCode = entity.PostCode,
                    WorkAddress1 = entity.WorkAddress1,
                    WorkAddress2 = entity.WorkAddress2,
                    WorkAddress3 = entity.WorkAddress3,
                    WorkAddress4 = entity.WorkAddress4,
                    WorkAddress5 = entity.WorkAddress5,
                    WorkAddress6 = entity.WorkAddress6,
                    WorkCode = entity.WorkCode,
                    HomeTelNumber = entity.HomeTelNumber,
                    WorkTelNumber = entity.WorkTelNumber,
                    FaxNumber = entity.FaxNumber,
                    CellNumber = entity.CellNumber,
                    CellNumber2 = entity.CellNumber2,
                    IdNumber1 = entity.IdNumber1,
                    IdNumber2 = entity.IdNumber2,
                    PasswordNumber = entity.PasswordNumber,
                    EmailAddress1 = entity.EmailAddress1,
                    EmailAddress2 = entity.EmailAddress2,
                    CoCcNumber = entity.CoCcNumber,
                    VatRegNumber = entity.VatRegNumber,
                    RaceCode = entity.RaceCode,
                    RaceDescription = entity.RaceDescription,
                    LanguageCode = entity.LanguageCode,
                    LanguageDescription = entity.LanguageDescription,
                    MaritalStatusCode = entity.MaritalStatusCode,
                    MaritalStatusDescription = entity.MaritalStatusDescription,
                    CitizenshipCode = entity.CitizenshipCode,
                    CitizenshipDescription = entity.CitizenshipDescription,
                    EasyPayNumber = entity.EasyPayNumber,
                    UniPay = entity.UniPay,
                    PayAt = entity.PayAt,
                    AccountStatusCode = entity.AccountStatusCode,
                    AccountStatusDescription = entity.AccountStatusDescription,
                    AccountTypeCode = entity.AccountTypeCode,
                    AccountTypeDescription = entity.AccountTypeDescription,
                    SubAccountTypeCode = entity.SubAccountTypeCode,
                    SubAccountTypeDescription = entity.SubAccountTypeDescription,
                    OwnerTypeCode = entity.OwnerTypeCode,
                    OwnerTypeDescription = entity.OwnerTypeDescription,
                    CategoryCode = entity.CategoryCode,
                    CategoryDescription = entity.CategoryDescription,
                    CreditStatusCode = entity.CreditStatusCode,
                    CreditStatusDescription = entity.CreditStatusDescription,
                    CreditInstructionCode = entity.CreditInstructionCode,
                    CreditInstructionDescription = entity.CreditInstructionDescription,
                    GroupAccountCode = entity.GroupAccountCode,
                    GroupAccountDescription = entity.GroupAccountDescription,
                    GroupAccountNumber = entity.GroupAccountNumber,
                    MailingInstructionCode = entity.MailingInstructionCode,
                    MailingInstrDescription = entity.MailingInstrDescription,
                    NextOfKinName = entity.NextOfKinName,
                    NextOfKinAddress1 = entity.NextOfKinAddress1,
                    NextOfKinAddress2 = entity.NextOfKinAddress2,
                    NextOfKinAddress3 = entity.NextOfKinAddress3,
                    NextOfKinAddress4 = entity.NextOfKinAddress4,
                    NextOfKinCode = entity.NextOfKinCode,
                    NextOfKinTelNumber = entity.NextOfKinTelNumber,
                    NextOfKinRelationship = entity.NextOfKinRelationship,
                    OldAccountNumber = entity.OldAccountNumber,
                    DateOpened = entity.DateOpened,
                    DateClosed = entity.DateClosed,
                    Notes1 = entity.Notes1,
                    Notes2 = entity.Notes2,
                    ProvinceCode = entity.ProvinceCode,
                    ProvinceDescription = entity.ProvinceDescription,
                    TownCode = entity.TownCode,
                    TownDescription = entity.TownDescription,
                    SuburbCode = entity.SuburbCode,
                    SuburbDescription = entity.SuburbDescription,
                    WardCode = entity.WardCode,
                    WardDescription = entity.WardDescription,
                    ErfLaCode = entity.ErfLaCode,
                    ErfLaDescription = entity.ErfLaDescription,
                    WardCouncillor = entity.WardCouncillor,
                    StreetName = entity.StreetName,
                    StreetNumber = entity.StreetNumber,
                    Directions = entity.Directions,
                    PropertyCategoryCode = entity.PropertyCategoryCode,
                    PropertyCategoryDescription = entity.PropertyCategoryDescription,
                    UnitNameNumber = entity.UnitNameNumber,
                    ZoningCode = entity.ZoningCode,
                    UsageCode = entity.UsageCode,
                    SurveyorLot = entity.SurveyorLot,
                    GisKey = entity.GisKey,
                    Gps = entity.Gps,
                    DeedNumber = entity.DeedNumber,
                    DeedDate = entity.DeedDate,
                    ErfReference = entity.ErfReference,
                    TotalDepositAmount = entity.TotalDepositAmount,
                    OutstandingBalance = entity.OutstandingBalance,
                    LastPaymentAmount = entity.LastPaymentAmount,
                    LastPaymentDate = entity.LastPaymentDate,
                    PortalPinNumber = entity.PortalPinNumber,
                    EmployeeNumber = entity.EmployeeNumber,
                    ExcludeIntYn = entity.ExcludeIntYn,
                    ExcludeAutoDepositYn = entity.ExcludeAutoDepositYn,
                    EmployeeSponsor = entity.EmployeeSponsor,
                    PayrollLineNumber = entity.PayrollLineNumber,
                    IsProcessed = entity.IsProcessed,
                    IsUpdated = entity.IsUpdated,
                    DeleteReason = entity.DeleteReason,
                    IsDeleted = entity.IsDeleted,
                    CreatedOn = entity.CreatedOn,
                    CreatedById = entity.CreatedById,
                    UpdatedOn = entity.UpdatedOn,
                    UpdatedById = entity.UpdatedById
                });

            if (!string.IsNullOrWhiteSpace(requestInput.Filter))
            {
                query = query.Where(requestInput.Filter);
            }

            query = !string.IsNullOrWhiteSpace(requestInput.OrderBy)
                ? query.OrderBy(requestInput.OrderBy)
                : query.OrderBy(entity => entity.CreatedOn);

            List<MunsoftDebtorDto> items = await query.PageBy(requestInput).ToListAsync(cancellationToken).ConfigureAwait(false);
            int totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            return Response<PaginationResponse<MunsoftDebtorDto>>.Success(
                new PaginationResponse<MunsoftDebtorDto>(items, totalCount, requestInput.SkipCount, requestInput.MaxResultCount));
        }

        public async Task<Response<MunsoftDebtorDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftDebtorDto? entity = await dbContext.MunsoftDebtors
                .AsNoTracking()
                .Where(current => current.Id == id)
                .Select(current => new MunsoftDebtorDto
                {
                    Id = current.Id,
                    InstanceId = current.InstanceId,
                    MDBCode = current.MDBCode,
                    AccountNumber = current.AccountNumber,
                    AccountHolderName = current.AccountHolderName,
                    ErfExtension = current.ErfExtension,
                    ErfLotNumber = current.ErfLotNumber,
                    ErfSubDivision = current.ErfSubDivision,
                    ErfUnitNumber = current.ErfUnitNumber,
                    StreetAddress = current.StreetAddress,
                    Surname = current.Surname,
                    Name = current.Name,
                    Initials = current.Initials,
                    PostAddress1 = current.PostAddress1,
                    PostAddress2 = current.PostAddress2,
                    PostAddress3 = current.PostAddress3,
                    PostAddress4 = current.PostAddress4,
                    PostAddress5 = current.PostAddress5,
                    PostAddress6 = current.PostAddress6,
                    PostCode = current.PostCode,
                    WorkAddress1 = current.WorkAddress1,
                    WorkAddress2 = current.WorkAddress2,
                    WorkAddress3 = current.WorkAddress3,
                    WorkAddress4 = current.WorkAddress4,
                    WorkAddress5 = current.WorkAddress5,
                    WorkAddress6 = current.WorkAddress6,
                    WorkCode = current.WorkCode,
                    HomeTelNumber = current.HomeTelNumber,
                    WorkTelNumber = current.WorkTelNumber,
                    FaxNumber = current.FaxNumber,
                    CellNumber = current.CellNumber,
                    CellNumber2 = current.CellNumber2,
                    IdNumber1 = current.IdNumber1,
                    IdNumber2 = current.IdNumber2,
                    PasswordNumber = current.PasswordNumber,
                    EmailAddress1 = current.EmailAddress1,
                    EmailAddress2 = current.EmailAddress2,
                    CoCcNumber = current.CoCcNumber,
                    VatRegNumber = current.VatRegNumber,
                    RaceCode = current.RaceCode,
                    RaceDescription = current.RaceDescription,
                    LanguageCode = current.LanguageCode,
                    LanguageDescription = current.LanguageDescription,
                    MaritalStatusCode = current.MaritalStatusCode,
                    MaritalStatusDescription = current.MaritalStatusDescription,
                    CitizenshipCode = current.CitizenshipCode,
                    CitizenshipDescription = current.CitizenshipDescription,
                    EasyPayNumber = current.EasyPayNumber,
                    UniPay = current.UniPay,
                    PayAt = current.PayAt,
                    AccountStatusCode = current.AccountStatusCode,
                    AccountStatusDescription = current.AccountStatusDescription,
                    AccountTypeCode = current.AccountTypeCode,
                    AccountTypeDescription = current.AccountTypeDescription,
                    SubAccountTypeCode = current.SubAccountTypeCode,
                    SubAccountTypeDescription = current.SubAccountTypeDescription,
                    OwnerTypeCode = current.OwnerTypeCode,
                    OwnerTypeDescription = current.OwnerTypeDescription,
                    CategoryCode = current.CategoryCode,
                    CategoryDescription = current.CategoryDescription,
                    CreditStatusCode = current.CreditStatusCode,
                    CreditStatusDescription = current.CreditStatusDescription,
                    CreditInstructionCode = current.CreditInstructionCode,
                    CreditInstructionDescription = current.CreditInstructionDescription,
                    GroupAccountCode = current.GroupAccountCode,
                    GroupAccountDescription = current.GroupAccountDescription,
                    GroupAccountNumber = current.GroupAccountNumber,
                    MailingInstructionCode = current.MailingInstructionCode,
                    MailingInstrDescription = current.MailingInstrDescription,
                    NextOfKinName = current.NextOfKinName,
                    NextOfKinAddress1 = current.NextOfKinAddress1,
                    NextOfKinAddress2 = current.NextOfKinAddress2,
                    NextOfKinAddress3 = current.NextOfKinAddress3,
                    NextOfKinAddress4 = current.NextOfKinAddress4,
                    NextOfKinCode = current.NextOfKinCode,
                    NextOfKinTelNumber = current.NextOfKinTelNumber,
                    NextOfKinRelationship = current.NextOfKinRelationship,
                    OldAccountNumber = current.OldAccountNumber,
                    DateOpened = current.DateOpened,
                    DateClosed = current.DateClosed,
                    Notes1 = current.Notes1,
                    Notes2 = current.Notes2,
                    ProvinceCode = current.ProvinceCode,
                    ProvinceDescription = current.ProvinceDescription,
                    TownCode = current.TownCode,
                    TownDescription = current.TownDescription,
                    SuburbCode = current.SuburbCode,
                    SuburbDescription = current.SuburbDescription,
                    WardCode = current.WardCode,
                    WardDescription = current.WardDescription,
                    ErfLaCode = current.ErfLaCode,
                    ErfLaDescription = current.ErfLaDescription,
                    WardCouncillor = current.WardCouncillor,
                    StreetName = current.StreetName,
                    StreetNumber = current.StreetNumber,
                    Directions = current.Directions,
                    PropertyCategoryCode = current.PropertyCategoryCode,
                    PropertyCategoryDescription = current.PropertyCategoryDescription,
                    UnitNameNumber = current.UnitNameNumber,
                    ZoningCode = current.ZoningCode,
                    UsageCode = current.UsageCode,
                    SurveyorLot = current.SurveyorLot,
                    GisKey = current.GisKey,
                    Gps = current.Gps,
                    DeedNumber = current.DeedNumber,
                    DeedDate = current.DeedDate,
                    ErfReference = current.ErfReference,
                    TotalDepositAmount = current.TotalDepositAmount,
                    OutstandingBalance = current.OutstandingBalance,
                    LastPaymentAmount = current.LastPaymentAmount,
                    LastPaymentDate = current.LastPaymentDate,
                    PortalPinNumber = current.PortalPinNumber,
                    EmployeeNumber = current.EmployeeNumber,
                    ExcludeIntYn = current.ExcludeIntYn,
                    ExcludeAutoDepositYn = current.ExcludeAutoDepositYn,
                    EmployeeSponsor = current.EmployeeSponsor,
                    PayrollLineNumber = current.PayrollLineNumber,
                    IsProcessed = current.IsProcessed,
                    IsUpdated = current.IsUpdated,
                    DeleteReason = current.DeleteReason,
                    IsDeleted = current.IsDeleted,
                    CreatedOn = current.CreatedOn,
                    CreatedById = current.CreatedById,
                    UpdatedOn = current.UpdatedOn,
                    UpdatedById = current.UpdatedById
                })
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return entity is null
                ? Response<MunsoftDebtorDto>.NotFound("MunsoftDebtor Not Found")
                : Response<MunsoftDebtorDto>.Success(entity);
        }

        public async Task<Response> CreateAsync(CreateOrUpdateMunsoftDebtorRequest request, CancellationToken cancellationToken)
        {
            MunsoftDebtor entity = new MunsoftDebtor();
            ApplyRequest(entity, request);
            entity.Id = Guid.CreateVersion7();

            await dbContext.MunsoftDebtors.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateAsync(Guid id, CreateOrUpdateMunsoftDebtorRequest request, CancellationToken cancellationToken)
        {
            MunsoftDebtor? entity = await dbContext.MunsoftDebtors
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftDebtor Not Found");
            }

            ApplyRequest(entity, request);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            MunsoftDebtor? entity = await dbContext.MunsoftDebtors
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftDebtor Not Found");
            }

            dbContext.MunsoftDebtors.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        public async Task<Response> UpdateProcessingAsync(Guid id, UpdateMunsoftDebtorProcessingRequest request, CancellationToken cancellationToken)
        {
            MunsoftDebtor? entity = await dbContext.MunsoftDebtors
                .SingleOrDefaultAsync(current => current.Id == id, cancellationToken)
                .ConfigureAwait(false);

            if (entity is null)
            {
                return Response.NotFound("MunsoftDebtor Not Found");
            }

            entity.IsProcessed = request.IsProcessed;
            entity.IsUpdated = request.IsUpdated;

            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Response.Success();
        }

        private static void ApplyRequest(MunsoftDebtor entity, CreateOrUpdateMunsoftDebtorRequest request)
        {
            entity.InstanceId = request.InstanceId;
            entity.MDBCode = request.MDBCode;
            entity.AccountNumber = request.AccountNumber;
            entity.AccountHolderName = request.AccountHolderName;
            entity.ErfExtension = request.ErfExtension;
            entity.ErfLotNumber = request.ErfLotNumber;
            entity.ErfSubDivision = request.ErfSubDivision;
            entity.ErfUnitNumber = request.ErfUnitNumber;
            entity.StreetAddress = request.StreetAddress;
            entity.Surname = request.Surname;
            entity.Name = request.Name;
            entity.Initials = request.Initials;
            entity.PostAddress1 = request.PostAddress1;
            entity.PostAddress2 = request.PostAddress2;
            entity.PostAddress3 = request.PostAddress3;
            entity.PostAddress4 = request.PostAddress4;
            entity.PostAddress5 = request.PostAddress5;
            entity.PostAddress6 = request.PostAddress6;
            entity.PostCode = request.PostCode;
            entity.WorkAddress1 = request.WorkAddress1;
            entity.WorkAddress2 = request.WorkAddress2;
            entity.WorkAddress3 = request.WorkAddress3;
            entity.WorkAddress4 = request.WorkAddress4;
            entity.WorkAddress5 = request.WorkAddress5;
            entity.WorkAddress6 = request.WorkAddress6;
            entity.WorkCode = request.WorkCode;
            entity.HomeTelNumber = request.HomeTelNumber;
            entity.WorkTelNumber = request.WorkTelNumber;
            entity.FaxNumber = request.FaxNumber;
            entity.CellNumber = request.CellNumber;
            entity.CellNumber2 = request.CellNumber2;
            entity.IdNumber1 = request.IdNumber1;
            entity.IdNumber2 = request.IdNumber2;
            entity.PasswordNumber = request.PasswordNumber;
            entity.EmailAddress1 = request.EmailAddress1;
            entity.EmailAddress2 = request.EmailAddress2;
            entity.CoCcNumber = request.CoCcNumber;
            entity.VatRegNumber = request.VatRegNumber;
            entity.RaceCode = request.RaceCode;
            entity.RaceDescription = request.RaceDescription;
            entity.LanguageCode = request.LanguageCode;
            entity.LanguageDescription = request.LanguageDescription;
            entity.MaritalStatusCode = request.MaritalStatusCode;
            entity.MaritalStatusDescription = request.MaritalStatusDescription;
            entity.CitizenshipCode = request.CitizenshipCode;
            entity.CitizenshipDescription = request.CitizenshipDescription;
            entity.EasyPayNumber = request.EasyPayNumber;
            entity.UniPay = request.UniPay;
            entity.PayAt = request.PayAt;
            entity.AccountStatusCode = request.AccountStatusCode;
            entity.AccountStatusDescription = request.AccountStatusDescription;
            entity.AccountTypeCode = request.AccountTypeCode;
            entity.AccountTypeDescription = request.AccountTypeDescription;
            entity.SubAccountTypeCode = request.SubAccountTypeCode;
            entity.SubAccountTypeDescription = request.SubAccountTypeDescription;
            entity.OwnerTypeCode = request.OwnerTypeCode;
            entity.OwnerTypeDescription = request.OwnerTypeDescription;
            entity.CategoryCode = request.CategoryCode;
            entity.CategoryDescription = request.CategoryDescription;
            entity.CreditStatusCode = request.CreditStatusCode;
            entity.CreditStatusDescription = request.CreditStatusDescription;
            entity.CreditInstructionCode = request.CreditInstructionCode;
            entity.CreditInstructionDescription = request.CreditInstructionDescription;
            entity.GroupAccountCode = request.GroupAccountCode;
            entity.GroupAccountDescription = request.GroupAccountDescription;
            entity.GroupAccountNumber = request.GroupAccountNumber;
            entity.MailingInstructionCode = request.MailingInstructionCode;
            entity.MailingInstrDescription = request.MailingInstrDescription;
            entity.NextOfKinName = request.NextOfKinName;
            entity.NextOfKinAddress1 = request.NextOfKinAddress1;
            entity.NextOfKinAddress2 = request.NextOfKinAddress2;
            entity.NextOfKinAddress3 = request.NextOfKinAddress3;
            entity.NextOfKinAddress4 = request.NextOfKinAddress4;
            entity.NextOfKinCode = request.NextOfKinCode;
            entity.NextOfKinTelNumber = request.NextOfKinTelNumber;
            entity.NextOfKinRelationship = request.NextOfKinRelationship;
            entity.OldAccountNumber = request.OldAccountNumber;
            entity.DateOpened = request.DateOpened;
            entity.DateClosed = request.DateClosed;
            entity.Notes1 = request.Notes1;
            entity.Notes2 = request.Notes2;
            entity.ProvinceCode = request.ProvinceCode;
            entity.ProvinceDescription = request.ProvinceDescription;
            entity.TownCode = request.TownCode;
            entity.TownDescription = request.TownDescription;
            entity.SuburbCode = request.SuburbCode;
            entity.SuburbDescription = request.SuburbDescription;
            entity.WardCode = request.WardCode;
            entity.WardDescription = request.WardDescription;
            entity.ErfLaCode = request.ErfLaCode;
            entity.ErfLaDescription = request.ErfLaDescription;
            entity.WardCouncillor = request.WardCouncillor;
            entity.StreetName = request.StreetName;
            entity.StreetNumber = request.StreetNumber;
            entity.Directions = request.Directions;
            entity.PropertyCategoryCode = request.PropertyCategoryCode;
            entity.PropertyCategoryDescription = request.PropertyCategoryDescription;
            entity.UnitNameNumber = request.UnitNameNumber;
            entity.ZoningCode = request.ZoningCode;
            entity.UsageCode = request.UsageCode;
            entity.SurveyorLot = request.SurveyorLot;
            entity.GisKey = request.GisKey;
            entity.Gps = request.Gps;
            entity.DeedNumber = request.DeedNumber;
            entity.DeedDate = request.DeedDate;
            entity.ErfReference = request.ErfReference;
            entity.TotalDepositAmount = request.TotalDepositAmount;
            entity.OutstandingBalance = request.OutstandingBalance;
            entity.LastPaymentAmount = request.LastPaymentAmount;
            entity.LastPaymentDate = request.LastPaymentDate;
            entity.PortalPinNumber = request.PortalPinNumber;
            entity.EmployeeNumber = request.EmployeeNumber;
            entity.ExcludeIntYn = request.ExcludeIntYn;
            entity.ExcludeAutoDepositYn = request.ExcludeAutoDepositYn;
            entity.EmployeeSponsor = request.EmployeeSponsor;
            entity.PayrollLineNumber = request.PayrollLineNumber;
            entity.IsProcessed = request.IsProcessed;
            entity.IsUpdated = request.IsUpdated;
            entity.DeleteReason = request.DeleteReason;
            entity.IsDeleted = request.IsDeleted;
        }
    }

    public sealed class Route : IRouteRegistrar
    {
        public void MapRoute(IEndpointRouteBuilder endpointRouteBuilder)
        {
            RouteGroupBuilder group = endpointRouteBuilder.MapGroup(ApiRoutes.Integrations)
                .AddFluentValidationFilter();

            group.MapGet("/munsoft/debtors", async (
                    [FromServices] Handler handler,
                    [AsParameters] GetRequestInput requestInput,
                    CancellationToken cancellationToken) =>
                {
                    Response<PaginationResponse<MunsoftDebtorDto>> response = await handler.GetAsync(requestInput, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<PaginationResponse<MunsoftDebtorDto>>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapGet("/munsoft/debtors/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response<MunsoftDebtorDto> response = await handler.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response<MunsoftDebtorDto>>()
                .MustHavePermission(PermissionAction.View, PermissionResource.Integrations);

            group.MapPost("/munsoft/debtors", async (
                    [FromBody] CreateOrUpdateMunsoftDebtorRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.CreateAsync(request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Create, PermissionResource.Integrations);

            group.MapPatch("/munsoft/debtors/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromBody] CreateOrUpdateMunsoftDebtorRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);

            group.MapDelete("/munsoft/debtors/{id:guid}", async (
                    [FromRoute] Guid id,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Delete, PermissionResource.Integrations);

            group.MapPatch("/munsoft/debtors/{id:guid}/processing", async (
                    [FromRoute] Guid id,
                    [FromBody] UpdateMunsoftDebtorProcessingRequest request,
                    [FromServices] Handler handler,
                    CancellationToken cancellationToken) =>
                {
                    Response response = await handler.UpdateProcessingAsync(id, request, cancellationToken).ConfigureAwait(false);
                    return Results.Json(response, statusCode: response.StatusCode);
                })
                .Produces<Response>()
                .MustHavePermission(PermissionAction.Update, PermissionResource.Integrations);
        }
    }
}
