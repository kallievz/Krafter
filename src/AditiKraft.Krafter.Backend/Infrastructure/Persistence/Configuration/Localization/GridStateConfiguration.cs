using AditiKraft.Krafter.Backend.Features.Localization.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Localization;

internal sealed class GridStateConfiguration : IEntityTypeConfiguration<GridState>
{
    public void Configure(EntityTypeBuilder<GridState> builder)
    {
        builder.ToTable(nameof(GridState), SchemaConstants.Localization);

        builder.HasKey(gridState => gridState.Id);

        builder.Property(gridState => gridState.Id)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(gridState => gridState.TenantId)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(gridState => gridState.UserId)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(gridState => gridState.ComponentKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(gridState => gridState.Name)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(gridState => gridState.StateJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(gridState => gridState.CreatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(gridState => gridState.UpdatedById)
            .HasMaxLength(36)
            .IsRequired(false);

        builder.Property(gridState => gridState.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Ignore(gridState => gridState.CreatedBy);
        builder.Ignore(gridState => gridState.UpdatedBy);

        builder.HasIndex(gridState => new { gridState.TenantId, gridState.UserId, gridState.ComponentKey, gridState.Name })
            .IsUnique();

        builder.HasIndex(gridState => new { gridState.TenantId, gridState.UserId, gridState.ComponentKey, gridState.IsDefault });
    }
}
