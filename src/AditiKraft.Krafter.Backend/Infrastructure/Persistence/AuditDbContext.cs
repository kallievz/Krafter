using AditiKraft.Krafter.Backend.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence;

public class AuditDbContext(DbContextOptions<AuditDbContext> options) : DbContext(options)
{
    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<AuditLog>(b =>
        {
            b.ToTable("AuditLog", SchemaConstants.Audit);
            b.HasKey(a => a.Id);
            b.Property(a => a.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
