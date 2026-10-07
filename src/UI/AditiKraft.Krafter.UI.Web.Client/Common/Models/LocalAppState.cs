namespace AditiKraft.Krafter.UI.Web.Client.Common.Models;

public static class LocalAppState
{
    public static string CurrentPageTitle { get; set; } = "Home"; //Density

    public static Density Density { get; set; } = Density.Compact;
    public static bool AllowColumnResize { get; set; } = true;
    public static bool ShowCellDataAsTooltip { get; set; } = true;
    public static string DateFormat { get; set; } = "yyyy/MM/dd";

    //12 hour date time format
    public static string DateTimeFormat { get; set; } = "yyyy/MM/dd hh:mm tt";

    public static string TimeFormat { get; set; } = "hh:mm tt";

    public static string GoogleLoginReturnUrl { get; set; } = string.Empty;
    //
}
