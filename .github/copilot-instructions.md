# Copilot Instructions

## Project Guidelines
- Follow naming rules (async methods should use Async suffix), use explicit types instead of var, and fix formatting when making changes.
- Always check for existence before inserting seed values.
- Split explicit logger registration into a separate class and include assembly information in the logger setup.
- When adding new entities/features, also add corresponding PermissionResource constants, PermissionCatalog entries, endpoint MustHavePermission wiring, and ensure seed sync includes those permissions.

## UI Component Guidelines
- For Radzen grid components, do not expose per-grid theme toggles; theme must be configured globally and reusable grids should respect global theme settings.
