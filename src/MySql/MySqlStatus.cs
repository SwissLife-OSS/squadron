using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;

namespace Squadron
{
    /// <summary>
    /// Status checker for MySql
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class MySqlStatus : IResourceStatusProvider
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlStatus" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MySqlStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);

                using (var cmd = new MySqlCommand("select version()", connection))
                {
                    object version = await cmd.ExecuteScalarAsync(cancellationToken);

                    return new Status
                    {
                        IsReady = true,
                        Message = version.ToString()
                    };
                }
            }
        }
    }
}
