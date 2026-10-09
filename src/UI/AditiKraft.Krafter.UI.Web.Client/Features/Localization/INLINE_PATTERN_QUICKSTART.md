# Inline Localization Quick Start

The **new pattern** for localization uses inline fire-and-forget requests. No preloading needed.

## Simplest Pattern

Just use `@L.Localize()` directly in Razor markup:

```razor
@inject Radzen.ILocalizer L

<h1>@L.Localize("HomePage_Title", "Welcome")</h1>
<p>@L.Localize("HomePage_Subtitle", "Main page")</p>
<button>@L.Localize("Button_Submit", "Submit")</button>
```

**That's it!** 

- ✅ Returns fallback instantly (non-blocking)
- ✅ Fires background HTTP POST to server
- ✅ Server auto-creates missing keys + translations
- ✅ Next render uses cached value
- ✅ No preloading, no async/await in markup

## With Arguments

```razor
<p>@L.Localize("Greeting", "Hello {0}!", userName)</p>
<p>@L.Localize("Items", "You have {0} items", count)</p>
```

## Full Example

```razor
@page "/"
@inject Radzen.ILocalizer L

<PageTitle>@L.Localize("HomePage_Title", "Home")</PageTitle>

<div class="container">
	<h1>@L.Localize("HomePage_Heading", "Welcome to Our Site")</h1>

	<section>
		<h2>@L.Localize("Features_Title", "Our Features")</h2>
		<p>@L.Localize("Features_Description", "Check out what we offer")</p>
	</section>

	<div class="actions">
		<button class="btn btn-primary">
			@L.Localize("Button_SignUp", "Sign Up")
		</button>
		<button class="btn btn-secondary">
			@L.Localize("Button_LearnMore", "Learn More")
		</button>
	</div>
</div>

@code {
	// No localization code needed here!
	// Markup handles everything with fire-and-forget pattern
}
```

## If You Need Value BEFORE Render

For page titles, meta tags, or component initialization that needs the localized value:

```csharp
@code {
	private string pageTitle = "Loading...";

	protected override async Task OnInitializedAsync()
	{
		// Await this to get value before first render
		pageTitle = await L.ResolveAndCacheAsync("Page_Title", "Default");
	}
}
```

Then use it in markup:
```razor
<PageTitle>@pageTitle</PageTitle>
<h1>@pageTitle</h1>
```

## Runtime Behavior

**First render:**
```
@L.Localize("Key", "fallback") → Cache miss → Return "fallback" → Fire POST
Output: "fallback"
```

**Background task:**
```
POST /localization/resolve with key="Key", fallback="fallback"
Server: Check DB → Not found → Create Resource + Translation → Return "fallback"
Cache: Store "fallback" locally
```

**Next render (after cache populated):**
```
@L.Localize("Key", "fallback") → Cache hit → Return cached value
Output: Actual localized value (or "fallback" if same)
```

## FAQ

**Q: Doesn't first render show fallback?**  
A: Yes, but only on first render. Next time that key appears, it's cached and renders correctly. This is the fire-and-forget trade-off: instant UI + background sync.

**Q: What if the server is down?**  
A: Fallback is used and cached. Component renders normally. Next render also uses same fallback.

**Q: Do I need to handle errors?**  
A: No. All errors are silent and non-blocking. Fallback is always returned.

**Q: What about language changes?**  
A: Call `L.InvalidateCache()` when language changes to clear cache. Next `@L.Localize()` call will fire new background request.

**Q: Can I use this with Radzen components?**  
A: Yes. Radzen components use `Radzen.ILocalizer` internally, and our implementation handles that.

```razor
<RadzenButton Text="@L.Localize("Button_Delete", "Delete")" />
<RadzenLabel Text="@L.Localize("Label_Email", "Email Address")" />
```

**Q: Multiple calls to same key?**  
A: Automatically deduplicated. If key is being resolved in background, subsequent calls skip duplicate POST and use same in-flight result.

## Rules of Thumb

1. **Use `@L.Localize()` in markup** ← Default choice
2. **Use `await L.ResolveAndCacheAsync()` in code** ← Only when you need value before render
3. **Use `L.PreloadAsync()` for batch efficiency** ← Optional, for multiple keys at once
4. **Call `L.InvalidateCache()` on language change** ← Clears all cache

## Key Advantage

**No preloading ceremony!** Old pattern required:

```csharp
// OLD - required preloading
protected override async Task OnInitializedAsync()
{
	await L.PreloadAsync(new[] { "Key1", "Key2", "Key3" });
}
```

**New pattern is simpler:**

```csharp
// NEW - just use it, background handles caching
<h1>@L.Localize("Key1", "fallback")</h1>
<p>@L.Localize("Key2", "fallback")</p>
<button>@L.Localize("Key3", "fallback")</button>
```

That's it!
