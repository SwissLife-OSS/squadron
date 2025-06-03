using System;
using System.Threading.Tasks;
using MySqlConnector;
using Xunit;

namespace Squadron;

/// <inheritdoc/>
public class MySqlResource : MySqlResource<MySqlDefaultOptions>
{
}

/// <summary>
/// Represents a MySql resource that can be used by unit tests.
/// </summary>
/// <seealso cref="IDisposable"/>
public class MySqlResource<TOptions> :
    ContainerResource<TOptions>,
    IAsyncLifetime
    where TOptions : ContainerResourceOptions, new()
{
    /// <summary>
    /// Connection string to access to queue
    /// </summary>
    public string ConnectionString { get; private set; }

    /// <inheritdoc cref="IAsyncLifetime"/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync().ConfigureAwait(false);
        ConnectionString = BuildConnectionString(Settings.Username, Settings.Password);

        await Initializer.WaitAsync(
            new MySqlStatus(ConnectionString));
    }

    private string BuildConnectionString(string username, string password)
    {
        return $"server={Manager.Instance.Address};" +
               $"port={Manager.Instance.HostPort};" +
               $"uid={username};" +
               $"pwd={password};";
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(ConnectionString);
    }

    public MySqlConnection GetConnection(string dbName)
    {
        return new MySqlConnection($"{ConnectionString}database={dbName}");
    }

    /// <summary>
    /// Create database with <see cref="dbName"/> and <see cref="grant"/> permissions to configured user.
    /// Default <see cref="grant"/> to ALL.
    /// </summary>
    public async Task<MySqlConnection> CreateDatabaseAsync(
        string dbName,
        string? grant = default)
    {
        await Manager.InvokeCommandAsync(CreateDbCommand.Execute(dbName, grant, Settings));
        return GetConnection(dbName);
    }

    public async Task RunSqlScriptAsync(string script, string dbName)
    {
        await Manager.InvokeCommandAsync(SqlCommand.Execute(script, dbName, Settings));
    }
}