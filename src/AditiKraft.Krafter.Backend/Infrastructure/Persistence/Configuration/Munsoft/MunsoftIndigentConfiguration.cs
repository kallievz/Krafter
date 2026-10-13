using System.Reflection;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;

internal sealed class MunsoftIndigentConfiguration : IEntityTypeConfiguration<MunsoftIndigent>
{
    public void Configure(EntityTypeBuilder<MunsoftIndigent> builder)
    {
        builder.ToTable(nameof(MunsoftIndigent), SchemaConstants.Munsoft);

        builder.HasKey(indigent => indigent.Id);
        builder.Property(indigent => indigent.Id)
            .ValueGeneratedNever();

        builder.Property(indigent => indigent.InstanceId)
            .IsRequired();

        builder.HasIndex(indigent => new { indigent.InstanceId, indigent.AccountNumber, indigent.IndigentSequenceNumber })
            .IsUnique();

        builder.Property(indigent => indigent.ApplicationDate)
            .HasColumnType("date");

        builder.Property(indigent => indigent.StartDate)
            .HasColumnType("date");

        builder.Property(indigent => indigent.ExpiryDate)
            .HasColumnType("date");

        builder.Property(indigent => indigent.InactivateDate)
            .HasColumnType("date");

        builder.Property(indigent => indigent.IndigentIncome)
            .HasPrecision(18, 2);

        builder.Property(indigent => indigent.TotalIncome)
            .HasPrecision(18, 2);

        builder.Property(indigent => indigent.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(indigent => indigent.IsDeleted)
            .HasDefaultValue(false);

        ConfigureVarcharColumns(builder);

        builder.Ignore(indigent => indigent.CreatedBy);
        builder.Ignore(indigent => indigent.UpdatedBy);
    }

    private static void ConfigureVarcharColumns(EntityTypeBuilder<MunsoftIndigent> builder)
    {
        PropertyInfo[] stringProperties = typeof(MunsoftIndigent)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.PropertyType == typeof(string))
            .ToArray();

        foreach (PropertyInfo property in stringProperties)
        {
            builder.Property(property.Name)
                .HasColumnType("varchar(max)")
                .IsUnicode(false)
                .IsRequired(false);
        }
    }
}
