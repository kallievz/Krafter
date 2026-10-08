using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class ExternalTokenStorageConfiguration : IEntityTypeConfiguration<ExternalTokenStorage>
{
    public void Configure(EntityTypeBuilder<ExternalTokenStorage> builder)
    {
        builder.ToTable(nameof(ExternalTokenStorage), SchemaConstants.Integration);
        builder.HasKey(token => token.Id);

        builder.Property(token => token.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(token => token.Key)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(token => token.Value)
            .HasMaxLength(4096)
            .IsRequired();

        builder.Property(token => token.ExpiresOnUtc)
            .IsRequired(false);

        builder.HasIndex(token => token.Key)
            .IsUnique();
    }
}
