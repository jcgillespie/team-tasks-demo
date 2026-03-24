using Microsoft.Extensions.Configuration;
using TeamTasks.Api.Data;

namespace TeamTasks.Api.Tests;

public class HostedConfigurationTests
{
    [Fact]
    public void DbProviderMode_PrefersSqliteForLocal()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "sqlite"
            })
            .Build();

        var mode = HostedConfiguration.ResolveProviderMode(configuration, "Development");

        Assert.Equal(DatabaseProviderMode.Sqlite, mode);
    }

    [Fact]
    public void DbProviderMode_UsesSqlServerForHosted()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "sqlserver",
                ["ConnectionStrings:DefaultConnection"] = "Server=tcp:example.database.windows.net;Database=teamtasks;"
            })
            .Build();

        var mode = HostedConfiguration.ResolveProviderMode(configuration, "Production");

        Assert.Equal(DatabaseProviderMode.SqlServer, mode);
    }

    [Fact]
    public void RequiresHostedConfiguration_WhenSqlServerWithoutConnectionString()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "sqlserver"
            })
            .Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            HostedConfiguration.ResolveConnectionString(configuration, DatabaseProviderMode.SqlServer));

        Assert.Contains("ConnectionStrings:DefaultConnection", exception.Message);
    }

    [Fact]
    public void ResolveConnectionString_UsesKeyVaultStyleReferenceWhenPresent()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "sqlserver",
                ["ConnectionStrings:DefaultConnection"] = "@Microsoft.KeyVault(SecretUri=https://kv.vault.azure.net/secrets/sql/)",
            })
            .Build();

        var value = HostedConfiguration.ResolveConnectionString(configuration, DatabaseProviderMode.SqlServer);

        Assert.StartsWith("@Microsoft.KeyVault", value);
    }
}
