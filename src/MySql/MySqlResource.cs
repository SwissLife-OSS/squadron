using System;
using System.Threading.Tasks;
using MySqlConnector;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class MySqlResource : MySqlResource<MySqlDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a MySql resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MySqlResource<TOptions>
        : ContainerResource<TOptions>,
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

        public async Task CreateDatabaseAsync(string dbName)
        {
            await Manager.InvokeCommandAsync(CreateDbCommand.Execute(dbName, Settings));
        }

        public async Task RunSqlScriptAsync(string script, string dbName)
        {
            await Manager.InvokeCommandAsync(SqlCommand.Execute(script, dbName, Settings));
        }
    }
}
