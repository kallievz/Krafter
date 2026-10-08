using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class ApplicationRoleClaimConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<ApplicationRoleClaim>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
    {
        builder.ToTable("AppRoleClaim", SchemaConstants.Identity);
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
    }
}
