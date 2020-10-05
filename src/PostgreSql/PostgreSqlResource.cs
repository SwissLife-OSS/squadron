using System.Threading.Tasks;
using Npgsql;

namespace Squadron
{
    /// <inheritdoc/>
    public class PostgreSqlResource : PostgreSqlResource<PostgreSqlDefaultOptions> { }

    /// <summary>
    /// Represents a PostgreSQL database that can be used by unit tests.
    /// </summary>
    public class PostgreSqlResource<TOptions>
        : ContainerResource<TOptions>, ISquadronAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {

        /// <summary>
        /// Connection string to access to database
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc/>
        protected override void OnSettingsBuilded(ContainerResourceSettings settings)
        {
            settings.EnvironmentVariables.Add($"POSTGRES_USER={settings.Username}");
            settings.EnvironmentVariables.Add($"POSTGRES_PASSWORD={settings.Password}");
            //This is required to run psql commands
            settings.EnvironmentVariables.Add($"PGPASSWORD=={settings.Password}");
        }


        /// <inheritdoc cref="ISquadronAsyncLifetime"/>
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = BuildConnectionString(Settings.Username);
            await Initializer.WaitAsync(new PostgreSqlStatus(ConnectionString));
        }

        private string BuildConnectionString(string database)
        {
            return $"Host={Manager.Instance.Address};Port={Manager.Instance.HostPort};" +
                   $"Username={Settings.Username};Password={Settings.Password};" +
                   $"Database={database}";
        }

        /// <summary>
        /// Gets the connection string for a give database.
        /// </summary>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public string GetConnectionString(string database)
        {
            return BuildConnectionString(database);
        }


        /// <summary>
        /// Get an Connection for the default database
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(ConnectionString);
        }
        

        /// <summary>
        /// Gets a Connection for the given database
        /// </summary>
        /// <param name="dbName">Database name</param>
        /// <returns></returns>
        public NpgsqlConnection GetConnection(string dbName)
        {
            return new NpgsqlConnection(BuildConnectionString(dbName));
        }

        /// <summary>
        /// Creates an new database with the given name
        /// </summary>
        /// <param name="dbName">Database name</param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(string dbName)
        {
            await Manager.InvokeCommandAsync(CreateDbCommand.Execute(dbName, Settings));
        }

        /// <summary>
        /// Runs a PqlScript on the database
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public async Task RunSqlScriptAsync(string sqlScript, string dbName)
        {
            var copyContext = CopyContext.CreateFromFileContent(sqlScript, "sql", "tmp");
            await Manager.CopyToContainerAsync(copyContext);
            await Manager.InvokeCommandAsync(
                PSqlCommand.ExecuteFile(copyContext.Destination, dbName, Settings));
        }
    }
}
