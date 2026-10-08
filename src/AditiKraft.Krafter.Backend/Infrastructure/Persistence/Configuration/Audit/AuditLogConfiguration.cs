using AditiKraft.Krafter.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Audit;

internal sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLog", SchemaConstants.Audit);
        builder.HasKey(a => a.Id);
        builder.Property(a => a.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
