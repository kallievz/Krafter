IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

string solutionRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".."));
string migratorProject = Path.Combine(solutionRoot, "src", "AditiKraft.Krafter.Backend.Migrator",
    "AditiKraft.Krafter.Backend.Migrator.csproj");
string observabilityRoot = Path.Combine(solutionRoot, "aspire", "AditiKraft.Krafter.Aspire.AppHost", "Observability");
string prometheusConfigRoot = Path.Combine(observabilityRoot, "prometheus");
string grafanaProvisioningRoot = Path.Combine(observabilityRoot, "grafana", "provisioning");
string grafanaDashboardsRoot = Path.Combine(observabilityRoot, "grafana", "dashboards");
string appConnectionString = builder.Configuration["ConnectionStrings:appDb"]
    ?? throw new InvalidOperationException("Connection string 'appDb' not found.");
Dictionary<string, string> appDbConnection = ParseConnectionString(appConnectionString);

IResourceBuilder<IResourceWithConnectionString> database = builder.AddConnectionString("appDb");

IResourceBuilder<ContainerResource> prometheus = builder.AddContainer("prometheus", "prom/prometheus:latest")
    .WithHttpEndpoint(targetPort: 9090, port: 9090, name: "http")
    .WithArgs("--config.file=/etc/prometheus/prometheus.yml")
    .WithBindMount(prometheusConfigRoot, "/etc/prometheus", isReadOnly: true);

IResourceBuilder<ContainerResource> grafana = builder.AddContainer("grafana", "grafana/grafana:latest")
    .WithHttpEndpoint(targetPort: 3000, port: 3000, name: "http")
    .WithBindMount(Path.Combine(grafanaProvisioningRoot, "datasources"), "/etc/grafana/provisioning/datasources", isReadOnly: true)
    .WithBindMount(Path.Combine(grafanaProvisioningRoot, "dashboards"), "/etc/grafana/provisioning/dashboards", isReadOnly: true)
    .WithBindMount(grafanaDashboardsRoot, "/var/lib/grafana/dashboards", isReadOnly: true)
    .WithEnvironment("GF_PATHS_PROVISIONING", "/etc/grafana/provisioning")
    .WithEnvironment("GF_SECURITY_ALLOW_EMBEDDING", "true")
    .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "true")
    .WithEnvironment("GF_AUTH_ANONYMOUS_ORG_ROLE", "Admin")
    .WithEnvironment("GF_AUTH_DISABLE_LOGIN_FORM", "true")
    .WithEnvironment("GF_USERS_DEFAULT_THEME", "light")
    .WithEnvironment("GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH", "/var/lib/grafana/dashboards/krafter-overview.json")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_NAME", "SqlServer")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_TYPE", "mssql")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_ACCESS", "proxy")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_ISDEFAULT", "false")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_HOST", GetValueOrDefault(appDbConnection, "Data Source"))
    .WithEnvironment("GF_DATASOURCES_DEFAULT_DATABASE", GetValueOrDefault(appDbConnection, "Initial Catalog"))
    .WithEnvironment("GF_DATASOURCES_DEFAULT_USER", GetValueOrDefault(appDbConnection, "User ID"))
    .WithEnvironment("GF_DATASOURCES_DEFAULT_PASSWORD", GetValueOrDefault(appDbConnection, "Password"))
    .WithEnvironment("GF_DATASOURCES_DEFAULT_SSLMODE", GetValueOrDefault(appDbConnection, "Encrypt") == "True" ? "require" : "disable")
    .WithEnvironment("GF_DATASOURCES_DEFAULT_TRUSTSERVERCERTIFICATE", GetValueOrDefault(appDbConnection, "TrustServerCertificate").ToLowerInvariant())
    .WaitFor(prometheus);

IResourceBuilder<ExecutableResource> migrator = builder.AddExecutable(
        "app-migrator",
        "dotnet",
        solutionRoot,
        "run",
        "--project",
        migratorProject,
        "--no-launch-profile")
    .WithReference(database)
    .WithEnvironment("DOTNET_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WaitFor(database);

IResourceBuilder<ProjectResource> backend = builder.AddProject<Projects.AditiKraft_Krafter_Backend>("api")
    .WithReference(database)
    .WaitForCompletion(migrator);

IResourceBuilder<ProjectResource> web = builder.AddProject<Projects.AditiKraft_Krafter_UI_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(backend)
    .WithReference(database)
    .WaitFor(backend);

web.WithEnvironment("Urls__GrafanaUrl", grafana.GetEndpoint("http"));

builder.Build().Run();

static Dictionary<string, string> ParseConnectionString(string connectionString)
{
    Dictionary<string, string> values = new(StringComparer.OrdinalIgnoreCase);
    foreach (string segment in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        int separatorIndex = segment.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        string key = segment[..separatorIndex].Trim();
        string value = segment[(separatorIndex + 1)..].Trim().Trim('"');
        if (!string.IsNullOrWhiteSpace(key) && !values.ContainsKey(key))
        {
            values[key] = value;
        }
    }

    return values;
}

static string GetValueOrDefault(Dictionary<string, string> values, string key)
{
    return values.TryGetValue(key, out string? value) ? value : string.Empty;
}
