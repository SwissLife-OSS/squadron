using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class ClickHouseResource : ClickHouseResource<ClickHouseDefaultOptions>
    {
    }

    /// <summary>
    /// Represents a ClickHouse database that can be used by unit tests.
    /// </summary>
    public class ClickHouseResource<TOptions>
        : ContainerResource<TOptions>, IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// The url of the ClickHouse server
        /// </summary>
        public string Url =>
            $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";

        /// <summary>
        /// Connection string to access to database
        /// </summary>
        public string ConnectionString { get; private set; } = null!;

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = BuildConnectionString(Settings.Username);
            await Initializer.WaitAsync(new ClickHouseStatus(Url, Settings.Username, Settings.Password));
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
        /// Creates an new database with the given name
        /// </summary>
        /// <param name="dbName">Database name</param>
        /// <returns></returns>
        public async Task CreateDatabaseAsync(string dbName)
        {
            await SendCommand($"CREATE DATABASE {dbName};");
        }

        /// <summary>
        /// Runs a PqlScript on the database
        /// </summary>
        /// <param name="sqlScript"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public async Task RunSqlScriptAsync(string sqlScript, string? dbName = null)
        {
            HttpResponseMessage result = await SendCommand(sqlScript, dbName);
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new Exception($"ClickHouse command failed: {content}");
            }
        }

        public async Task<HttpResponseMessage> SendCommand(
            string command,
            string? dbName = null,
            CancellationToken cancellationToken = default)
        {
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            var content = new StringContent(command);

            if (dbName != null)
            {
                content.Headers.Add("X-ClickHouse-Database", dbName);
            }

            // Add authentication headers
            if (!string.IsNullOrEmpty(Settings.Username))
            {
                content.Headers.Add("X-ClickHouse-User", Settings.Username);
                if (!string.IsNullOrEmpty(Settings.Password))
                {
                    content.Headers.Add("X-ClickHouse-Key", Settings.Password);
                }
            }

            return await httpClient.PostAsync(Url, content, cancellationToken);
        }
    }
}
