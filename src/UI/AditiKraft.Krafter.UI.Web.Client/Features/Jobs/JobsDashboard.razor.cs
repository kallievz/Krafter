using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Jobs;

public partial class JobsDashboard(AppUrls appUrls) : ComponentBase
{
    public const string RoutePath = "/jobs-dashboard";

    private string DashboardUrl => BuildDashboardUrl();

    protected override void OnInitialized()
    {
        LocalAppState.CurrentPageTitle = "Jobs Dashboard";
    }

    private string BuildDashboardUrl()
    {
        Uri dashboardUri = appUrls.GetApiUri() is Uri apiUri
            ? new Uri(apiUri, "tickerq/")
            : new Uri("/tickerq/", UriKind.Relative);

        return dashboardUri.ToString();
    }
}
