using AditiKraft.Krafter.Backend.Features.Localization.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Localization;

internal sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable(nameof(Resource), SchemaConstants.Localization);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(r => r.Key)
            .HasMaxLength(200)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        builder.Property(r => r.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(r => r.CreatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(r => r.UpdatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(r => r.UpdatedOn)
            .IsRequired(false);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);

        builder.HasIndex(r => r.Key).IsUnique();
    }
}
