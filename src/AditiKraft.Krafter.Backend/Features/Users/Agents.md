# Users Feature AI Instructions

> **SCOPE**: User CRUD, roles, permissions, and password flows.
> **PARENT**: See also: ../../Agents.md

## 1. Core Principles
- Use `UserManager<ApplicationUser>` and `RoleManager<ApplicationRole>` for all user operations.
- On create, ensure the Basic role is assigned (`RoleConstants.Basic`).
- Send account and password emails via `IJobService.EnqueueAsync(...)`.
- Soft-delete users and related roles; never hard-delete.

## 2. Decision Tree
- Creating a user? Follow `CreateUser` pattern.
- Updating a user? Follow `UpdateUser` pattern.
- Deleting a user? Follow `DeleteUser` soft-delete pattern.
- Password changes or reset flows? Use `ChangePassword`, `ForgotPassword`, `ResetPassword` patterns.
- Need user roles or permissions? Use `GetUserRoles` and `GetUserPermissions` patterns.

## 3. Code Templates

### Shared User Changes
`CreateUser` and `UpdateUser` delegate to `IUserMutationService`. `UserMutationService` owns user creation, profile updates, role synchronization, and account email jobs. `IUserService` is for permission queries.

For tenant admin email changes, call `UpdateEmailAsync(id, email, cancellationToken)`. This preserves the user's profile and roles. Check the returned response before saving the tenant email.

Role synchronization checks that requested roles belong to the current tenant. New users receive the Basic role. Existing profile-only updates preserve roles when the list is omitted or empty.

## 4. Checklist
1. Use `UserManager`/`RoleManager` for user and role operations.
2. Ensure Basic role is added when creating a new user.
3. If `UpdateTenantEmail` is true, update tenant admin email in `TenantDbContext`.
4. Enqueue email jobs for account creation and password flows.
5. Soft-delete user roles when removing a user.

## 5. Common Mistakes
- Skipping Basic role assignment for new users.
- Updating user email without handling tenant admin email when requested.
- Hard-deleting users or roles instead of soft delete.

## 6. Evolution Triggers
- Role assignment rules change.
- Password or email notification flows change.
- User role/permission query patterns change.
- New entities/features are added that require authorization/permission coverage.

## 7. Permission Expansion Rule
When adding new entities or feature endpoints, always update permission coverage in the same change:
1. Add new resource constants to `PermissionResource`.
2. Ensure every entity has a `PermissionResource` entry.
3. Add `PermissionAction.Manage` for every `PermissionResource` in `PermissionCatalog`.
4. Add matching `PermissionCatalog` entries for other actions (View/Create/Update/Delete/Search as applicable).
5. Keep `PermissionCatalog` entries grouped by entity using `#region` blocks or `//` section comments.
6. Apply `MustHavePermission(...)` on every gated backend route with the relevant permission action/resource.
7. Ensure seed/sync path grants missing permissions (admin/root via `DataSeedService` + `PermissionCatalog`).

## 8. Build Error Recurrence Rule
- When a build error is encountered, fix the root cause instead of applying a symptom-only workaround.
- Update the relevant `Agents.md` file(s) with the missing rule/pattern that would have prevented the error.
- Proceed only after the fix and instruction update are both applied to avoid repeating the same error.

## References (real code)
- `src/AditiKraft.Krafter.Backend/Features/Users/Common/UserMutationService.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/CreateUser.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/UpdateUser.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/DeleteUser.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/ChangePassword.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/ForgotPassword.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/ResetPassword.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/GetUserRoles.cs`
- `src/AditiKraft.Krafter.Backend/Features/Users/GetUserPermissions.cs`

---
Last Updated: 2026-09-09
Verified Against: src/AditiKraft.Krafter.Backend/Features/Users/Common/UserMutationService.cs, src/AditiKraft.Krafter.Backend/Features/Users/CreateUser.cs, src/AditiKraft.Krafter.Backend/Features/Users/UpdateUser.cs, src/AditiKraft.Krafter.Backend/Features/Users/DeleteUser.cs, src/AditiKraft.Krafter.Backend/Features/Users/ChangePassword.cs, src/AditiKraft.Krafter.Backend/Features/Users/ForgotPassword.cs, src/AditiKraft.Krafter.Backend/Features/Users/ResetPassword.cs, src/AditiKraft.Krafter.Backend/Features/Users/GetUserRoles.cs, src/AditiKraft.Krafter.Backend/Features/Users/GetUserPermissions.cs, src/AditiKraft.Krafter.Backend/Features/Users/Common/UserService.cs, src/AditiKraft.Krafter.Contracts/Contracts/Roles/RoleConstants.cs
---
