using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;

internal sealed class MunsoftIndigentLogConfiguration : IEntityTypeConfiguration<MunsoftIndigentLog>
{
    public void Configure(EntityTypeBuilder<MunsoftIndigentLog> builder)
    {
        builder.ToTable(nameof(MunsoftIndigentLog), SchemaConstants.Munsoft);

        builder.HasKey(log => log.Id);
        builder.Property(log => log.Id)
            .ValueGeneratedNever();

        builder.Property(log => log.InstanceId)
            .IsRequired();

        builder.Property(log => log.RowNumber)
            .IsRequired(false);

        builder.Property(log => log.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(log => log.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(log => log.MDBCode)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.AccountNumber)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.IndigentSequenceNumber)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.EntryJson)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.ErrorMessage)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.DeleteReason)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.CreatedById)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Property(log => log.UpdatedById)
            .HasColumnType("varchar(max)")
            .IsUnicode(false)
            .IsRequired(false);

        builder.Ignore(log => log.CreatedBy);
        builder.Ignore(log => log.UpdatedBy);
    }
}
