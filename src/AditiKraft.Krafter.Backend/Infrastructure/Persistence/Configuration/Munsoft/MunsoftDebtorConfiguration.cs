using System.Reflection;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;

internal sealed class MunsoftDebtorConfiguration : IEntityTypeConfiguration<MunsoftDebtor>
{
    public void Configure(EntityTypeBuilder<MunsoftDebtor> builder)
    {
        builder.ToTable(nameof(MunsoftDebtor), SchemaConstants.Munsoft);

        builder.HasKey(debtor => debtor.Id);
        builder.Property(debtor => debtor.Id)
            .ValueGeneratedNever();

        builder.Property(debtor => debtor.InstanceId)
            .IsRequired();

        builder.HasIndex(debtor => new { debtor.InstanceId, debtor.AccountNumber })
            .IsUnique();

        builder.Property(debtor => debtor.DateOpened)
            .HasColumnType("date");

        builder.Property(debtor => debtor.DateClosed)
            .HasColumnType("date");

        builder.Property(debtor => debtor.DeedDate)
            .HasColumnType("date");

        builder.Property(debtor => debtor.LastPaymentDate)
            .HasColumnType("date");

        builder.Property(debtor => debtor.TotalDepositAmount)
            .HasPrecision(18, 2);

        builder.Property(debtor => debtor.OutstandingBalance)
            .HasPrecision(18, 2);

        builder.Property(debtor => debtor.LastPaymentAmount)
            .HasPrecision(18, 2);

        builder.Property(debtor => debtor.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(debtor => debtor.IsDeleted)
            .HasDefaultValue(false);

        ConfigureVarcharColumns(builder);

        builder.Ignore(debtor => debtor.CreatedBy);
        builder.Ignore(debtor => debtor.UpdatedBy);
    }

    private static void ConfigureVarcharColumns(EntityTypeBuilder<MunsoftDebtor> builder)
    {
        PropertyInfo[] stringProperties = typeof(MunsoftDebtor)
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
