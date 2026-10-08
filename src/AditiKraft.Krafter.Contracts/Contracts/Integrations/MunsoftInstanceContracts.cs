namespace AditiKraft.Krafter.Contracts.Contracts.Integrations;

public sealed class MunsoftInstanceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MunicipalityName { get; set; } = string.Empty;
    public string DemarcationCode { get; set; } = string.Empty;
}

public sealed class MunsoftInstanceSyncResult
{
    public int TotalReceived { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
}
