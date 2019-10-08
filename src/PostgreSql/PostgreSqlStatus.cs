using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Squadron
{
    /// <summary>
    /// Status checker for PostgreSQL
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class PostgreSqlStatus : IResourceStatusProvider
    {
        private readonly string _connectionString;


        /// <summary>
        /// Initializes a new instance of the <see cref="PostgreSqlStatus" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PostgreSqlStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("select version()", conn))
                {
                    object version = await cmd.ExecuteScalarAsync();

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
