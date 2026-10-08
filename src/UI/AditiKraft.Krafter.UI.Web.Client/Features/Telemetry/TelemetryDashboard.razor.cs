using AditiKraft.Krafter.Contracts.Common;
using AditiKraft.Krafter.UI.Web.Client.Common.Models;

namespace AditiKraft.Krafter.UI.Web.Client.Features.Telemetry;

public partial class TelemetryDashboard(AppUrls appUrls) : ComponentBase
{
    public const string RoutePath = "/telemetry-dashboard";

    private string MetricsUrl => BuildMetricsUrl();

    protected override void OnInitialized()
    {
        LocalAppState.CurrentPageTitle = "Telemetry Dashboard";
    }

    private string BuildMetricsUrl()
    {
        Uri metricsUri = appUrls.GetGrafanaUri()
            ?? (appUrls.GetApiUri() is Uri apiUri
                ? new Uri(apiUri, "metrics")
                : new Uri("/metrics", UriKind.Relative));

        return metricsUri.ToString();
    }
}
