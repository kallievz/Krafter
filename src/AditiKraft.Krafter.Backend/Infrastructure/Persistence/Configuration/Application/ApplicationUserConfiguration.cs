using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AditiKraft.Krafter.Backend.Infrastructure.Persistence.Configuration.Application;

internal sealed class ApplicationUserConfiguration(ITenantGetterService tenantGetterService)
    : IEntityTypeConfiguration<ApplicationUser>
{
    private readonly ITenantGetterService _tenantGetterService = tenantGetterService;

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AppUser", SchemaConstants.Identity);
        builder.Property(c => c.Id).HasMaxLength(36);
        builder.Property(c => c.CreatedById).HasMaxLength(36);
        builder.HasQueryFilter(c => c.IsDeleted == false && c.TenantId == _tenantGetterService.Tenant.Id);
        builder.HasMany(e => e.UserRoles)
            .WithOne(e => e.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.PasswordHistories)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(e => e.NormalizedEmail).IsUnique(false);
        builder.HasIndex(e => new { e.NormalizedEmail, e.TenantId }).IsUnique();
        builder.HasIndex(e => e.NormalizedUserName).IsUnique(false);
        builder.HasIndex(e => new { e.NormalizedUserName, e.TenantId }).IsUnique();
    }
}
