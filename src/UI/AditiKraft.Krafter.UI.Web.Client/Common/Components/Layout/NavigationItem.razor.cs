using AditiKraft.Krafter.UI.Web.Client.Common.Models;
using AditiKraft.Krafter.UI.Web.Client.Features.Localization;

namespace AditiKraft.Krafter.UI.Web.Client.Common.Components.Layout;

public partial class NavigationItem()
{
    [Inject] private Radzen.ILocalizer L { get; set; } = null!;
    [EditorRequired]
    [Parameter]
    public Menu Example { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

    [Parameter] public bool Expanded { get; set; }

    protected override void OnParametersSet()
    {
        Example?.Expanded = Expanded;
    }

    private string? GetUrl()
    {
        return string.IsNullOrEmpty(Example.Path) ? null : Example.Path;
    }

    private string GetLocalizedName()
    {
        string localizationKey = !string.IsNullOrWhiteSpace(Example.NameLocalizationKey)
            ? Example.NameLocalizationKey
            : $"Nav_{Example.Name.Replace(" ", string.Empty)}";

        return L.Localize(localizationKey, Example.Name);
    }
}
