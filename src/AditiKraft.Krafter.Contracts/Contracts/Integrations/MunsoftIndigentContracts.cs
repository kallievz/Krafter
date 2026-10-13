namespace AditiKraft.Krafter.Contracts.Contracts.Integrations;

public sealed class MunsoftIndigentDto
{
    public Guid Id { get; set; }
    public int InstanceId { get; set; }
    public string? MDBCode { get; set; }
    public string? Touched { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountHolderName { get; set; }
    public string? ErfExtension { get; set; }
    public string? ErfLotNumber { get; set; }
    public string? ErfSubDivision { get; set; }
    public string? ErfUnitNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? IndigentSequenceNumber { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public bool? Approved { get; set; }
    public bool? Active { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? InactivateDate { get; set; }
    public string? InactivateReason { get; set; }
    public string? IndigentName { get; set; }
    public string? IndigentIdNumber { get; set; }
    public long? NumberOfOccupants { get; set; }
    public decimal? IndigentIncome { get; set; }
    public decimal? TotalIncome { get; set; }
    public string? IndigentPercentCode { get; set; }
    public string? IndigentPercentDescription { get; set; }
    public string? ReferenceSystem { get; set; }
    public string? ReferenceTransactionId { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedById { get; set; }
}

public sealed class CreateOrUpdateMunsoftIndigentRequest
{
    public int InstanceId { get; set; }
    public string? MDBCode { get; set; }
    public string? Touched { get; set; }
    public string? AccountNumber { get; set; }
    public string? AccountHolderName { get; set; }
    public string? ErfExtension { get; set; }
    public string? ErfLotNumber { get; set; }
    public string? ErfSubDivision { get; set; }
    public string? ErfUnitNumber { get; set; }
    public string? StreetAddress { get; set; }
    public string? IndigentSequenceNumber { get; set; }
    public DateOnly? ApplicationDate { get; set; }
    public bool? Approved { get; set; }
    public bool? Active { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public DateOnly? InactivateDate { get; set; }
    public string? InactivateReason { get; set; }
    public string? IndigentName { get; set; }
    public string? IndigentIdNumber { get; set; }
    public long? NumberOfOccupants { get; set; }
    public decimal? IndigentIncome { get; set; }
    public decimal? TotalIncome { get; set; }
    public string? IndigentPercentCode { get; set; }
    public string? IndigentPercentDescription { get; set; }
    public string? ReferenceSystem { get; set; }
    public string? ReferenceTransactionId { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
}

public sealed class UpdateMunsoftIndigentProcessingRequest
{
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
}
