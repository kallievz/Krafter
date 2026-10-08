using System.Security.Claims;
using AditiKraft.Krafter.Backend.Common.Attributes;
using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AditiKraft.Krafter.Tests.Persistence;

public sealed class AuditInterceptorDedicatedDbTests
{
    private class TestEntity : AditiKraft.Krafter.Backend.Common.Entities.ICommonAuthEntityProperty
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsDeleted { get; set; }
        public string? DeleteReason { get; set; }
        public DateTime CreatedOn { get; set; }
        public AditiKraft.Krafter.Backend.Features.Users.Common.ApplicationUser? CreatedBy { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public AditiKraft.Krafter.Backend.Features.Users.Common.ApplicationUser? UpdatedBy { get; set; }
        public string? UpdatedById { get; set; }
        public string TenantId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        [AuditIgnore]
        public string Secret { get; set; } = string.Empty;
    }

    [Fact]
    public async Task Adding_entity_creates_audit_entry_in_dedicated_db_and_respects_AuditIgnore()
    {
        var currentUser = new CurrentUser();
        currentUser.SetCurrentUserId("user-123");

        var tenantService = new CurrentTenantService();
        tenantService.SetTenant(new AditiKraft.Krafter.Contracts.Common.Models.CurrentTenantDetails { Id = "tenant-1", TenantLink = "http://local" });

        // Prepare a service provider with AuditDbContext registered using the same in-memory database name
        string dbName = Guid.NewGuid().ToString();
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        services.AddDbContext<AuditDbContext>(opts => opts.UseInMemoryDatabase(dbName));
        var serviceProvider = services.BuildServiceProvider();

        // Create interceptor with metadata cache (no external scope factory needed)
        var metadataCache = new AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services.AuditMetadataCache();
        var interceptor = new AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor(currentUser, tenantService, metadataCache);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors(interceptor)
            .Options;

        await using var db = new ApplicationDbContext(options, currentUser, tenantService);

        var entity = new TestEntity { Name = "Alice", Secret = "top-secret" };
        db.Add(entity);
        await db.SaveChangesAsync();

        // After SaveChanges, an outbox row should be created in ApplicationDbContext
        var outbox = await db.AuditOutbox.FirstOrDefaultAsync();
        Assert.NotNull(outbox);
        Assert.NotNull(outbox.Payload);
        Assert.Contains("Name", outbox.Payload);
        Assert.DoesNotContain("Secret", outbox.Payload);

        // Simulate the outbox processor by delivering to the AuditDbContext
        await using var auditDb = serviceProvider.GetRequiredService<AuditDbContext>();
        var audit = System.Text.Json.JsonSerializer.Deserialize<AditiKraft.Krafter.Backend.Common.Entities.AuditLog>(outbox.Payload);
        Assert.NotNull(audit);
        auditDb.AuditLogs.Add(audit);
        outbox.ProcessedOn = DateTime.UtcNow;
        db.AuditOutbox.Update(outbox);
        await auditDb.SaveChangesAsync();
        await db.SaveChangesAsync();

        var persisted = await auditDb.AuditLogs.FirstOrDefaultAsync();
        Assert.NotNull(persisted);
        Assert.Equal("Added", persisted.Action);
        Assert.Equal("user-123", persisted.UserId);
        Assert.Equal("tenant-1", persisted.TenantId);
        Assert.Contains("Name", persisted.NewValues);
        Assert.DoesNotContain("Secret", persisted.NewValues);
    }
}
