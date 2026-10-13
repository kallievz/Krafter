using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Tenant;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Features.Tenants.Entities.Tenant>
{
    public void Configure(EntityTypeBuilder<Features.Tenants.Entities.Tenant> builder)
    {
        builder.ToTable(nameof(Features.Tenants.Entities.Tenant), SchemaConstants.Tenancy);
        builder.Property(tenant => tenant.Id).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false);
    }
}
