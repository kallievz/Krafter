# Localization Resource Seeding Pattern

## Overview

This guide documents the standardized approach to seeding localization resources (keys and translations) for the entire application, including entity properties, Radzen component strings, button actions, and navigation menus.

The seeding happens once during database initialization via `LocalizationDbSeedService`, which calls `LocalizationResourceSeedData.GetResources()` and `GetTranslations()`.

## Resource Key Naming Conventions

All localization keys follow these patterns:

### 1. **Entity Property Keys**: `[Entity]_[Attribute]`

**Purpose**: Localize entity field names and display labels.

**Examples**:
- `Language_Code` → "Code"
- `Language_Name` → "Name"
- `Language_IsActive` → "Is Active"
- `Resource_Key` → "Key"
- `Resource_Description` → "Description"

**Generation**: Auto-discovered from entity types via reflection in seeding data:
```csharp
// From LocalizationResourceSeedData.GetResources()
foreach (Type entityType in GetEntityTypes())
{
	foreach (string propertyName in entityType.GetProperties(...))
	{
		string key = $"{entityType.Name}_{propertyName}";
		// E.g., "Language_IsActive"
	}
}
```

**Translation Value**: Property name split by camel case:
- `Language_IsActive` → "Is Active"
- `User_FirstName` → "First Name"

---

### 2. **Radzen Component Keys**: `[Component_Details]`

**Purpose**: Localize built-in Radzen component UI strings (buttons, menus, confirmations).

**Source File**: `RadzenStrings.resx` (copied from Radzen.Blazor NuGet package)

**Examples**:
- `Spreadsheet_FileTab` → "File"
- `Spreadsheet_HomeTab` → "Home"
- `Spreadsheet_OK` → "OK"
- `Spreadsheet_Cancel` → "Cancel"

**Generation**: Parsed directly from embedded `RadzenStrings.resx` XML:
```csharp
private static IEnumerable<Resource> GetRadzenResources()
{
	XDocument doc = XDocument.Load(RadzenResxPath);
	var resources = doc.Descendants("data")
		.Select(element => new Resource
		{
			Key = element.Attribute("name")?.Value,  // Exact key as-is
			Description = $"Radzen: {element.Attribute("name")?.Value}"
		});
}
```

**Translation Value**: Exact value from the `.resx` file (preserved as-is, no transformation).

---

### 3. **Button Action Keys**: `Button_[Action]`

**Purpose**: Standardize button labels for common actions across the application.

**Predefined Actions**:
- `Button_Save`, `Button_Delete`, `Button_Cancel`, `Button_Submit`, `Button_Reset`
- `Button_Create`, `Button_Update`, `Button_Add`, `Button_Remove`, `Button_Edit`, `Button_View`
- `Button_Close`, `Button_Export`, `Button_Import`, `Button_Search`, `Button_Filter`
- `Button_Clear`, `Button_Apply`, `Button_Refresh`, `Button_Download`, `Button_Upload`
- `Button_Approve`, `Button_Reject`, `Button_Confirm`, `Button_Yes`, `Button_No`

**Usage in Razor**:
```razor
<RadzenButton Text="@L("Button_Save", "Save")" Click="@OnSave" />
<RadzenButton Text="@L("Button_Delete", "Delete")" Click="@OnDelete" />
<RadzenButton Text="@L("Button_Cancel", "Cancel")" Click="@OnCancel" />
```

**Translation Value**: Action name with proper casing:
- `Button_Save` → "Save"
- `Button_DeleteConfirm` → "Delete Confirm" (camel case split)

---

### 4. **Navigation Menu Keys**: `Nav_[MenuItem]`

**Purpose**: Localize menu items, navigation labels, and page titles.

**Entity-Based Navigation** (auto-discovered):
- `Nav_Languages` → "Languages"
- `Nav_Resources` → "Resources"
- `Nav_Users` → "Users"
- `Nav_Roles` → "Roles"

**Common Navigation Items**:
- `Nav_Home`, `Nav_Dashboard`, `Nav_Settings`, `Nav_Profile`
- `Nav_Help`, `Nav_About`

**Usage in Razor**:
```razor
<NavLink href="languages">@L("Nav_Languages", "Languages")</NavLink>
<NavLink href="dashboard">@L("Nav_Dashboard", "Dashboard")</NavLink>
```

**Translation Value**: Menu item name with proper casing:
- `Nav_Languages` → "Languages"
- `Nav_Dashboard` → "Dashboard"

---

## Seeding Process

### Step 1: LocalizationResourceSeedData.GetResources()

Called during database seeding to collect all resource keys:

```csharp
public static IEnumerable<Resource> GetResources()
{
	List<Resource> resources = [];
	HashSet<string> existingKeys = new(StringComparer.OrdinalIgnoreCase);

	// 1. Add entity property resources
	foreach (Type entityType in GetEntityTypes())
	{
		// E.g., Language_Name, Language_IsActive, etc.
	}

	// 2. Add Radzen component strings (from RadzenStrings.resx)
	var radzenResources = GetRadzenResources();
	foreach (var radzenResource in radzenResources)
	{
		if (!existingKeys.Contains(radzenResource.Key))
		{
			resources.Add(radzenResource);
			existingKeys.Add(radzenResource.Key);
		}
	}

	// 3. Add standard Button_ keys with duplicate checking
	var buttonKeys = GetStandardButtonKeys();
	foreach (var buttonResource in buttonKeys)
	{
		if (!existingKeys.Contains(buttonResource.Key))
		{
			resources.Add(buttonResource);
			existingKeys.Add(buttonResource.Key);
		}
	}

	// 4. Add standard Nav_ keys with duplicate checking
	var navKeys = GetStandardNavKeys();
	foreach (var navResource in navKeys)
	{
		if (!existingKeys.Contains(navResource.Key))
		{
			resources.Add(navResource);
			existingKeys.Add(navResource.Key);
		}
	}

	return resources;
}
```

**Key Points**:
- **Duplicate Checking**: Uses `HashSet<string>` to ensure no duplicate keys
- **Order**: Entity → Radzen → Buttons → Navigation
- **No Filtering**: All discovered resources are included; filtering happens at CRUD time

### Step 2: LocalizationResourceSeedData.GetTranslations()

Called to generate translation values for each resource, for each language (initially only English):

```csharp
public static IEnumerable<ResourceTranslation> GetTranslations(
	IEnumerable<Resource> resources, 
	IEnumerable<Language> languages)
{
	List<ResourceTranslation> translations = [];
	Dictionary<string, string> radzenTranslations = GetRadzenTranslations()
		.ToDictionary(x => x.Key, x => x.Value);

	foreach (Resource resource in resources)
	{
		string value;

		// Radzen resources: use exact value from .resx
		if (radzenTranslations.TryGetValue(resource.Key, out string? radzenValue))
		{
			value = radzenValue ?? string.Empty;
		}
		// Button_ resources: extract action name
		else if (resource.Key.StartsWith("Button_", StringComparison.OrdinalIgnoreCase))
		{
			string actionName = resource.Key["Button_".Length..];
			value = SplitCamelCase(actionName);  // "Button_Save" → "Save"
		}
		// Nav_ resources: extract menu name
		else if (resource.Key.StartsWith("Nav_", StringComparison.OrdinalIgnoreCase))
		{
			string navName = resource.Key["Nav_".Length..];
			value = SplitCamelCase(navName);    // "Nav_Dashboard" → "Dashboard"
		}
		// Entity property resources: extract and split property name
		else
		{
			string propertyName = resource.Key.Contains('_') 
				? resource.Key[(resource.Key.IndexOf('_') + 1)..] 
				: resource.Key;
			value = SplitCamelCase(propertyName);  // "Language_IsActive" → "Is Active"
		}

		// Create translation for each language
		foreach (Language language in languages)
		{
			translations.Add(new ResourceTranslation
			{
				Id = Guid.CreateVersion7().ToString(),
				ResourceId = resource.Id,
				LanguageId = language.Id,
				Value = value
			});
		}
	}

	return translations;
}
```

**Key Points**:
- **Type Detection**: Checks key prefix to determine how to extract the translation value
- **CamelCase Splitting**: Converts `IsActive` → "Is Active" using regex
- **Fallback Order**: Radzen exact → Button action → Nav menu → Entity property
- **Language Loop**: Creates one translation per language

### Step 3: LocalizationDbSeedService.SeedResourcesAsync()

Orchestrates the seeding:

```csharp
public static async Task SeedResourcesAsync(LocalizationDbContext dbContext, CancellationToken cancellationToken)
{
	// Get only English language
	List<Language> languages = await dbContext.Languages
		.Where(language => language.Code == "en")
		.ToListAsync(cancellationToken);

	// Get all resources (entity, Radzen, Button_, Nav_)
	IEnumerable<Resource> allResources = LocalizationResourceSeedData.GetResources();

	// Filter out existing resources to avoid duplicates
	List<Resource> existingResources = await dbContext.Resources
		.AsNoTracking()
		.ToListAsync(cancellationToken);

	IEnumerable<Resource> missingResources = allResources
		.Where(seedResource => existingResources.All(
			existingResource => existingResource.Key != seedResource.Key));

	// Insert missing resources
	if (missingResources.Count() > 0)
	{
		await dbContext.Resources.AddRangeAsync(missingResources, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
	}

	// Get all resources again (now including the newly inserted ones)
	allResources = await dbContext.Resources
		.AsNoTracking()
		.OrderBy(resource => resource.Key)
		.ToListAsync(cancellationToken);

	// Generate translations
	IEnumerable<ResourceTranslation> allTranslations = LocalizationResourceSeedData
		.GetTranslations(allResources, languages);

	// Filter out existing translations
	List<ResourceTranslation> existingTranslations = await dbContext.ResourceTranslations
		.AsNoTracking()
		.ToListAsync(cancellationToken);

	IEnumerable<ResourceTranslation> missingTranslations = allTranslations
		.Where(seedTranslation => existingTranslations.All(existingTranslation =>
			existingTranslation.ResourceId != seedTranslation.ResourceId ||
			existingTranslation.LanguageId != seedTranslation.LanguageId));

	// Insert missing translations
	if (missingTranslations.Count() > 0)
	{
		await dbContext.ResourceTranslations.AddRangeAsync(missingTranslations, cancellationToken);
		await dbContext.SaveChangesAsync(cancellationToken);
	}
}
```

---

## UI Component Usage Pattern

### Using Localizer in Razor Components

Inject the `Radzen.ILocalizer` (implemented as `Localizer`):

```csharp
@inject Radzen.ILocalizer L
```

Use the `GetOrDefault` method for synchronous access in component attributes:

```razor
<RadzenButton 
	Text="@L.GetOrDefault("Button_Save", "Save")" 
	Click="@OnSave" 
	ButtonStyle="ButtonStyle.Primary" />

<RadzenButton 
	Text="@L.GetOrDefault("Button_Delete", "Delete")" 
	Click="@OnDelete" 
	ButtonStyle="ButtonStyle.Danger" />

<RadzenDataGrid Data="@_items">
	<Columns>
		<RadzenDataGridColumn 
			Title="@L.GetOrDefault("Language_Name", "Name")" 
			Property="Name" />
		<RadzenDataGridColumn 
			Title="@L.GetOrDefault("Language_Code", "Code")" 
			Property="Code" />
	</Columns>
</RadzenDataGrid>
```

### Preloading in Code-Behind

Preload keys during component initialization for better performance:

```csharp
@code {
	[Inject]
	private Radzen.ILocalizer L { get; set; } = null!;

	protected override async Task OnInitializedAsync()
	{
		var keysToPreload = new[]
		{
			"Button_Save",
			"Button_Delete", 
			"Button_Cancel",
			"Language_Name",
			"Language_Code",
			"Language_IsActive"
		};

		await ((Localizer)L).PreloadAsync(keysToPreload);
	}
}
```

### Formatting with Arguments

For parameterized messages:

```csharp
string message = L.Format("Message_SuccessDeleted", "Record deleted", "Record#123");
// Returns: "Record#123 deleted" (if translation supports {0})
```

---

## Best Practices

### DO ✓

1. **Use consistent key naming**: Always follow `[Prefix]_[Name]` pattern
2. **Provide meaningful fallback values**: Fallback = English translation
3. **Preload common keys**: Call `PreloadAsync()` in component init
4. **Use Button_ keys** for all action buttons instead of hardcoded strings
5. **Use Nav_ keys** for navigation items and menu labels
6. **Check for duplicates** before inserting new keys in custom seed data
7. **Use exact Radzen values**: Don't transform Radzen strings from `.resx`

### DON'T ✗

1. **Don't hardcode UI strings**: Always use localization keys and fallbacks
2. **Don't create duplicate keys**: Check existing keys before adding new ones
3. **Don't await in Razor attributes**: Pre-load values in lifecycle methods
4. **Don't transform Radzen values**: Preserve them exactly as in `.resx`
5. **Don't mix naming conventions**: Use one pattern per resource type
6. **Don't forget fallback values**: Every call should provide a fallback

---

## Adding Custom Localization Keys at Runtime

For custom keys not in the seeding data, use the backend auto-create feature:

1. Call the localization endpoint with a new key and fallback value
2. Backend automatically creates the resource in English locale
3. Component receives English value immediately
4. Keys become available for future translations

**Example**:
```csharp
string result = await Localization.ResolveAsync(
	"Custom_MyFeature_Title",
	"My Feature Title");
// If key doesn't exist, backend creates it with English translation
// Next calls will use the created resource
```

---

## File Locations

- **Seeding Logic**: `src/AditiKraft.Krafter.Backend/Infrastructure/Persistence/Seeding/LocalizationResourceSeedData.cs`
- **Radzen Strings**: `src/AditiKraft.Krafter.Backend/Infrastructure/Persistence/Seeding/RadzenStrings.resx`
- **Seeding Service**: `src/AditiKraft.Krafter.Backend/Infrastructure/Persistence/Seeding/LocalizationDbSeedService.cs`
- **UI Localizer**: `src/UI/AditiKraft.Krafter.UI.Web.Client/Features/Localization/Localizer.cs`
- **Backend Resolver**: `src/AditiKraft.Krafter.Backend/Features/Localization/Common/LocalizationTextResolver.cs`
