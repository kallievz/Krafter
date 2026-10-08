using AditiKraft.Krafter.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Audit;

internal sealed class AuditOutboxConfiguration : IEntityTypeConfiguration<AuditOutbox>
{
    public void Configure(EntityTypeBuilder<AuditOutbox> builder)
    {
        builder.ToTable("AuditOutbox", SchemaConstants.Audit);
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Payload).IsRequired();
        builder.Property(o => o.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(o => o.ProcessedOn);
        builder.Property(o => o.FailureCount).HasDefaultValue(0);
        builder.Property(o => o.LastError);
    }
}
