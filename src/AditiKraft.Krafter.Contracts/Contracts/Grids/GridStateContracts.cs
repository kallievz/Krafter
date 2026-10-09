namespace AditiKraft.Krafter.Contracts.Contracts.Grids;

public sealed class GridStateDto
{
    public string Id { get; set; } = string.Empty;
    public string ComponentKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StateJson { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public sealed class CreateGridStateRequest
{
    public string ComponentKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StateJson { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}

public sealed class UpdateGridStateRequest
{
    public string Name { get; set; } = string.Empty;
    public string StateJson { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
