# Copilot Instructions

## Project Guidelines
- Follow naming rules (async methods should use Async suffix), use explicit types instead of var, and fix formatting when making changes.
- Always check for existence before inserting seed values.
- Split explicit logger registration into a separate class and include assembly information in the logger setup.
- When a build error occurs, fix the root cause, update relevant Agents.md guidance to capture the prevention rule, then proceed to avoid repeating the same error.

## UI Component Guidelines
- For Radzen grid components, do not expose per-grid theme toggles; theme must be configured globally and reusable grids should respect global theme settings.
- Use inline `L.Localize("key", "fallback", args)` calls directly in Razor markup via LocalizerExtensions without code-behind properties. This method combines localization and formatting into a single call, intelligently handling args: if args are null/empty, it returns the value as-is; if args are provided, it applies string.Format automatically. It is null-safe and exception-safe (returns unformatted on format error). Inject `Radzen.ILocalizer L` in code-behind, preload keys once during OnInitializedAsync via `(L as Localizer).PreloadAsync(keys)`, invalidate/reload on language change. This eliminates repetitive property declarations and keeps localization strings visible at the point of use.

## Permission Model
- Every entity must have a PermissionResource entry.
- PermissionCatalog must include PermissionAction.Manage for every PermissionResource.
- PermissionCatalog entries should be grouped by entity using regions/comments for readability.
