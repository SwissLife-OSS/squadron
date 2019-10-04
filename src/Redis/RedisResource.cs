using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a Redis resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class RedisResource
        : ResourceBase<RedisImageSettings>, IAsyncLifetime
    {
        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task InitializeAsync()
        {
            await StartContainerAsync();
            ConnectionString = $"{Settings.ContainerAddress}:{Settings.HostPort}";

            await Initializer.WaitAsync(
                new RedisStatus(ConnectionString), Settings);
        }

        /// <summary>
        /// Gets the Redix connection
        /// </summary>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnection()
        {
            return ConnectionMultiplexer
                .Connect(ConnectionString);
        }

        /// <summary>
        /// Gets the Redix connection async
        /// </summary>
        /// <returns></returns>
        public async Task<ConnectionMultiplexer> GetConnectionAsync()
        {
            return await ConnectionMultiplexer
                        .ConnectAsync(ConnectionString);
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
