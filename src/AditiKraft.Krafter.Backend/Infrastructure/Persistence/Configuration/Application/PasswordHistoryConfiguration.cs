using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class PasswordHistoryConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<PasswordHistory>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<PasswordHistory> builder)
    {
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
        builder.Property(c => c.UserId).HasMaxLength(36);
        builder.Property(c => c.PasswordHash).IsRequired();
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
        builder.HasIndex(c => new { c.UserId, c.CreatedOn });
    }
}
