using System.Collections.ObjectModel;

namespace AditiKraft.Krafter.Contracts.Common.Auth.Permissions;

public static class PermissionCatalog
{
    private static readonly PermissionDefinition[] _allPermissions =
    [
#region Users
        new("Manage Users", PermissionAction.Manage, PermissionResource.Users),
        new("View Users", PermissionAction.View, PermissionResource.Users),
        new("Search Users", PermissionAction.Search, PermissionResource.Users),
        new("Create Users", PermissionAction.Create, PermissionResource.Users),
        new("Update Users", PermissionAction.Update, PermissionResource.Users),
        new("Delete Users", PermissionAction.Delete, PermissionResource.Users),
#endregion

#region UserRoles
        new("Manage UserRoles", PermissionAction.Manage, PermissionResource.UserRoles),
        new("View UserRoles", PermissionAction.View, PermissionResource.UserRoles),
        new("Update UserRoles", PermissionAction.Update, PermissionResource.UserRoles),
#endregion

#region Roles
        new("Manage Roles", PermissionAction.Manage, PermissionResource.Roles),
        new("View Roles", PermissionAction.View, PermissionResource.Roles),
        new("Create Roles", PermissionAction.Create, PermissionResource.Roles),
        new("Update Roles", PermissionAction.Update, PermissionResource.Roles),
        new("Delete Roles", PermissionAction.Delete, PermissionResource.Roles),
#endregion

#region RoleClaims
        new("Manage RoleClaims", PermissionAction.Manage, PermissionResource.RoleClaims),
        new("View RoleClaims", PermissionAction.View, PermissionResource.RoleClaims),
        new("Update RoleClaims", PermissionAction.Update, PermissionResource.RoleClaims),
#endregion

#region UserClaims
        new("Manage UserClaims", PermissionAction.Manage, PermissionResource.UserClaims),
#endregion

#region Notifications
        new("Manage Notifications", PermissionAction.Manage, PermissionResource.Notifications, true),
        new("View Notifications", PermissionAction.View, PermissionResource.Notifications, true),
#endregion

#region Integrations
        new("Manage Integrations", PermissionAction.Manage, PermissionResource.Integrations, IsRoot: true),
        new("View Integrations", PermissionAction.View, PermissionResource.Integrations),
        new("Update Integrations", PermissionAction.Update, PermissionResource.Integrations),
#endregion

#region LocalizationLanguages
        new("Manage LocalizationLanguages", PermissionAction.Manage, PermissionResource.LocalizationLanguages, IsRoot: true),
        new("View LocalizationLanguages", PermissionAction.View, PermissionResource.LocalizationLanguages),
        new("Create LocalizationLanguages", PermissionAction.Create, PermissionResource.LocalizationLanguages),
        new("Update LocalizationLanguages", PermissionAction.Update, PermissionResource.LocalizationLanguages),
        new("Delete LocalizationLanguages", PermissionAction.Delete, PermissionResource.LocalizationLanguages),
#endregion

#region LocalizationResources
        new("Manage LocalizationResources", PermissionAction.Manage, PermissionResource.LocalizationResources, IsRoot: true),
        new("View LocalizationResources", PermissionAction.View, PermissionResource.LocalizationResources),
        new("Create LocalizationResources", PermissionAction.Create, PermissionResource.LocalizationResources),
        new("Update LocalizationResources", PermissionAction.Update, PermissionResource.LocalizationResources),
        new("Delete LocalizationResources", PermissionAction.Delete, PermissionResource.LocalizationResources),
#endregion

#region LocalizationResourceTranslations
        new("Manage LocalizationResourceTranslations", PermissionAction.Manage, PermissionResource.LocalizationResourceTranslations, IsRoot: true),
        new("View LocalizationResourceTranslations", PermissionAction.View, PermissionResource.LocalizationResourceTranslations),
        new("Create LocalizationResourceTranslations", PermissionAction.Create, PermissionResource.LocalizationResourceTranslations),
        new("Update LocalizationResourceTranslations", PermissionAction.Update, PermissionResource.LocalizationResourceTranslations),
        new("Delete LocalizationResourceTranslations", PermissionAction.Delete, PermissionResource.LocalizationResourceTranslations),
#endregion

#region Tenants
        new("Manage Tenants", PermissionAction.Manage, PermissionResource.Tenants, IsRoot: true),
        new("View Tenants", PermissionAction.View, PermissionResource.Tenants, IsRoot: true),
        new("Create Tenants", PermissionAction.Create, PermissionResource.Tenants, IsRoot: true),
        new("Update Tenants", PermissionAction.Update, PermissionResource.Tenants, IsRoot: true),
        new("Delete Tenants", PermissionAction.Delete, PermissionResource.Tenants, IsRoot: true)
#endregion
    ];

    public static IReadOnlyList<PermissionDefinition> All { get; } =
        new ReadOnlyCollection<PermissionDefinition>(_allPermissions);

    public static IReadOnlyList<PermissionDefinition> Root { get; } =
        new ReadOnlyCollection<PermissionDefinition>(_allPermissions.Where(p => p.IsRoot).ToArray());

    public static IReadOnlyList<PermissionDefinition> Admin { get; } =
        new ReadOnlyCollection<PermissionDefinition>(_allPermissions.Where(p => !p.IsRoot).ToArray());

    public static IReadOnlyList<PermissionDefinition> Basic { get; } =
        new ReadOnlyCollection<PermissionDefinition>(_allPermissions.Where(p => p.IsBasic).ToArray());
}
