using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a SqlServer database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public partial class SqlServerResource
        : ResourceBase<SqlServerImageSettings>, IAsyncLifetime
    {
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1,1);
        private readonly HashSet<string> _databases = new HashSet<string>();
        private string _serverConnectionString;

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            _serverConnectionString = CreateServerConnectionString(Settings);

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
        /// Crate a database from an SQL script
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
                throw new ArgumentException("The sql script cannot be null or empty.", nameof(sqlScript));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("The database name cannot be null or empty.", nameof(databaseName));
            }

            return CreateDatabaseInternalAsync(sqlScript, databaseName);
        }

        private async Task<string> CreateDatabaseInternalAsync(string sqlScript, string databaseName)
        {
            await _sync.WaitAsync();
            try
            {
                await SqlScript.DeployAndExecute(sqlScript, Settings);
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
                await Container.InvokeCommand(SqlCommand.ExecuteQuery(sql, Settings), Settings);
            }
            finally
            {
                _sync.Release();
            }
        }

        private string CreateDatabaseConnectionString(string databaseName)
            => $"{_serverConnectionString}Database={databaseName}";

        private string CreateServerConnectionString(IImageSettings settings)
            => new StringBuilder()
                .Append($"Data Source={settings.ContainerIp},{settings.DefaultPort};")
                .Append("Integrated Security=False;")
                .Append($"User ID={settings.Username};")
                .Append($"Password={settings.Password};")
                .Append($"MultipleActiveResultSets=True;")
                .ToString();

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
