using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;

internal sealed class MunsoftInstanceConfiguration : IEntityTypeConfiguration<MunsoftInstance>
{
    public void Configure(EntityTypeBuilder<MunsoftInstance> builder)
    {
        builder.ToTable(nameof(MunsoftInstance), SchemaConstants.Munsoft);

        builder.HasKey(instance => instance.Id);
        builder.Property(instance => instance.Id)
            .ValueGeneratedNever();

        builder.Property(instance => instance.Name)
            .HasMaxLength(75)
            .IsUnicode(true)
            .IsRequired();

        builder.Property(instance => instance.MunicipalityName)
            .HasMaxLength(75)
            .IsUnicode(true)
            .IsRequired();

        builder.Property(instance => instance.DemarcationCode)
            .HasMaxLength(75)
            .IsUnicode(true)
            .IsRequired();

        builder.Property(instance => instance.CreatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(instance => instance.UpdatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(instance => instance.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(instance => instance.DeleteReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Ignore(instance => instance.CreatedBy);
        builder.Ignore(instance => instance.UpdatedBy);

        builder.HasIndex(instance => instance.DemarcationCode)
            .IsUnique(false);
    }
}
