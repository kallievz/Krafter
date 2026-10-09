using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Features.Localization.Common;

public sealed class Resource : IHistory
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string Key { get; set; } = string.Empty; // e.g., "HomePage_Title"
    public string Description { get; set; } = string.Empty;
    public List<ResourceTranslation> Translations { get; set; } = new();
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
