using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using AditiKraft.Krafter.Contracts.Common.Auth;
using AditiKraft.Krafter.Contracts.Contracts.Localization;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;
using AditiKraft.Krafter.UI.Web.Client.Features.AppInfo;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;
using AditiKraft.Krafter.UI.Web.Client.Features.Users;
using AditiKraft.Krafter.UI.Web.Client.Infrastructure.Auth;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Layout;

public partial class MainLayout(
    ApiCallService api,
    IAppInfoApi appInfoApi,
#pragma warning disable CS9113 // Parameter is unread.
    CookieThemeService cookieThemeService,
#pragma warning restore CS9113 // Parameter is unread.
    MenuService menuService,
    LayoutService layoutService,
    IAuthenticationService authenticationService,
#pragma warning disable CS9113 // Parameter is unread.
    ThemeService themeService,
    TooltipService tooltipService,
#pragma warning restore CS9113 // Parameter is unread.
    ThemeManager themeManager,
    IAuthStorageService authStorageService,
    IUsersApi usersApi
) : IDisposable
{
    [Inject]
    private ILocalizer L { get; set; } = null!;

    [CascadingParameter]
    public bool IsMobileDevice { get; set; }

    private bool _sidebarExpanded = true;
    private bool _configSidebarExpanded = false;
    private bool _rendered;

    private IEnumerable<Menu> _menus = new List<Menu>();
    private List<LanguageDto> _availableLanguages = new();
    private string _selectedLanguageCode = "en";

    private ICollection<string>? _cachedPermissions = new List<string>();
    public Response<string>? AppInfo { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _rendered = true;
        }
    }

    protected override async Task OnInitializedAsync()
    {
        AppInfo = await api.CallAsync(() => appInfoApi.GetAppInfoAsync(), showErrorNotification: false);
        _cachedPermissions = await authStorageService.GetCachedPermissionsAsync() ?? new List<string>();
        _menus = menuService.Menus;
        await PreloadMenuLocalizationAsync(_menus);
        await LoadLanguagesAsync();
        await LoadSelectedLanguageFromTokenAsync();

        layoutService.HeadingChanged += HeadingChanged;
        authenticationService.LoginChange += async name =>
        {
            _cachedPermissions = await authStorageService.GetCachedPermissionsAsync() ?? new List<string>();
            _menus = menuService.Menus;
            await PreloadMenuLocalizationAsync(_menus);
            await LoadLanguagesAsync();
            await LoadSelectedLanguageFromTokenAsync();
            await InvokeAsync(StateHasChanged);
        };
    }

    private void FilterPanelMenu(ChangeEventArgs args)
    {
        string? term = args.Value?.ToString();
        _menus = string.IsNullOrEmpty(term) ? menuService.Menus : menuService.Filter(term);
    }

    private async Task LoadLanguagesAsync()
    {
        Response<List<LanguageDto>> response = await api.CallAsync(() => usersApi.GetLanguagesAsync(),
            showErrorNotification: false);

        _availableLanguages = response.Data?
            .Where(language => language.IsActive)
            .OrderBy(language => language.Name)
            .ToList() ?? new List<LanguageDto>();

        if (_availableLanguages.Count == 0)
        {
            _availableLanguages.Add(new LanguageDto { Code = "en", Name = "English", IsActive = true });
        }
    }

    private async Task LoadSelectedLanguageFromTokenAsync()
    {
        string? cachedToken = await authStorageService.GetCachedAuthTokenAsync();
        string selectedCode = "en";

        if (!string.IsNullOrWhiteSpace(cachedToken))
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(cachedToken);
            string? tokenLanguageCode = token.Claims.FirstOrDefault(c => c.Type == AppClaimTypes.PreferredLanguageCode)?.Value;
            if (!string.IsNullOrWhiteSpace(tokenLanguageCode))
            {
                selectedCode = tokenLanguageCode.Trim().ToLowerInvariant();
            }
        }

        _selectedLanguageCode = selectedCode;
        ApplyCurrentCulture(selectedCode);
    }

    private async Task ChangeLanguageAsync(object value)
    {
        string code = value?.ToString()?.Trim().ToLowerInvariant() ?? "en";
        if (string.IsNullOrWhiteSpace(code))
        {
            code = "en";
        }

        if (string.Equals(code, _selectedLanguageCode, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Response response = await api.CallAsync(
            () => usersApi.UpdatePreferredLanguageAsync(new UpdatePreferredLanguageRequest { PreferredLanguageCode = code }),
            successMessage: L.Localize("Notification_Success", "Record updated successfully"));

        if (response.IsError)
        {
            return;
        }

        bool synchronized = await authenticationService.SynchronizeAsync();
        if (!synchronized)
        {
            await authenticationService.RefreshAsync();
        }

        _selectedLanguageCode = code;
        ApplyCurrentCulture(code);

        // Invalidate localizer cache for new language
        Localizer? localizer = L as Localizer;
        localizer?.InvalidateCache();
        await PreloadMenuLocalizationAsync(_menus);

        await InvokeAsync(StateHasChanged);
    }

    private async Task PreloadMenuLocalizationAsync(IEnumerable<Menu> menus)
    {
        if (L is not Localizer localizer)
        {
            return;
        }

        IEnumerable<Menu> Flatten(IEnumerable<Menu> source)
        {
            return source.SelectMany(menu =>
                menu.Children is not null && menu.Children.Any()
                    ? new[] { menu }.Concat(Flatten(menu.Children))
                    : new[] { menu });
        }

        string[] keys = Flatten(menus)
            .Select(menu => !string.IsNullOrWhiteSpace(menu.NameLocalizationKey)
                ? menu.NameLocalizationKey
                : $"Nav_{menu.Name.Replace(" ", string.Empty)}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        await localizer.PreloadAsync(keys).ConfigureAwait(false);
    }

    private static void ApplyCurrentCulture(string languageCode)
    {
        string normalizedCode = string.IsNullOrWhiteSpace(languageCode)
            ? "en"
            : languageCode.Trim().ToLowerInvariant();

        try
        {
            CultureInfo culture = new(normalizedCode);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            CultureInfo fallbackCulture = new("en");
            CultureInfo.CurrentCulture = fallbackCulture;
            CultureInfo.CurrentUICulture = fallbackCulture;
            CultureInfo.DefaultThreadCurrentCulture = fallbackCulture;
            CultureInfo.DefaultThreadCurrentUICulture = fallbackCulture;
        }
    }

    private void HeadingChanged(object? sender, EventArgs e) => InvokeAsync(StateHasChanged);

    public void Dispose() => layoutService.HeadingChanged -= HeadingChanged;

    private bool HasChildPermission(Menu category)
    {
        IEnumerable<string> childPermission =
            category.Children?.Select(c => c.Permission) ?? Enumerable.Empty<string>();

        return childPermission.Intersect(_cachedPermissions ?? Enumerable.Empty<string>()).Any();
    }

    private bool HasPermission(Menu category)
    {
        if (string.IsNullOrWhiteSpace(category.Permission) &&
            (category.Children is null || !category.Children.Any()))
        {
            return true;
        }

        return _cachedPermissions?.Contains(category.Permission) == true;
    }
}
