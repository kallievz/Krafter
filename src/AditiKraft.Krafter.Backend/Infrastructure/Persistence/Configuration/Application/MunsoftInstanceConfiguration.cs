using AditiKraft.Krafter.Backend.Features.Integrations.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

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

        builder.Property(instance => instance.DeleteReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.HasOne(instance => instance.CreatedBy)
            .WithMany()
            .HasForeignKey(instance => instance.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(instance => instance.UpdatedBy)
            .WithMany()
            .HasForeignKey(instance => instance.UpdatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(instance => instance.DemarcationCode)
            .IsUnique(false);
    }
}
