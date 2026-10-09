using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;

namespace AditiKraft.Krafter.Backend.Features.Localization.Common;

public sealed class Language : IHistory
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string Code { get; set; } = string.Empty; // e.g., "en"
    public string Name { get; set; } = string.Empty; // e.g., "English"
    public bool IsActive { get; set; }
    public List<ResourceTranslation> Translations { get; set; } = new();

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
