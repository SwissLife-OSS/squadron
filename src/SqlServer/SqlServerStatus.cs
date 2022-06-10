using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Squadron
{
    /// <summary>
    /// Status checker for SQLServer
    /// </summary>
    /// <seealso cref="Squadron.IResourceStatusProvider" />
    public class SqlServerStatus : IResourceStatusProvider
    {
        private readonly string _serverConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerStatus"/> class.
        /// </summary>
        public SqlServerStatus(string serverConnectionString)
        {
            _serverConnectionString = serverConnectionString;
        }

        /// <summary>
        /// Determines whether SQLServer is ready.
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_serverConnectionString))
            {
                await connection.OpenAsync(cancellationToken);

                using (Microsoft.Data.SqlClient.SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Count(name) FROM sys.databases";
                    return new Status
                    {
                        IsReady = (int)await command.ExecuteScalarAsync(cancellationToken) >= 4
                    };
                }
            }
        }
    }
}
