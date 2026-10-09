using AditiKraft.Krafter.Backend.Common.Auth;
using AditiKraft.Krafter.Backend.Common.Tenants;
using AditiKraft.Krafter.Backend.Features.Roles.Common;
using AditiKraft.Krafter.Backend.Features.Tenants.Common;
using AditiKraft.Krafter.Backend.Features.Users.Common;
using AditiKraft.Krafter.Backend.Infrastructure.Jobs;
using AditiKraft.Krafter.Backend.Infrastructure.Localization;
using AditiKraft.Krafter.Backend.Infrastructure.Notifications;
using AditiKraft.Krafter.Backend.Infrastructure.Persistence;
using AditiKraft.Krafter.Backend.Web.Configuration;
using AditiKraft.Krafter.Contracts.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AditiKraft.Krafter.Tests.Users;

internal sealed class IdentityTestContext : IAsyncDisposable
{
    private readonly ServiceProvider _provider;
    private readonly AsyncServiceScope _scope;

    public IdentityTestContext()
    {
        var services = new ServiceCollection();
        string databaseName = Guid.CreateVersion7().ToString();
        services.AddLogging();
        services.AddSingleton<ICurrentUser, CurrentUser>();
        services.AddSingleton<CurrentTenantService>();
        services.AddSingleton<ITenantGetterService>(p => p.GetRequiredService<CurrentTenantService>());
        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
        services.AddDbContext<TenantDbContext>(options => options.UseInMemoryDatabase($"{databaseName}-tenants"));
        services.AddDbContext<LocalizationDbContext>(options => options.UseInMemoryDatabase($"{databaseName}-localization"));
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddSingleton<RecordingJobService>();
        services.AddSingleton<IJobService>(p => p.GetRequiredService<RecordingJobService>());
        services.AddSingleton<FakeEmailService>();
        services.AddSingleton<IEmailService>(p => p.GetRequiredService<FakeEmailService>());
        services.Configure<SecuritySettings>(settings =>
        {
            settings.PasswordReusePreventionWindowInDays = 90;
            settings.PasswordExpiryInDays = 90;
            settings.PasswordExpiryNotificationCooldownInDays = 1;
        });
        services.AddScoped<IUserMutationService, UserMutationService>();
        services.AddScoped<ITenantSettingService, TenantSettingService>();
        services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();
        _provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
        _scope = _provider.CreateAsyncScope();
        SetTenant("tenant-a");
    }

    public ApplicationDbContext Db => _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    public TenantDbContext TenantDb => _scope.ServiceProvider.GetRequiredService<TenantDbContext>();
    public UserManager<ApplicationUser> Users => _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    public RoleManager<ApplicationRole> Roles => _scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    public IUserMutationService Mutations => _scope.ServiceProvider.GetRequiredService<IUserMutationService>();
    public IPasswordPolicyService PasswordPolicies => _scope.ServiceProvider.GetRequiredService<IPasswordPolicyService>();
    public CurrentTenantService Tenant => _provider.GetRequiredService<CurrentTenantService>();
    public RecordingJobService Jobs => _provider.GetRequiredService<RecordingJobService>();
    public FakeEmailService EmailService => _provider.GetRequiredService<FakeEmailService>();

    public void SetTenant(string id)
        => Tenant.SetTenant(new CurrentTenantDetails { Id = id, Name = id, TenantLink = $"https://{id}.example.com" });

    public async Task<ApplicationRole> AddRoleAsync(string name)
    {
        var role = new ApplicationRole(name) { Id = Guid.CreateVersion7().ToString() };
        IdentityResult result = await Roles.CreateAsync(role);
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(error => error.Description)));
        return role;
    }

    public async Task<ApplicationUser> AddUserAsync(string email = "user@example.com")
    {
        var user = new ApplicationUser
        {
            Id = Guid.CreateVersion7().ToString(),
            FirstName = "First",
            LastName = "Last",
            PhoneNumber = "123456",
            UserName = email,
            Email = email,
            IsActive = true
        };
        IdentityResult result = await Users.CreateAsync(user, "TestPassword1!");
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(error => error.Description)));
        return user;
    }

    public async ValueTask DisposeAsync()
    {
        await _scope.DisposeAsync();
        await _provider.DisposeAsync();
    }

    internal sealed class RecordingJobService : IJobService
    {
        public List<string> QueuedEmailLogIds { get; } = [];

        public Task EnqueueAsync<T>(T requestInput, string methodName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.CompletedTask;
        }

        public Task EnqueueByLogIdAsync(string emailLogId, string methodName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            QueuedEmailLogIds.Add(emailLogId);
            return Task.CompletedTask;
        }
    }
}
