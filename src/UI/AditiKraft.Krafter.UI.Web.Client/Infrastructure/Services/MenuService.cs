using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.Contracts.Common.Enums;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Infrastructure.Services;

public class MenuService
{
    private readonly Menu[] _allMenus = new[]
    {
        new Menu
        {
            Name = "Home",
            Path = "/",
            Icon = "home",
            Title = "Manage Courses",
            Description =
                "Manage courses, including creation, updates, and access control for course listings.",
            Tags = new[] { "manage", "courses", "processes", "process" }
        },
        new Menu
        {
            Name = "Account",
            Icon = "account_circle",
            Children = new[]
            {
                new Menu
                {
                    Name = "Users",
                    Path = ApiRoutes.Users,
                    Title = "User Management",
                    Description = "Manage user accounts, including creation, updates, and access control.",
                    Icon = "people",
                    Tags = new[] { "users", "accounts", "profiles", "authentication" },
                    Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Users)
                },
                new Menu
                {
                    Name = "Roles",
                    Path = ApiRoutes.Roles,
                    Title = "Role Management",
                    Description = "Manage user roles and associated permissions within the system.",
                    Icon = "admin_panel_settings",
                    Tags = new[] { "roles", "permissions", "access control", "user groups" },
                    Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Roles)
                },
                new Menu
                {
                    Name = "Jobs Dashboard",
                    Path = "jobs-dashboard",
                    Title = "TickerQ Jobs Dashboard",
                    Description = "View and monitor background jobs in the TickerQ dashboard.",
                    Icon = "monitor_heart",
                    Tags = new[] { "jobs", "tickerq", "dashboard", "background processing" },
                    Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Notifications)
                },
                new Menu
                {
                    Name = "Telemetry Dashboard",
                    Path = "telemetry-dashboard",
                    Title = "Grafana Metrics Dashboard",
                    Description = "View application telemetry in Grafana backed by prometheus-net.",
                    Icon = "query_stats",
                    Tags = new[] { "telemetry", "metrics", "grafana", "prometheus", "observability" },
                    Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Notifications)
                }
            }
        },
        new Menu
        {
            Name = "Tenants",
            Path = ApiRoutes.Tenants,
            Icon = "business",
            Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Tenants),
            Title = "Tenant Management",
            Description = "Manage multiple tenants within the system, including creation and updates.",
            Tags = new[] { "tenants", "multi-tenancy", "organizations", "clients" }
        },
        new Menu
        {
            Name = "Integration",
            Icon = "integration_instructions",
            Children = new[]
            {
                new Menu
                {
                    Name = "Munsoft",
                    Path = $"{ApiRoutes.Integrations}/munsoft",
                    Icon = "sync",
                    Title = "Munsoft Integration",
                    Description = "View Munsoft instances and trigger instance synchronization.",
                    Tags = new[] { "integration", "munsoft", "instances", "sync" },
                    Permission = PermissionDefinition.NameFor(PermissionAction.View, PermissionResource.Tenants)
                }
            }
        }
    };

    public IEnumerable<Menu> Menus =>
        TenantSettings.TenancyMode == TenancyMode.Single
            ? _allMenus.Where(menu => menu.Path != ApiRoutes.Tenants)
            : _allMenus;

    public IEnumerable<Menu> Filter(string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            return _allMenus;
        }

        bool Contains(string value) => value != null && value.Contains(term, StringComparison.OrdinalIgnoreCase);
        bool Filter(Menu menu) => Contains(menu.Name) || (menu.Tags != null && menu.Tags.Any(Contains));
        bool DeepFilter(Menu menu) => Filter(menu) || menu.Children?.Any(Filter) == true;

        return Menus.Where(category => category.Children?.Any(DeepFilter) == true || Filter(category))
            .Select(category => new Menu
            {
                Name = category.Name,
                Path = category.Path,
                Icon = category.Icon,
                Permission = category.Permission,
                Expanded = true,
                Children = category.Children?.Where(DeepFilter).Select(menu => new Menu
                {
                    Name = menu.Name,
                    Path = menu.Path,
                    Icon = menu.Icon,
                    Expanded = true,
                    Permission = menu.Permission,
                    Children = menu.Children
                }).ToArray()
            }).ToList();
    }

    public Menu? FindCurrent(Uri uri)
    {
        IEnumerable<Menu> Flatten(IEnumerable<Menu> e) =>
            e.SelectMany(c => c.Children != null ? Flatten(c.Children) : new[] { c });

        return Flatten(Menus)
            .FirstOrDefault(menu => menu.Path == uri.AbsolutePath || $"/{menu.Path}" == uri.AbsolutePath);
    }

    public string TitleFor(Menu menu)
    {
        if (menu != null && menu.Name != "Overview")
        {
            return menu.Title ?? $"Blazor {menu.Name} Component | Free UI Components by Radzen";
        }

        return "Free Blazor Components | 70+ UI controls by Radzen";
    }

    public string DescriptionFor(Menu menu) => menu?.Description ??
                                               "The Radzen Blazor component library provides more than 70 UI controls for building rich ASP.NET Core web applications.";
}
