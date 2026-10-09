# Radzen UI Localization Pattern (Inline Extension Method Approach)

## Overview

The UI layer uses **Radzen.ILocalizer** with extension methods to provide inline localization for Radzen components. **No code-behind properties; all string assets stay in the Razor markup using inline `@L.GetOrDefault()` calls.** The pattern achieves cache-based synchronous access to preloaded strings, enabling non-blocking component rendering while supporting language switching at runtime.

## Architecture

### Localizer (Concrete Implementation)
- **File:** `src/UI/AditiKraft.Krafter.UI.Web.Client/Features/Localization/Localizer.cs`
- **Role:** Implements `Radzen.ILocalizer` with a ConcurrentDictionary cache
- **Key Methods:**
  - `Get(string key, CultureInfo culture)` — Returns cached value or null
  - `GetOrDefault(string key, string fallbackValue)` — Returns cached value or fallback
  - `Format(string key, string fallbackValue, params object[] args)` — Applies string formatting
  - `PreloadAsync(string[] keys, CancellationToken ct)` — Asynchronously batch-load keys into cache
  - `InvalidateCache()` — Clear cache (used on language change)

### LocalizerExtensions
- **File:** `src/UI/AditiKraft.Krafter.UI.Web.Client/Features/Localization/LocalizerExtensions.cs`
- **Role:** Adds `GetOrDefault` and `Format` extension methods to `Radzen.ILocalizer`
- **Purpose:** Enable inline localization in Razor markup without casting or wrapper properties
- **Key Methods:**
  - `GetOrDefault(this Radzen.ILocalizer localizer, string key, string fallbackValue)` — Synchronous cache access with fallback
  - `Format(this Radzen.ILocalizer localizer, string key, string fallbackValue, params object[] args)` — String formatting support

### ILocalizationService (Backend Integration)
- **File:** `src/UI/AditiKraft.Krafter.UI.Web.Client/Features/Localization/LocalizationService.cs`
- **Role:** Async wrapper that fetches localized strings from the backend
- **Usage:** Called by `PreloadAsync` to resolve missing keys during component initialization

### DI Registration
- **File:** `src/UI/AditiKraft.Krafter.UI.Web.Client/RegisterUIServices.cs`
- Registers Localizer (concrete) as `Radzen.ILocalizer` in the DI container
- Enables component injection via `@inject Radzen.ILocalizer L`

## Usage Pattern

### 1. Inject Radzen.ILocalizer in Code-Behind
```csharp
[Inject]
private Radzen.ILocalizer L { get; set; } = null!;
```

### 2. Preload Keys in Code-Behind (Initialization Only)
```csharp
private async Task PreloadLocalizationKeysAsync()
{
	var localizer = L as Features.Localization.Localizer;
	if (localizer == null) return;

	var keysToPreload = new[]
	{
		"Button_Save",
		"Button_Delete",
		"Language_Name",
		"Spreadsheet_Find",
		"UI_Common_ConfigTitle"
	};

	await localizer.PreloadAsync(keysToPreload);
}

// Call during initialization
protected override async Task OnInitializedAsync()
{
	await PreloadLocalizationKeysAsync();
	// ... other initialization
}

// Call on language change
private async Task ChangeLanguageAsync(string newLanguageCode)
{
	// Update backend and refresh token...

	// Invalidate and reload cache
	var localizer = L as Features.Localization.Localizer;
	localizer?.InvalidateCache();
	await PreloadLocalizationKeysAsync();

	await InvokeAsync(StateHasChanged);
}
```

### 3. Use Inline Localize in Markup (Direct, No Properties!)
```razor
<!-- Button with inline localization -->
<RadzenButton Text="@L.Localize("Button_Save", "Save")" />

<!-- Label with inline localization -->
<RadzenLabel Text="@L.Localize("Language_Name", "Name")" />

<!-- TextBox placeholder with inline localization -->
<RadzenTextBox Placeholder="@L.Localize("Spreadsheet_Find", "Find items...")" />

<!-- Text element with inline localization -->
<RadzenText>@L.Localize("UI_Common_ConfigTitle", "Demos Configuration")</RadzenText>

<!-- Formatted string with parameters (Localize detects args and formats automatically) -->
<RadzenText>@L.Localize("User_WelcomeMessage", "Welcome", userName)</RadzenText>

<!-- Multiple parameters (auto-detected and formatted) -->
<RadzenText>@L.Localize("Report_Summary", "{0} items in {1}", itemCount, categoryName)</RadzenText>
```

## Key Naming Conventions

- **Radzen Component Keys:** Exact keys from `RadzenStrings.resx` (no prefix)
  - Examples: `Spreadsheet_Find`, `Spreadsheet_OK`, `Calendar_DateFormat`, `SchedulerWeek_Tuesday`

- **Button Actions:** `Button_[Action]`
  - Examples: `Button_Save`, `Button_Delete`, `Button_Cancel`, `Button_Submit`, `Button_Reset`

- **Entity Attributes:** `[Entity]_[Attribute]`
  - Examples: `Language_Name`, `Language_Code`, `User_Email`, `User_FirstName`

- **Navigation Items:** `Nav_[Menu]`
  - Examples: `Nav_Dashboard`, `Nav_Users`, `Nav_Settings`, `Nav_Home`

- **UI Appearance/Config:** `UI_[Category]_[Item]`
  - Examples: `UI_Appearance_ThemePreference`, `UI_Common_ConfigTitle`, `UI_Appearance_WCAGLabel`

## Extension Methods (LocalizerExtensions)

### Localize (Unified Method)
```csharp
public static string Localize(this Radzen.ILocalizer localizer, string key, string fallbackValue, params object?[]? args)
```
**Purpose:** Unified localization method that handles both simple retrieval and formatting in a single call  
**Behavior:**
1. Retrieves localized string from cache (or fallback if key not found)
2. **If args are provided and non-empty:** applies `string.Format(value, args)` automatically
3. **If args are null or empty:** returns value as-is without formatting
4. If formatting fails, returns unformatted value gracefully
5. **Always synchronous** — returns immediately from cache

**Advantages Over GetOrDefault + Format:**
- Single method signature instead of two
- Intelligent arg detection: no need to decide which method to call
- Null-safe: handles null or empty args gracefully
- Exception-safe: catches formatting errors and falls back to unformatted value
- Cleaner markup: one call for both localization and formatting

**Usage Examples:**

Simple localization (no args):
```razor
<!-- Returns "Save" from cache -->
Text="@L.Localize("Button_Save", "Save")"
```

Localization with formatting (with args):
```razor
<!-- With 1 arg: formats to "User: John Doe" -->
@L.Localize("User_Welcome", "Welcome: {0}", userName)

<!-- With multiple args: formats to "2 users in department Sales" -->
@L.Localize("Dept_UserCount", "{0} users in department {1}", userCount, deptName)
```

Mixed usage (let `Localize` decide):
```razor
<!-- If args supplied: format; else return as-is -->
<RadzenText>@L.Localize("Message_Status", "Status: Unknown", statusCode)</RadzenText>
<RadzenText>@L.Localize("Label_Simple", "Simple Label")</RadzenText>
```

Error-safe formatting:
```csharp
// If formatting fails (e.g., wrong arg count), Localize returns unformatted string
// instead of throwing; graceful degradation
string result = L.Localize("Bad_Format", "Value: {0} {1}", singleArg); 
// Returns "Value: {0} {1}" (unformatted fallback)
```

## Cache Lifecycle

1. **Initialization:** Component calls `PreloadAsync()` during `OnInitializedAsync()` → keys fetched from backend and stored in cache
2. **Rendering:** Markup accesses cache via `L.GetOrDefault(key, fallback)` → **synchronous, no await**
3. **Language Change:** Application calls `InvalidateCache()` on localizer, then calls `PreloadAsync()` again → cache emptied, reloaded
4. **Component Re-Render:** `StateHasChanged()` invoked → markup re-evaluates `L.GetOrDefault()` calls with fresh cache values

## Benefits of Inline Pattern

1. **No Property Overhead:** Eliminates repetitive computed properties (e.g., `ThemeAuto`, `ThemeLight`, `ThemeDark`)
2. **Direct Usage:** Keys and fallbacks visible at point of use in markup
3. **Easy Refactoring:** Change key or fallback string in one place only, no sync between code-behind and markup
4. **Clean Code-Behind:** Only preload logic, no property declarations
5. **Type-Safe Fallback:** Fallback text paired with key at call site; less error-prone
6. **Less Boilerplate:** No intermediary string properties; fewer lines of code

## Example: MainLayout Component

**Code-Behind (MainLayout.razor.cs):**
```csharp
[Inject]
private Radzen.ILocalizer L { get; set; } = null!;

protected override async Task OnInitializedAsync()
{
	await PreloadLocalizationKeysAsync();
	// ... other initialization
}

private async Task PreloadLocalizationKeysAsync()
{
	var localizer = L as Features.Localization.Localizer;
	if (localizer == null) return;

	var keysToPreload = new[]
	{
		"Spreadsheet_Find",
		"UI_Common_ConfigTitle",
		"UI_Appearance_WCAGLabel",
		"UI_Appearance_WCAGInfo",
		"UI_Appearance_ThemePreference",
		// ... etc.
	};

	await localizer.PreloadAsync(keysToPreload);
}

private async Task ChangeLanguageAsync(object value)
{
	string code = value?.ToString()?.Trim().ToLowerInvariant() ?? "en";
	// ... update backend, refresh token

	var localizer = L as Features.Localization.Localizer;
	localizer?.InvalidateCache();
	await PreloadLocalizationKeysAsync();
	await InvokeAsync(StateHasChanged);
}
```

**Razor Markup (MainLayout.razor):**
```razor
@inherits LayoutComponentBase

<!-- SEARCH PLACEHOLDER -->
<RadzenTextBox 
	id="find" 
	Placeholder="@L.Localize("Spreadsheet_Find", "Find items ...")" 
	type="search" 
	@oninput="@FilterPanelMenu" 
	style="width: 100%" />

<!-- CONFIG TITLE -->
<RadzenText TextStyle="TextStyle.H6" TagName="TagName.H2">
	@L.Localize("UI_Common_ConfigTitle", "Demos Configuration")
</RadzenText>

<!-- WCAG LABEL + INFO -->
<RadzenLabel 
	Text="@L.Localize("UI_Appearance_WCAGLabel", "Use WCAG compliant colors")" 
	Title="@L.Localize("UI_Appearance_WCAGInfo", "Use WCAG 2.2 AA compliant color swatches")" />

<!-- THEME PREFERENCE LABEL -->
<RadzenText TextStyle="TextStyle.Subtitle1" TagName="TagName.P">
	@L.Localize("UI_Appearance_ThemePreference", "Theme Preference")
</RadzenText>

<!-- THEME RADIO BUTTONS -->
<RadzenRadioButtonListItem Text="@L.Localize("UI_Appearance_ThemeAuto", "Auto (System)")" Value="ThemeManager.ThemePreference.Auto"/>
<RadzenRadioButtonListItem Text="@L.Localize("UI_Appearance_ThemeLight", "Light")" Value="ThemeManager.ThemePreference.Light"/>
<RadzenRadioButtonListItem Text="@L.Localize("UI_Appearance_ThemeDark", "Dark")" Value="ThemeManager.ThemePreference.Dark"/>

<!-- TOOLTIP TEXT -->
<RadzenText TextStyle="TextStyle.Body2">
	@L.Localize("UI_Appearance_WCAGTooltipDescription", "Enable to preview themes with WCAG 2.2 AA compliant color swatches.")
</RadzenText>
```

## Best Practices

1. **Preload Early:** Load all required keys once during component initialization to avoid "flash of fallback"
2. **Use Seeded Keys:** Rely on backend-seeded keys from `LocalizationResourceSeedData`; backend auto-creates unknown keys at runtime
3. **Provide Fallbacks:** Always pass fallback text to `GetOrDefault()` for graceful degradation if key missing
4. **Group Preloads:** Batch all keys needed by a component in one `PreloadAsync()` call 
5. **Invalidate on Language Change:** Always call `InvalidateCache()` before reloading to ensure fresh data
6. **No Fire-and-Forget:** Always `await` `PreloadAsync()` and `StateHasChanged()` in language change flows
7. **Avoid Repeated Preloads:** Preload once per language change, not per component render cycle
8. **Consistent Naming:** Follow the standardized key patterns; document custom patterns in Agents.md

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| Fallback text always shown | Keys not preloaded or not in backend | Check backend seeding; verify key spelling; ensure `PreloadAsync()` awaited |
| Wrong translation after language change | Cache not invalidated | Ensure `InvalidateCache()` called before `PreloadAsync()` on language change |
| Null reference exception | L is null in code-behind | Ensure `[Inject]` property exists and DI registration in `RegisterUIServices` |
| `Localize` not recognized in markup | Type mismatch or missing extension method | Verify `LocalizerExtensions.cs` compiles; ensure using directive for namespace or extension method is visible |
| Flickering or placeholder text on init | Preload not awaited | Use `await PreloadAsync(...)` in async contexts; ensure completion before `StateHasChanged()` or render completes |
| Language change not reflected | Cache not cleared | Ensure `InvalidateCache()` called and `StateHasChanged()` invoked after preload completes |
| Formatted string shows wrong result | Arg count mismatch or wrong format string | Verify placeholder count in key matches args passed; `Localize` returns unformatted as fallback if format fails
