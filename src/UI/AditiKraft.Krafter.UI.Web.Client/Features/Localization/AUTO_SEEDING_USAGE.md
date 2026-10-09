# Auto-Seeding Localization - Inline Pattern

This guide explains how to use **inline fire-and-forget localization** with automatic server auto-seeding.

## Overview

The localization system supports **inline synchronous use in Razor markup** that automatically triggers background server requests to create missing keys:

- **Immediate response:** Returns cached value or fallback instantly (non-blocking)
- **Background resolution:** Missing keys trigger fire-and-forget HTTP POST to server
- **Server auto-seeding:** Backend creates missing Resource + Translation records
- **Automatic caching:** Resolved value is cached locally for next render
- **Next render:** Shows localized value instead of fallback

## Quick Start

### Simplest Usage - Inline in Markup

```razor
@inject Radzen.ILocalizer L

<h1>@L.Localize("HomePage_Title", "Welcome")</h1>
<p>@L.Localize("HomePage_Subtitle", "This is our home page")</p>
<button>@L.Localize("Button_SaveChanges", "Save Changes")</button>
```

**What happens:**
1. First render: Returns fallback values (instant)
2. Background: Fires HTTP POST to server for each missing key
3. Server: Auto-creates Resource + Translation for each missing key
4. Cache: Result is stored locally
5. Next render: Shows localized values

### With Format Arguments

```razor
<p>@L.Localize("Greeting_User", "Hello {0}!", userName)</p>
<p>@L.Localize("Items_Count", "You have {0} items", itemCount)</p>
```

## Workflow Diagram

```
┌─────────────────────────────┐
│ Component renders           │
│ @L.Localize("Key", "fallback")
└──────────────┬──────────────┘
			   │
	   ┌───────▼────────┐
	   │ Check local    │ No
	   │ cache for "Key"├──────┐
	   └───────┬────────┘      │
		 Yes   │               │
			   │         ┌─────▼──────────────────┐
			   │         │ Return fallback value  │
			   │         │ (instantly, blocking)  │
			   │         │                        │
			   │         │ Fire background task:  │
			   │         │ ResolveAndCacheAsync() │
			   └────┬────┴─────────────────────────┘
					│
					▼
			 ┌────────────────────┐
			 │ Return cached value│
			 │ to component for   │
			 │ rendering          │
			 └────────────────────┘

[Background Task - Fire and Forget]
					│
			┌───────▼─────────┐
			│ HTTP POST to    │
			│ backend Resolve │
			│ endpoint        │
			└───────┬─────────┘
					│
			┌───────▼──────────────┐
			│ Backend checks DB    │
			│ for key              │
			└───────┬──────────────┘
					│
		┌───────────┴───────────┐
		│                       │
   Exists              Doesn't exist
		│                       │
		│           ┌───────────▼────────────┐
		│           │ Auto-create:           │
		│           │ - Resource record      │
		│           │ - Translation (English)│
		│           │ - SaveChanges()        │
		│           └───────────┬────────────┘
		│                       │
		└───────────┬───────────┘
					│
			┌───────▼─────────┐
			│ Return value    │
			│ to client       │
			└───────┬─────────┘
					│
			┌───────▼──────────┐
			│ Store in local   │
			│ _cache           │
			└──────────────────┘

[On next render of same key: uses cached value]
```

## Two Scenarios

### Scenario 1: Known Keys (Pre-seeded in Database)

```razor
<!-- Keys already exist in database, just fetching translations -->
<h1>@L.Localize("HomePage_Title", "Home")</h1>
```

1. First render: Returns "Home" (fallback)
2. Background check: Server finds existing translation
3. Cache: Stores actual translation
4. Next render: Shows actual translation

### Scenario 2: Dynamic Keys (Auto-seeded from Fallback)

```razor
<!-- Keys don't exist; fallback value becomes the resource -->
<p>@L.Localize("DynamicStatus_Active", "Active")</p>
<p>@L.Localize("DynamicStatus_Inactive", "Inactive")</p>
```

1. First render: Returns fallback (instant)
2. Background check: Server doesn't find "DynamicStatus_Active"
3. Auto-create: Creates Resource("DynamicStatus_Active") with Translation value="Active"
4. Cache: Stores "Active"
5. Next render: Shows "Active", knows key exists

## When Key is NOT Cached Yet

**Render 1 (missing cache):**
```
@L.Localize("HomePage_Title", "Welcome")
↓
Cache miss → check database → not found
↓
Return fallback "Welcome" immediately
↓
Fire background: POST resolve request
↓
Render shows: "Welcome"
```

**Render 2 (after background completes):**
```
@L.Localize("HomePage_Title", "Welcome")
↓
Cache HIT → found "Welcome to HomePage"
↓
Return cached value "Welcome to HomePage" immediately
↓
Render shows: "Welcome to HomePage"
```

## Fire-and-Forget Behavior

The background resolution is truly **fire-and-forget**:

- No `await` required in markup
- No `Task.Wait()` or blocking
- Returns fallback in microseconds
- HTTP request happens in background
- Result stored in cache

**Example:** Multiple missing keys on first render

```razor
<h1>@L.Localize("Feature_Title", "Features")</h1>
<p>@L.Localize("Feature_Description", "Manage all features")</p>
<button>@L.Localize("Feature_Add", "Add Feature")</button>
```

1. All 3 lines render instantly with fallback values
2. 3 background POST requests fire simultaneously
3. Server auto-creates 3 Resource+Translation records
4. Next page load or re-render: Shows actual translations

## Preventing Duplicate Requests

The system uses `ConcurrentHashSet<string> _pendingResolutions` to prevent duplicate concurrent requests:

```csharp
// First call
@L.Localize("Key", "fallback")  // Fires POST, adds Key to _pendingResolutions

// Simultaneous second call (same render or race condition)
@L.Localize("Key", "fallback")  // Skips duplicate POST, uses same in-flight request

// After first POST completes
// Both calls use same cached result
```

This protects against:
- Multiple components using same key on same page
- Repeated rapid calls to same key
- Concurrent renders evaluating same localization

## Formatting with Fire-and-Forget

Formatting works with fire-and-forget pattern:

```razor
<!-- On first render: shows "Hello {0}!" with fallback value -->
<p>@L.Localize("Greeting", "Hello {0}!", "Alice")</p>

<!-- On next render after background resolves: shows localized value formatted -->
<p>@L.Localize("Greeting", "Hello {0}!", "Alice")</p>
```

## For Pre-resolved Keys: Use Component Init

If you need a key resolved BEFORE rendering (e.g., for page title, critical content):

```csharp
@inject Radzen.ILocalizer L
@code {
	private string pageTitle = "Loading...";

	protected override async Task OnInitializedAsync()
	{
		// This awaits the server response before rendering starts
		pageTitle = await L.ResolveAndCacheAsync("PageTitle", "Default Title");
		await base.OnInitializedAsync();
	}
}
```

Then in markup, use cached value:
```razor
<h1>@pageTitle</h1>
```

## Caching Behavior

- **Lifetime:** Localizer instance lifetime (scoped per request)
- **Invalidation:** Call `L.InvalidateCache()` on language change
- **Size:** Unbounded (keys accumulate per page session)

## Error Handling

All background failures are silent and non-blocking:

```razor
<!-- Even if server is down, fallback is returned -->
<h1>@L.Localize("Title", "My Title")</h1>

<!-- Component renders immediately regardless of background HTTP status -->
```

On background POST failure:
- Fallback value is cached
- No error is thrown
- Next render uses same fallback
- User sees consistent UI

## Monitoring Auto-Seeding

Check backend logs for auto-creation events:

```
Information: Resolving missing localization for key 'HomePage_Title'. 
			 MissingPreferred: True, MissingEnglish: True

Information: Resource key 'HomePage_Title' not found, 
			 auto-creating with fallback value: 'Welcome'

Information: Translation for key 'HomePage_Title' in English not found, 
			 auto-creating translation...

Information: Saving auto-created localization resources for key 'HomePage_Title'
```

## Best Practices

1. **Use `@L.Localize()` for inline markup** - Fire-and-forget, instant rendering
2. **Use `await L.ResolveAndCacheAsync()` before rendering** - For critical content
3. **Use `PreloadAsync()` for batch efficiency** - Multiple keys in one HTTP request
4. **Provide meaningful fallback values** - They become the auto-created resource values
5. **Use consistent key naming** - e.g., `Feature_PropertyName`, `Button_Action`
6. **Call `InvalidateCache()` on language change** - Clears local cache
7. **No preloading required** - Fire-and-forget handles missing keys automatically

## Examples

### Simple Page

```razor
@page "/"
@inject Radzen.ILocalizer L

<h1>@L.Localize("HomePage_Title", "Welcome")</h1>
<p>@L.Localize("HomePage_Subtitle", "This is the home page")</p>

<div class="features">
	<h2>@L.Localize("Features_Title", "Features")</h2>
	<p>@L.Localize("Features_Description", "Discover our features")</p>
</div>

<button class="btn btn-primary">
	@L.Localize("Button_GetStarted", "Get Started")
</button>
```

**Renders instantly, background requests auto-seed missing keys.**

### Page with Pre-resolved Title

```razor
@page "/dashboard"
@inject Radzen.ILocalizer L

@code {
	private string pageTitle = "Loading...";

	protected override async Task OnInitializedAsync()
	{
		// Resolve before rendering to avoid fallback on first render
		pageTitle = await L.ResolveAndCacheAsync("Dashboard_Title", "Dashboard");
	}
}

<PageTitle>@pageTitle</PageTitle>

<h1>@pageTitle</h1>
<p>@L.Localize("Dashboard_Welcome", "Welcome to your dashboard")</p>
```

### Dynamic Content with Fire-and-Forget

```razor
@foreach (var status in statuses)
{
	<!-- Each status key is auto-seeded if missing -->
	<span class="badge">@L.Localize($"Status_{status}", status)</span>
}
```
