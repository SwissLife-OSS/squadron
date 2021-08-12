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
          IAsyncLifetime,
          IComposableResource
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// Gets the Redis connection
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

        public override Dictionary<string, string> GetComposeExports()
        {
            var internalConnectionString =
                $"{Manager.Instance.Name}:{Settings.InternalPort},allowAdmin=true";

            Dictionary<string, string> exports = base.GetComposeExports();
            exports.Add("CONNECTIONSTRING", ConnectionString);
            exports.Add("CONNECTIONSTRING_INTERNAL", internalConnectionString);

            return exports;
        }
    }
}
