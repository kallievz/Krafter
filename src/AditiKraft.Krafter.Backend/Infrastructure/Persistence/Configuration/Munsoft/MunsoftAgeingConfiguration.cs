using System.Reflection;
using AditiKraft.Krafter.Backend.Features.Integrations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Munsoft;

internal sealed class MunsoftAgeingConfiguration : IEntityTypeConfiguration<MunsoftAgeing>
{
    public void Configure(EntityTypeBuilder<MunsoftAgeing> builder)
    {
        builder.ToTable(nameof(MunsoftAgeing), SchemaConstants.Munsoft);

        builder.HasKey(ageing => ageing.Id);
        builder.Property(ageing => ageing.Id)
            .ValueGeneratedNever();

        builder.Property(ageing => ageing.InstanceId)
            .IsRequired();

        builder.HasIndex(ageing => new { ageing.InstanceId, ageing.AccountNumber })
            .IsUnique();

        builder.Property(ageing => ageing.DueDate)
            .HasColumnType("date");

        builder.Property(ageing => ageing.Days30)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days60)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days90)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days120)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days150)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days180)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days210)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days240)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days270)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days300)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days330)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days360)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.Days390)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.TotalOutstanding)
            .HasPrecision(18, 2);

        builder.Property(ageing => ageing.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(ageing => ageing.IsDeleted)
            .HasDefaultValue(false);

        ConfigureVarcharColumns(builder);

        builder.Ignore(ageing => ageing.CreatedBy);
        builder.Ignore(ageing => ageing.UpdatedBy);
    }

    private static void ConfigureVarcharColumns(EntityTypeBuilder<MunsoftAgeing> builder)
    {
        PropertyInfo[] stringProperties = typeof(MunsoftAgeing)
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
