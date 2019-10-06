using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class RedisResource : RedisResource<RedisDefaultOptions> { }
    
    /// <summary>
    /// Represents a redis resource that can be used by unit tests.
    /// </summary>
    public class RedisResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the Redix connection
        /// </summary>
        /// <returns></returns>
        public ConnectionMultiplexer GetConnection()
        {
            return ConnectionMultiplexer
                .Connect(ConnectionString);
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = $"{Manager.Instance.Address}:{Manager.Instance.HostPort}";
            await Initializer.WaitAsync(new RedisStatus(ConnectionString));
        }
    }
}
