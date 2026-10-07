namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Brand;

public partial class Logo(ThemeManager themeManager) : IDisposable
{
    [Parameter] public LogoSize Size { get; set; } = LogoSize.Medium;

    [Parameter] public LogoOrientation Orientation { get; set; } = LogoOrientation.Horizontal;

    private string _currentLogo = string.Empty;

    protected override void OnInitialized()
    {
        // Current = this;

        themeManager.ThemeChangeRequested += OnThemeChangedAsync;
    }

    public string GetAlt()
    {
        if (Size == LogoSize.ExtraSmall)
        {
            return "extra small logo";
        }
        else if (Size == LogoSize.Small)
        {
            return "small logo";
        }
        else if (Size == LogoSize.Medium)
        {
            return "medium logo";
        }
        else if (Size == LogoSize.Large)
        {
            return "large logo";
        }
        else if (Size == LogoSize.ExtraLarge)
        {
            return "extra large logo";
        }
        else
        {
            return "medium logo";
        }
    }

    private string GetSource()
    {
        string orientationDirection = "horizontal";
        if (Orientation == LogoOrientation.Vertical)
        {
            orientationDirection = "vertical";
        }

        string res = "";
        if (themeManager.CurrentActive == ThemeManager.ThemePreference.Dark)
        {
            res = "dark";
        }
        else
        {
            res = "light";
        }

        if (Size == LogoSize.ExtraSmall)
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-xs-{res}.svg";
        }
        else if (Size == LogoSize.Small)
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-s-{res}.svg";
        }
        else if (Size == LogoSize.Medium)
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-m-{res}.svg";
        }
        else if (Size == LogoSize.Large)
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-l-{res}.svg";
        }
        else if (Size == LogoSize.ExtraLarge)
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-xl-{res}.svg";
        }
        else
        {
            _currentLogo =
                $"brand/logos/{orientationDirection}/logo-{orientationDirection}-m-{res}.svg";
        }

        return _currentLogo;
    }

    private async Task OnThemeChangedAsync(string preference)
    {
        GetSource();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose() => themeManager.ThemeChangeRequested -= OnThemeChangedAsync;
}

public enum LogoSize
{
    ExtraSmall = 0,
    Small = 1,
    Medium = 2,
    Large = 3,
    ExtraLarge = 4
}

public enum LogoOrientation
{
    Horizontal = 0,
    Vertical = 1
}
