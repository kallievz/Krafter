namespace AditiKraft.Krafter.Contracts.Contracts.Integrations;

public sealed class MunsoftAgeingDto
{
    public Guid Id { get; set; }
    public int InstanceId { get; set; }
    public string? MDBCode { get; set; }
    public string? AccountNumber { get; set; }
    public string? CurrentMonth { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? Days30 { get; set; }
    public decimal? Days60 { get; set; }
    public decimal? Days90 { get; set; }
    public decimal? Days120 { get; set; }
    public decimal? Days150 { get; set; }
    public decimal? Days180 { get; set; }
    public decimal? Days210 { get; set; }
    public decimal? Days240 { get; set; }
    public decimal? Days270 { get; set; }
    public decimal? Days300 { get; set; }
    public decimal? Days330 { get; set; }
    public decimal? Days360 { get; set; }
    public decimal? Days390 { get; set; }
    public decimal? TotalOutstanding { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedById { get; set; }
}

public sealed class CreateOrUpdateMunsoftAgeingRequest
{
    public int InstanceId { get; set; }
    public string? MDBCode { get; set; }
    public string? AccountNumber { get; set; }
    public string? CurrentMonth { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal? Days30 { get; set; }
    public decimal? Days60 { get; set; }
    public decimal? Days90 { get; set; }
    public decimal? Days120 { get; set; }
    public decimal? Days150 { get; set; }
    public decimal? Days180 { get; set; }
    public decimal? Days210 { get; set; }
    public decimal? Days240 { get; set; }
    public decimal? Days270 { get; set; }
    public decimal? Days300 { get; set; }
    public decimal? Days330 { get; set; }
    public decimal? Days360 { get; set; }
    public decimal? Days390 { get; set; }
    public decimal? TotalOutstanding { get; set; }
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
    public string? DeleteReason { get; set; }
    public bool IsDeleted { get; set; }
}

public sealed class UpdateMunsoftAgeingProcessingRequest
{
    public bool IsProcessed { get; set; }
    public bool IsUpdated { get; set; }
}
