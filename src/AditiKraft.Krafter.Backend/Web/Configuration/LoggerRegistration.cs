using Microsoft.Data.SqlClient;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Reflection;

namespace AditiKraft.Krafter.Backend.Web.Configuration;

public static class LoggerRegistration
{
    public static async Task EnsureSerilogLogTableAsync(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("appDb")
            ?? throw new InvalidOperationException("Database connection string 'appDb' not found");

        const string sql = """
            IF OBJECT_ID(N'[dbo].[AppLogs]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[AppLogs]
                (
                    [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_AppLogs] PRIMARY KEY,
                    [Message] nvarchar(max) NULL,
                    [MessageTemplate] nvarchar(max) NULL,
                    [Level] nvarchar(128) NULL,
                    [TimeStamp] datetime2(7) NOT NULL,
                    [Exception] nvarchar(max) NULL,
                    [LogEvent] nvarchar(max) NULL
                );
            END
            """;

        await using SqlConnection connection = new SqlConnection(connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        _ = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public static Serilog.ILogger CreateLogger(IConfiguration configuration, params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string applicationAssemblies = GetAssemblyNames(assemblies);

        ColumnOptions columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Add(StandardColumn.LogEvent);
        columnOptions.LogEvent.DataLength = int.MaxValue;

        MSSqlServerSinkOptions sinkOptions = new MSSqlServerSinkOptions
        {
            TableName = "AppLogs",
            SchemaName = "dbo",
            AutoCreateSqlTable = false
        };

        string logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
        string logFilePath = Path.Combine(logDirectory, "backend-.log");

        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProcessId()
            .Enrich.WithProcessName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithThreadName()
            .Enrich.WithProperty("Application", "AditiKraft.Krafter.Backend")
            .Enrich.WithProperty("ApplicationAssemblies", applicationAssemblies)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
            .MinimumLevel.Override("TickerQ", LogEventLevel.Information)
            .MinimumLevel.Override("TickerQ.EntityFrameworkCore", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console()
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 14,
                shared: true,
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}{Properties}{NewLine}")
            .WriteTo.MSSqlServer(
                connectionString: configuration.GetConnectionString("appDb"),
                sinkOptions: sinkOptions,
                restrictedToMinimumLevel: LogEventLevel.Information,
                columnOptions: columnOptions)
            .CreateLogger();
    }

    private static string GetAssemblyNames(IEnumerable<Assembly> assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);

        string[] assemblyNames = assemblies
            .Where(assembly => assembly is not null)
            .Select(assembly => assembly.GetName().Name)
            .Where(assemblyName => !string.IsNullOrWhiteSpace(assemblyName))
            .Select(assemblyName => assemblyName!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(assemblyName => assemblyName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return assemblyNames.Length == 0 ? "Unknown" : string.Join(", ", assemblyNames);
    }
}
