using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class ZookeeperResource : ZookeeperResource<KafkaDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a Kafka resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class ZookeeperResource<TOptions>
        : ContainerResource<TOptions>,
            IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
        }
    }
}
