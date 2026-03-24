using Microsoft.Extensions.Configuration;

namespace TeamTasks.Api.Data;

public static class HostedConfiguration
{
    private const string SqliteProvider = "sqlite";
    private const string SqlServerProvider = "sqlserver";

    public static DatabaseProviderMode ResolveProviderMode(IConfiguration configuration, string environmentName)
    {
        var configuredProvider = configuration["Database:Provider"]?.Trim().ToLowerInvariant();

        if (configuredProvider == SqlServerProvider)
        {
            return DatabaseProviderMode.SqlServer;
        }

        if (configuredProvider == SqliteProvider)
        {
            return DatabaseProviderMode.Sqlite;
        }

        return string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase)
            ? DatabaseProviderMode.Sqlite
            : DatabaseProviderMode.SqlServer;
    }

    public static string ResolveConnectionString(IConfiguration configuration, DatabaseProviderMode providerMode)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString;
        }

        if (providerMode == DatabaseProviderMode.SqlServer)
        {
            throw new InvalidOperationException("Missing required configuration key: ConnectionStrings:DefaultConnection");
        }

        return "Data Source=teamtasks.db";
    }
}
