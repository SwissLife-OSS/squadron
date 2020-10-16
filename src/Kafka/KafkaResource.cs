using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class KafkaResource : KafkaResource<KafkaDefaultOptions>
    {

    }


    /// <summary>
    /// Represents a Kafka resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class KafkaResource<TOptions>
        : ContainerResource<TOptions>,
            IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var name = Manager.Instance.Name;
            var address = Manager.Instance.Address;

            await Initializer.WaitAsync(new KafkaStatusChecker());
        }
    }
}
