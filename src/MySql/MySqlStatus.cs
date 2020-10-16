using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new MySqlCommand("select version()", conn))
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
