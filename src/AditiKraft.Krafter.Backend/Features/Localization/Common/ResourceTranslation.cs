using AditiKraft.Krafter.Backend.Common.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;
namespace AditiKraft.Krafter.Backend.Features.Localization.Common;

public sealed class ResourceTranslation : IHistory
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();

    public string ResourceId { get; set; } = null!;
    public string LanguageId { get; set; } = null!;
    public string Value { get; set; } = string.Empty; // e.g., "Welcome"
    public Resource? Resource { get; set; }
    public Language? Language { get; set; }
    // Audit
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public ApplicationUser? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public ApplicationUser? UpdatedBy { get; set; }
    public string? UpdatedById { get; set; }
}
