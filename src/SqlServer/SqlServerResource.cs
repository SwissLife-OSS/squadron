using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public partial class SqlServerResource : SqlServerResource<SqlServerDefaultOptions> { }

    /// <summary>
    /// Represents a SqlServer database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public partial class SqlServerResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// Sync lock
        /// </summary>
        protected readonly SemaphoreSlim _sync = new SemaphoreSlim(1,1);
        /// <summary>
        /// The databases
        /// </summary>
        protected readonly HashSet<string> _databases = new HashSet<string>();
        /// <summary>
        /// The SqlServer connection string
        /// </summary>
        protected string _serverConnectionString;

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _serverConnectionString = CreateServerConnectionString();

            await Initializer.WaitAsync(
                new SqlServerStatus(_serverConnectionString));
        }

        /// <summary>
        /// Creates a connection string targeting the given <paramref name="databaseName"/>
        /// </summary>
        /// <param name="databaseName">Database name</param>
        public string CreateConnectionString(string databaseName)
            => CreateDatabaseConnectionString(databaseName);

        /// <summary>
        /// Create a empty database
        /// </summary>
        /// <param name="databaseName">Optional: the database name.</param>
        /// <returns>Database connection string.</returns>
        public Task<string> CreateDatabaseAsync(string databaseName = null)
        {
            var name = databaseName ?? UniqueNameGenerator.Create("db");
            var script = $"CREATE DATABASE {name};";
            return CreateDatabaseAsync(script, name);
        }

        /// <summary>
        /// Create a database from an SQL script
        /// </summary>
        /// <param name="sqlScript">The SQL script content</param>
        /// <param name="databaseName">The database name.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="sqlScript"/> is <c>null</c> or <see cref="string.Empty"/>
        /// or
        /// <paramref name="databaseName"/> is <c>null</c> or <see cref="string.Empty"/>
        /// </exception>
        /// <returns>Database connection string.</returns>
        public Task<string> CreateDatabaseAsync(string sqlScript, string databaseName)
        {
            if (string.IsNullOrEmpty(sqlScript))
            {
                throw new ArgumentException(
                    "The sql script cannot be null or empty.", nameof(sqlScript));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException(
                    "The database name cannot be null or empty.", nameof(databaseName));
            }

            return CreateDatabaseInternalAsync(sqlScript, databaseName);
        }

        private async Task<string> CreateDatabaseInternalAsync(string sqlScript, string databaseName)
        {
            await _sync.WaitAsync();
            try
            {
                FileInfo scriptFile = CreateSqlFile(sqlScript);
                var copyContext = new CopyContext(scriptFile.FullName, $"/tmp/{scriptFile.Name}");

                await Manager.CopyToContainerAsync(copyContext);

                await Manager.InvokeCommandAsync(
                    ChmodCommand.ReadWrite($"/tmp/{scriptFile.Name}"));

                await Manager.InvokeCommandAsync(
                    SqlCommand.ExecuteFile(copyContext.Destination, Settings));

                _databases.Add(databaseName);

                return CreateDatabaseConnectionString(databaseName);
            }
            finally
            {
                _sync.Release();
            }
        }

        /// <summary>
        /// Executes the specified <paramref name="sql"/> on the currently running server.
        /// </summary>
        /// <param name="sql">
        /// The SQL that shall be executed on the currently running server.
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="sql"/> is <c>null</c> or <see cref="string.Empty"/>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// There is currently no SQL Server running.
        /// </exception>
        public Task ExecuteSqlAsync(string sql)
        {
            if (string.IsNullOrEmpty(sql))
            {
                throw new ArgumentException("The sql script cannot be null or empty.", nameof(sql));
            }

            if (string.IsNullOrEmpty(_serverConnectionString))
            {
                throw new InvalidOperationException("There is currently no databse deployed.");
            }

            return ExecuteSqlInternalAsync(sql);
        }

        private async Task ExecuteSqlInternalAsync(string sql)
        {
            await _sync.WaitAsync();
            try
            {
                await Manager.InvokeCommandAsync(
                    SqlCommand.ExecuteQuery(sql, Settings));
            }
            finally
            {
                _sync.Release();
            }
        }

        /// <summary>
        /// Creates the database connection string.
        /// </summary>
        /// <param name="databaseName">Name of the database.</param>
        /// <returns></returns>
        protected string CreateDatabaseConnectionString(string databaseName)
            => $"{_serverConnectionString}Database={databaseName}";

        private string CreateServerConnectionString()
            => new StringBuilder()
                .Append($"Data Source={Manager.Instance.Address},{Manager.Instance.HostPort};")
                .Append("Integrated Security=False;")
                .Append($"User ID={Settings.Username};")
                .Append($"Password={Settings.Password};")
                .Append("MultipleActiveResultSets=True;")
                .Append("TrustServerCertificate=True;")
                .ToString();

        internal async Task DeployAndExecute(string sqlScript)
        {
            FileInfo scriptFile = CreateSqlFile(sqlScript);
            var copyContext = new CopyContext(scriptFile.FullName, $"/tmp/{scriptFile.Name}");

            await Manager.CopyToContainerAsync(copyContext);
            await Manager.InvokeCommandAsync(
                SqlCommand.ExecuteFile(copyContext.Destination, Settings));

            File.Delete(scriptFile.FullName);
        }

        private FileInfo CreateSqlFile(string content)
        {
            var scriptFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sql");
            File.WriteAllText(scriptFile, content);
            return new FileInfo(scriptFile);
        }
    }
}
