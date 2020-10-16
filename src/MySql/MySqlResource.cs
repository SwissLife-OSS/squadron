using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
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
            await base.InitializeAsync();
            ConnectionString = BuildConnectionString(Settings.Username);

            await Initializer.WaitAsync(
                new MySqlStatus(ConnectionString));
        }

        private string BuildConnectionString(string database)
        {
            return $"server={Manager.Instance.Address};port={Manager.Instance.HostPort};" +
                $"uid=root;pwd=MyPassword;" +
                $"ConnectionTimeout=600;DefaultCommandTimeout=600;SslMode=None";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public MySqlConnection GetConnection(string dbName)
        {
            throw new NotImplementedException();
        }

        public async Task CreateDatabaseAsync(string dbName)
        {
            await Manager.InvokeCommandAsync(CreateDbCommand.Execute(dbName, Settings));
        }
    }
}
