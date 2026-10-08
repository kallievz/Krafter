IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

string solutionRoot = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", ".."));
string migratorProject = Path.Combine(solutionRoot, "src", "AditiKraft.Krafter.Backend.Migrator",
    "AditiKraft.Krafter.Backend.Migrator.csproj");

IResourceBuilder<IResourceWithConnectionString> database = builder.AddConnectionString("appDb");

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

builder.AddProject<Projects.AditiKraft_Krafter_UI_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(backend)
    .WithReference(database)
    .WaitFor(backend);

builder.Build().Run();
