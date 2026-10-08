using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class ApplicationUserClaimConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<ApplicationUserClaim>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.ToTable("AppUserClaim", SchemaConstants.Identity);
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
    }
}
