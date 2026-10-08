using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Tenant;

internal sealed class TenantSettingConfiguration : IEntityTypeConfiguration<TenantSetting>
{
    public void Configure(EntityTypeBuilder<TenantSetting> builder)
    {
        // Ensure TenantSetting is stored in the tenancy schema
        builder.ToTable(nameof(TenantSetting), SchemaConstants.Tenancy);
        builder.Property(setting => setting.Id).HasMaxLength(36);
        builder.Property(setting => setting.TenantId).HasMaxLength(36);
        builder.Property(setting => setting.Key).HasMaxLength(256);
        builder.Property(setting => setting.Value).HasMaxLength(4096);
        builder.HasIndex(setting => new { setting.TenantId, setting.Key }).IsUnique();
        builder.HasOne(setting => setting.Tenant)
            .WithMany(tenant => tenant.Settings)
            .HasForeignKey(setting => setting.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
