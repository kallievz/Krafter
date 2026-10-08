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

        // Delegate entity configuration to dedicated configuration classes under
        // Infrastructure/Persistence/Configuration/Audit for maintainability and parity
        // with other DbContexts.
        modelBuilder.ApplyConfiguration(new Configuration.Audit.AuditLogConfiguration());
        modelBuilder.ApplyConfiguration(new Configuration.Audit.AuditOutboxConfiguration());
    }
}
