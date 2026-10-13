using AditiKraft.Krafter.Backend.Features.Localization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Localization;

internal sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable(nameof(Language), SchemaConstants.Localization);

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(l => l.Code)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired();

        builder.Property(l => l.Name)
            .HasMaxLength(100)
            .IsUnicode(true)
            .IsRequired();

        builder.Property(l => l.IsActive)
            .HasDefaultValue(true);

        builder.Property(l => l.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(l => l.CreatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(l => l.UpdatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(l => l.UpdatedOn)
            .IsRequired(false);

        builder.Ignore(l => l.CreatedBy);
        builder.Ignore(l => l.UpdatedBy);

        builder.HasIndex(l => l.Code).IsUnique();
    }
}
