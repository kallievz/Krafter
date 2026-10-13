using AditiKraft.Krafter.Backend.Features.Localization.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Localization;

internal sealed class ResourceTranslationConfiguration : IEntityTypeConfiguration<ResourceTranslation>
{
    public void Configure(EntityTypeBuilder<ResourceTranslation> builder)
    {
        builder.ToTable(nameof(ResourceTranslation), SchemaConstants.Localization);

        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(rt => rt.Value)
            .HasMaxLength(2000)
            .IsUnicode(true)
            .IsRequired(false);
        builder.HasIndex(rt => new { rt.ResourceId, rt.LanguageId }).IsUnique();

        builder.Property(rt => rt.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(rt => rt.CreatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(rt => rt.UpdatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(rt => rt.UpdatedOn)
            .IsRequired(false);

        builder.Ignore(rt => rt.CreatedBy);
        builder.Ignore(rt => rt.UpdatedBy);

        builder.HasOne(rt => rt.Resource)
            .WithMany(r => r.Translations)
            .HasForeignKey(rt => rt.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rt => rt.Language)
            .WithMany(l => l.Translations)
            .HasForeignKey(rt => rt.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

