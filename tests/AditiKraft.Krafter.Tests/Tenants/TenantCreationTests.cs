using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Entities;
using AditiKraft.Krafter.Backend.Features.Tenants;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Entities;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Features.Users.Entities;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Models;
using AditiKraft.Krafter.Contracts.Contracts.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AditiKraft.Krafter.Tests.Tenants;

public sealed class TenantCreationTests
{
    private static readonly AppUrls _urls = new()
    {
        RootUiUrl = "https://krafter.getkrafter.dev",
        TenantBaseDomain = "getkrafter.dev",
        ApiBaseUrl = "https://api.getkrafter.dev",
        ReservedTenantIdentifiers = ["status"]
    };

    [Fact]
    public async Task CreatingTenantPersistsUtcCreationTimeAndUpdatingPreservesItAsync()
    {
        var services = new ServiceCollection();
        string databaseName = Guid.CreateVersion7().ToString();
        services.AddLogging();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddTenantServices();
        services.AddDbContext<TenantDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase($"{databaseName}-identity"));
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<DataSeedService>();
        services.AddSingleton<IJobService, NoExternalJobs>();
        services.AddSingleton<IEmailService, NoExternalEmailService>();
        await using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        await using AsyncServiceScope scope = provider.CreateAsyncScope();
        IServiceProvider scoped = scope.ServiceProvider;
        scoped.GetRequiredService<ITenantSetterService>().SetTenant(new CurrentTenantDetails
        {
            Id = "root",
            TenantLink = "https://root.example.com"
        });
        var currentUser = (CurrentUser)scoped.GetRequiredService<ICurrentUser>();
        currentUser.SetCurrentUserId("creator");
        TenantDbContext db = scoped.GetRequiredService<TenantDbContext>();
        var request = new CreateOrUpdateTenantRequest
        {
            Name = "Example tenant",
            Identifier = "example",
            AdminEmail = "admin@example.com",
            IsActive = true,
            ValidUpto = new DateTime(2030, 6, 15, 18, 15, 0, DateTimeKind.Utc)
        };
        var create = new TenantsCrud.Handler(
            db,
            provider,
            currentUser,
            _urls,
            Options.Create(new SecuritySettings()),
            new SMTPEmailSettings(),
            new SendGridEmailSettings());

        DateTime before = DateTime.UtcNow;
        Response created = await create.CreateAsync(request, CancellationToken.None);
        DateTime after = DateTime.UtcNow;

        Assert.False(created.IsError, created.Message);
        db.ChangeTracker.Clear();
        Tenant saved = await db.Tenants.AsNoTracking().SingleAsync();
        Assert.InRange(saved.CreatedOn, before, after);
        Assert.Equal(DateTimeKind.Utc, saved.CreatedOn.Kind);
        Assert.Equal("creator", saved.CreatedById);
        Assert.Equal(request.ValidUpto, saved.ValidUpto);
        Assert.Equal(DateTimeKind.Utc, saved.ValidUpto.Kind);

        request.Name = "Updated tenant";
        var update = new TenantsCrud.Handler(db, provider, currentUser, _urls, Options.Create(new SecuritySettings()), new SMTPEmailSettings(), new SendGridEmailSettings());
        Response updated = await update.PatchAsync(saved.Id!, request, CancellationToken.None);

        Assert.False(updated.IsError, updated.Message);
        db.ChangeTracker.Clear();
        Tenant reloaded = await db.Tenants.AsNoTracking().SingleAsync();
        Assert.Equal("Updated tenant", reloaded.Name);
        Assert.Equal(saved.CreatedOn, reloaded.CreatedOn);
        Assert.Equal(saved.ValidUpto, reloaded.ValidUpto);
    }

    [Theory]
    [InlineData("api")]
    [InlineData("www")]
    [InlineData("mail")]
    [InlineData("krafter")]
    [InlineData("status")]
    [InlineData("root")]
    [InlineData("Blue")]
    [InlineData("blue.red")]
    [InlineData("blue_red")]
    [InlineData("-blue")]
    [InlineData("blue-")]
    [InlineData(" blue ")]
    [InlineData("blue\n")]
    [InlineData("blue/../../")]
    [InlineData("abcdefghijk")]
    [InlineData("नेपाल")]
    [InlineData("")]
    public async Task CreateRejectsInvalidOrReservedIdentifiersEvenWithForgedRootIdAsync(string identifier)
    {
        await using TenantDbContext db = CreateDb();
        CreateOrUpdateTenantRequest request = ValidRequest(identifier);
        request.Id = "root";
        var handler = new TenantsCrud.Handler(
            db,
            null!,
            null!,
            _urls,
            Options.Create(new SecuritySettings()),
            new SMTPEmailSettings(),
            new SendGridEmailSettings());

        Response response = await handler.CreateAsync(request, CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Empty(await db.Tenants.ToListAsync());
    }

    [Theory]
    [InlineData("blue")]
    [InlineData("new-tenant")]
    [InlineData("a")]
    [InlineData("1")]
    [InlineData("a-1")]
    public void ValidatorAcceptsValidIdentifiers(string identifier)
    {
        var validator = new CreateOrUpdateTenantRequestValidator(_urls);
        Assert.True(validator.Validate(ValidRequest(identifier)).IsValid);
    }

    [Fact]
    public async Task CreateRejectsIdentifierMatchingExistingTenantIgnoringCaseAsync()
    {
        await using TenantDbContext db = CreateDb();
        db.Tenants.Add(ExistingTenant("existing", "Blue"));
        await db.SaveChangesAsync();
        var handler = new TenantsCrud.Handler(
            db,
            null!,
            null!,
            _urls,
            Options.Create(new SecuritySettings()),
            new SMTPEmailSettings(),
            new SendGridEmailSettings());

        Response response = await handler.CreateAsync(ValidRequest("blue"), CancellationToken.None);

        Assert.Equal(409, response.StatusCode);
        Assert.Single(await db.Tenants.ToListAsync());
    }

    [Fact]
    public async Task UpdateRejectsIdentifierMatchingAnotherTenantIgnoringCaseAsync()
    {
        await using TenantDbContext db = CreateDb();
        db.Tenants.AddRange(ExistingTenant("existing", "Blue"), ExistingTenant("edited", "green"));
        await db.SaveChangesAsync();
        var handler = new TenantsCrud.Handler(db, null!, null!, _urls, null!, null!, null!);

        Response response = await handler.PatchAsync("edited", ValidRequest("blue"), CancellationToken.None);

        Assert.Equal(409, response.StatusCode);
        Assert.Equal("green", (await db.Tenants.FindAsync("edited"))!.Identifier);
    }

    [Theory]
    [InlineData("root")]
    [InlineData("api")]
    [InlineData("krafter")]
    [InlineData("status")]
    [InlineData("Blue")]
    [InlineData("blue.red")]
    public async Task UpdateRejectsInvalidIdentifiersAndIgnoresForgedRequestIdAsync(string identifier)
    {
        await using TenantDbContext db = CreateDb();
        db.Tenants.Add(ExistingTenant("edited", "green"));
        await db.SaveChangesAsync();
        CreateOrUpdateTenantRequest request = ValidRequest(identifier);
        request.Id = "root";
        var handler = new TenantsCrud.Handler(db, null!, null!, _urls, null!, null!, null!);

        Response response = await handler.PatchAsync("edited", request, CancellationToken.None);

        Assert.Equal(400, response.StatusCode);
        Assert.Equal("green", (await db.Tenants.FindAsync("edited"))!.Identifier);
    }

    [Fact]
    public async Task UpdateAllowsRootNameChangeWithoutChangingReservedIdentifierAsync()
    {
        await using TenantDbContext db = CreateDb();
        Tenant root = ExistingTenant(SeedDataConstants.RootTenant.Id, DefaultTenantConstants.Identifier);
        db.Tenants.Add(root);
        await db.SaveChangesAsync();
        CreateOrUpdateTenantRequest request = ValidRequest(DefaultTenantConstants.Identifier);
        request.Name = "Updated root";
        request.ValidUpto = root.ValidUpto;
        var handler = new TenantsCrud.Handler(db, null!, null!, _urls, null!, null!, null!);

        Response response = await handler.PatchAsync(root.Id!, request, CancellationToken.None);

        Assert.False(response.IsError, response.Message);
        Assert.Equal("Updated root", root.Name);
        Assert.Equal(DefaultTenantConstants.Identifier, root.Identifier);
    }

    private static TenantDbContext CreateDb() => new(new DbContextOptionsBuilder<TenantDbContext>()
        .UseInMemoryDatabase(Guid.CreateVersion7().ToString()).Options);

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ConcurrentIdentifierConflictReturnsConflictAsync(bool updating)
    {
        var conflict = new IdentifierConflictInterceptor();
        DbContextOptions<TenantDbContext> options = new DbContextOptionsBuilder<TenantDbContext>()
            .UseInMemoryDatabase(Guid.CreateVersion7().ToString()).AddInterceptors(conflict).Options;
        await using var db = new TenantDbContext(options);
        if (updating)
        {
            db.Tenants.Add(ExistingTenant("edited", "green"));
            await db.SaveChangesAsync();
        }
        conflict.RejectSave = true;

        Response response = updating
            ? await new TenantsCrud.Handler(db, null!, null!, _urls, null!, null!, null!).PatchAsync("edited", ValidRequest("blue"), CancellationToken.None)
            : await new TenantsCrud.Handler(
                    db,
                    null!,
                    new CurrentUser(),
                    _urls,
                    Options.Create(new SecuritySettings()),
                    new SMTPEmailSettings(),
                    new SendGridEmailSettings())
                .CreateAsync(ValidRequest("blue"), CancellationToken.None);

        Assert.Equal(409, response.StatusCode);
        db.ChangeTracker.Clear();
        if (updating)
        {
            Assert.Equal("green", (await db.Tenants.SingleAsync()).Identifier);
        }
        else
        {
            Assert.Empty(await db.Tenants.ToListAsync());
        }
    }

    private sealed class IdentifierConflictInterceptor : SaveChangesInterceptor
    {
        public bool RejectSave { get; set; }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (RejectSave)
            {
                throw new DbUpdateException("A concurrent request used this identifier.",
                    new InvalidOperationException($"Duplicate identifier ({TenantDbContext.TenantIdentifierIndexName})."));
            }

            return ValueTask.FromResult(result);
        }
    }

    private static CreateOrUpdateTenantRequest ValidRequest(string identifier) => new()
    {
        Identifier = identifier,
        Name = "Example tenant",
        AdminEmail = "admin@example.com",
        IsActive = true,
        ValidUpto = DateTime.UtcNow.AddDays(30)
    };

    private static Tenant ExistingTenant(string id, string identifier) => new()
    {
        Id = id,
        Identifier = identifier,
        Name = "Existing tenant",
        AdminEmail = "admin@example.com",
        IsActive = true,
        ValidUpto = DateTime.UtcNow.AddDays(30)
    };

    private sealed class NoExternalJobs : IJobService
    {
        public Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken) =>
            Task.CompletedTask;

        public Task EnqueueByLogIdAsync(string emailLogId, string methodName, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }

    private sealed class NoExternalEmailService : IEmailService
    {
        public Task<string> QueueEmailAsync(SendEmailRequestInput request, CancellationToken cancellationToken) =>
            Task.FromResult(Guid.CreateVersion7().ToString());

        public Task ProcessEmailAsync(string emailLogId, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task SendEmailAsync(string email, string subject, string htmlMessage, CancellationToken cancellationToken) =>
            Task.CompletedTask;
    }
}

