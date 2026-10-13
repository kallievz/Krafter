# Copilot Instructions

## Project Guidelines
- Follow naming rules (async methods should use Async suffix), use explicit types instead of var, and fix formatting when making changes.
- Always check for existence before inserting seed values.
- Split explicit logger registration into a separate class and include assembly information in the logger setup.
- When a build error occurs, fix the root cause, update relevant Agents.md guidance to capture the prevention rule, then proceed to avoid repeating the same error.
- Use the `MunsoftDebtor` spelling (not `MusoftDebtor`) for the debtor entity, mapper, log, and related import types in the Munsoft integration workspace.
- For integration CRUD and contracts in this repo, use one CRUD file per entity and include full non-partial attribute mappings in request/response/dto models; document this pattern in Agents.md guidance.

## UI Component Guidelines
- For Radzen grid components, do not expose per-grid theme toggles; theme must be configured globally and reusable grids should respect global theme settings.
- Use inline `L.Localize("key", "fallback", args)` calls directly in Razor markup via LocalizerExtensions without code-behind properties. Inject `Radzen.ILocalizer L` in code-behind, preload keys once during OnInitializedAsync via `(L as Localizer).PreloadAsync(keys)`, invalidate/reload on language change. This eliminates repetitive property declarations and keeps localization strings visible at point of use.

## Permission Model
- Every entity must have a PermissionResource entry.
- PermissionCatalog must include PermissionAction.Manage for every PermissionResource.
- PermissionCatalog entries should be grouped by entity using regions/comments for readability.
