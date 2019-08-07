#if NET46
using System;
using System.IO;
using Microsoft.SqlServer.Dac;

namespace Squadron
{
    public partial class SqlServerResource
    {
        /// <summary>
        /// Clone an existing database schema and deploy.
        /// </summary>
        /// <param name="sourceDatabase">Source database connection string.</param>
        /// <param name="databaseName">Database name.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="sourceDatabase"/> is <c>null</c> or <see cref="string.Empty"/>
        /// or
        /// <paramref name="databaseName"/> is <c>null</c> or <see cref="string.Empty"/>
        /// </exception>
        /// <returns>Database connection string.</returns>
        public string CloneDatabase(string sourceDatabase, string databaseName)
        {
            if (string.IsNullOrEmpty(sourceDatabase))
            {
                throw new ArgumentException("The source database cannot be null or empty.", nameof(sourceDatabase));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("The database name cannot be null or empty.", nameof(databaseName));
            }

            lock (_sync)
            {
                using (var stream = new MemoryStream())
                {
                    var extractService = new DacServices(sourceDatabase);
                    extractService.Extract(stream, databaseName, "SqlServer.Test",
                        Version.Parse("0.0.1"), "SqlServer.Test", null, DacpacOptions.Extract);

                    DacPackage.Deploy(stream, _serverConnectionString, databaseName, Settings);
                }

                _databases.Add(databaseName);

                return CreateDatabaseConnectionString(databaseName);
            }
        }

        /// <summary>
        /// Deploy an existing dacpac file.
        /// </summary>
        /// <param name="dacpacFile">The dacpac file path.</param>
        /// <param name="databaseName">The database name.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="dacpacFile"/> is <c>null</c> or <see cref="string.Empty"/>
        /// or
        /// <paramref name="databaseName"/> is <c>null</c> or <see cref="string.Empty"/>
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// <paramref name="dacpacFile"/> does not exist.
        /// </exception>
        /// <returns>Database connection string.</returns>
        public string DeployDatabase(string dacpacFile, string databaseName)
        {
            if (string.IsNullOrEmpty(dacpacFile))
            {
                throw new ArgumentException("The sql script cannot be null or empty.", nameof(dacpacFile));
            }

            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("The database name cannot be null or empty.", nameof(databaseName));
            }

            if (!File.Exists(dacpacFile))
            {
                throw new FileNotFoundException("The dacpac file does not exist.", dacpacFile);
            }

            lock (_sync)
            {
                using (FileStream stream = File.OpenRead(dacpacFile))
                {
                    DacPackage.Deploy(stream, _serverConnectionString, databaseName, Settings);
                }

                _databases.Add(databaseName);

                return CreateDatabaseConnectionString(databaseName);
            }
        }
    }
}
#endif
