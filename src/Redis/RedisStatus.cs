using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Squadron
{
    /// <summary>
    /// Status checker for Redis
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class RedisStatus : IResourceStatusProvider
    {
        private readonly string _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisStatus"/> class.
        /// </summary>
        /// <param name="host">Hostname</param>
        public RedisStatus(string host)
        {
            _host = host;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            ConnectionMultiplexer redis = null;
            try
            {
                redis =
                    await ConnectionMultiplexer.ConnectAsync(_host);

                await redis.GetDatabase()
                                .PingAsync();

                return new Status
                {
                    IsReady = true,
                    Message = redis.GetStatus()
                };

            }
            catch (Exception ex)
            {
                return new Status
                {
                    IsReady = false,
                    Message = ex.Message
                };
            }
            finally
            {
                redis.Dispose();
            }
        }

    }
}
