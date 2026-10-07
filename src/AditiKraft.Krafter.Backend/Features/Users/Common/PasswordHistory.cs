using AditiKraft.Krafter.Backend.Common.Entities;

namespace AditiKraft.Krafter.Backend.Features.Users.Common;

public sealed class PasswordHistory : CommonEntityProperty
{
    public string UserId { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}
