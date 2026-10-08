using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class EmailTemplateConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<EmailTemplate>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
        builder.Property(c => c.Key).HasMaxLength(128).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(200).IsRequired();
        builder.Property(c => c.SubjectTemplate).HasMaxLength(1000).IsRequired();
        builder.Property(c => c.BodyTemplate).IsRequired();
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
        builder.HasIndex(c => new { c.TenantId, c.Key }).IsUnique();
    }
}
