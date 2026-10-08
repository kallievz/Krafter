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
        CurrentUser currentUser = new();
        currentUser.SetCurrentUserId("user-123");

        CurrentTenantService tenantService = new();
        tenantService.SetTenant(new AditiKraft.Krafter.Contracts.Common.Models.CurrentTenantDetails { Id = "tenant-1", TenantLink = "http://local" });

        // Prepare a service provider with AuditDbContext registered using the same in-memory database name
        string dbName = Guid.NewGuid().ToString();
        Microsoft.Extensions.DependencyInjection.ServiceCollection services = new();
        services.AddDbContext<AuditDbContext>(opts => opts.UseInMemoryDatabase(dbName));
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // Create interceptor with metadata cache (no external scope factory needed)
        AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services.AuditMetadataCache metadataCache = new();
        AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor interceptor = new(currentUser, tenantService, metadataCache);

        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors(interceptor)
            .Options;

        await using ApplicationDbContext db = new(options, currentUser, tenantService);

        TestEntity entity = new() { Name = "Alice", Secret = "top-secret" };
        db.Add(entity);
        await db.SaveChangesAsync();

        // After SaveChanges, an outbox row should be created in ApplicationDbContext
        AuditOutbox? outbox = await db.AuditOutbox.FirstOrDefaultAsync();
        Assert.NotNull(outbox);
        Assert.NotNull(outbox.Payload);
        Assert.Contains("Name", outbox.Payload);
        Assert.DoesNotContain("Secret", outbox.Payload);

        // Simulate the outbox processor by delivering to the AuditDbContext
        await using AuditDbContext auditDb = serviceProvider.GetRequiredService<AuditDbContext>();
        AuditLog? audit = System.Text.Json.JsonSerializer.Deserialize<AditiKraft.Krafter.Backend.Common.Entities.AuditLog>(outbox.Payload);
        Assert.NotNull(audit);
        auditDb.AuditLogs.Add(audit);
        outbox.ProcessedOn = DateTime.UtcNow;
        db.AuditOutbox.Update(outbox);
        await auditDb.SaveChangesAsync();
        await db.SaveChangesAsync();

        AuditLog? persisted = await auditDb.AuditLogs.FirstOrDefaultAsync();
        Assert.NotNull(persisted);
        Assert.Equal("Added", persisted.Action);
        Assert.Equal("user-123", persisted.UserId);
        Assert.Equal("tenant-1", persisted.TenantId);
        Assert.Contains("Name", persisted.NewValues);
        Assert.DoesNotContain("Secret", persisted.NewValues);
    }

    [Fact]
    public async Task Updating_tenant_name_creates_audit_outbox_entry()
    {
        CurrentUser currentUser = new();
        currentUser.SetCurrentUserId("user-123");

        CurrentTenantService tenantService = new();
        tenantService.SetTenant(new AditiKrafter.Contracts.Common.Models.CurrentTenantDetails { Id = "tenant-1", TenantLink = "http://local" });

        string dbName = Guid.NewGuid().ToString();
        Microsoft.Extensions.DependencyInjection.ServiceCollection services = new();
        services.AddDbContext<AuditDbContext>(opts => opts.UseInMemoryDatabase($"{dbName}-audit"));
        ServiceProvider serviceProvider = services.BuildServiceProvider();

        AditiKraft.Krafter.Backend.Infrastructure.Persistence.Services.AuditMetadataCache metadataCache = new();
        AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor interceptor = new(currentUser, tenantService, metadataCache, serviceProvider, Microsoft.Extensions.Logging.Abstractions.NullLogger<AditiKraft.Krafter.Backend.Infrastructure.Persistence.Interceptors.AuditSaveChangesInterceptor>.Instance);

        DbContextOptions<TenantDbContext> options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(dbName)
            .AddInterceptors(interceptor)
            .Options;

        await using TenantDbContext db = new(options);
        db.Tenants.Add(new AditiKraft.Krafter.Backend.Features.Tenants.Common.Tenant
        {
            Id = "tenant-1",
            Identifier = "tenant-1",
            Name = "Tenant One",
            AdminEmail = "admin@example.com",
            IsActive = true,
            ValidUpto = DateTime.UtcNow.AddDays(1),
            CreatedOn = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var tenant = await db.Tenants.SingleAsync(t => t.Id == "tenant-1");
        tenant.Name = "Tenant Updated";
        await db.SaveChangesAsync();

        AuditOutbox? outbox = await db.AuditOutbox.OrderByDescending(x => x.CreatedOn).FirstOrDefaultAsync();
        Assert.NotNull(outbox);
        Assert.Contains("Tenant Updated", outbox.Payload);
        Assert.Contains("Tenant", outbox.Payload);
    }
}
