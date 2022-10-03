using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class NatsResource : NatsResource<NatsDefaultOptions> { }

    /// <summary>
    /// Represents a NATS resource that can be used by unit tests.
    /// </summary>
    public class NatsResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime,
          IComposableResource
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// NATS Client ConnectionString, in the format of "nats://localhost:port"
        /// </summary>
        public string NatsConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var baseAddressClient =
                $"{Manager.Instance.Address}:{Manager.Instance.HostPort}";
            var baseAddressMonitoring =
                $"{Manager.Instance.Address}:{Manager.Instance.AdditionalPorts[0].ExternalPort}";
            NatsConnectionString = $"nats://{baseAddressClient}";

            await Initializer.WaitAsync(new NatsStatus(baseAddressMonitoring));
        }
    }
}
