
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Tenant;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Features.Tenants.Common.Tenant>
{
    public void Configure(EntityTypeBuilder<Features.Tenants.Common.Tenant> builder)
    {
        builder.ToTable(nameof(Features.Tenants.Common.Tenant), SchemaConstants.Tenancy);
        builder.Property(tenant => tenant.Id).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false);
    }
}
