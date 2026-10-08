using AditiKraft.Krafter.Backend.Features.Auth.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(c => c.UserId);
        builder.ToTable("UserRefreshTokens", SchemaConstants.Identity);
    }
}
