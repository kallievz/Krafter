using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class EmailLogConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<EmailLog>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<EmailLog> builder)
    {
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36).IsRequired(false);
        builder.Property(c => c.ToEmail).HasMaxLength(256);
        builder.Property(c => c.Subject).HasMaxLength(500);
        builder.Property(c => c.Provider).HasMaxLength(50);
        builder.Property(c => c.Status).HasMaxLength(50);
        builder.Property(c => c.EmailTemplateId).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
        builder.HasIndex(c => new { c.TenantId, c.Status, c.CreatedOn });
        builder.HasOne(c => c.EmailTemplate)
            .WithMany(c => c.EmailLogs)
            .HasForeignKey(c => c.EmailTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
