using AditiKraft.Krafter.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<AuditOutbox> AuditOutbox { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(b =>
        {
            b.ToTable("AuditLog", SchemaConstants.Audit);
            b.HasKey(a => a.Id);
            b.Property(a => a.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Explicitly map AuditOutbox to the audit schema so migrations include the table.
        modelBuilder.Entity<AuditOutbox>(b =>
        {
            b.ToTable("AuditOutbox", SchemaConstants.Audit);
            b.HasKey(o => o.Id);
            b.Property(o => o.Payload).IsRequired();
            b.Property(o => o.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
            b.Property(o => o.ProcessedOn);
            b.Property(o => o.FailureCount).HasDefaultValue(0);
            b.Property(o => o.LastError);
        });
    }
}
