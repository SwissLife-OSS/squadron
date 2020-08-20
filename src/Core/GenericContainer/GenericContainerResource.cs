using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Defines a Generic container resource that can be used in a unit test
    /// </summary>
    /// <typeparam name="TOptions">The type of the options.</typeparam>
    /// <seealso cref="Squadron.ContainerResource{TOptions}" />
    /// <seealso cref="Xunit.IAsyncLifetime" />
    public class GenericContainerResource<TOptions>
          : ContainerResource<TOptions>,
            IAsyncLifetime,
            IComposableResource
          where TOptions : GenericContainerOptions, new()
    {

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public ContainerAddress Address { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            Address = new ContainerAddress
            {
                Address = Manager.Instance.Address,
                Port = Manager.Instance.HostPort
            };

            await Initializer.WaitAsync(new GenericContainerStatus(
                ResourceOptions.StatusChecker, Address));
        }

        /// <summary>
        /// Gets the external container URI.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Uri GetContainerUri(string scheme = "http")
        {
            return new Uri($"{scheme}://{Address.Address}:{Address.Port}");
        }

        public Task WaitUntilReadyAsync()
        {
            return Task.CompletedTask;
        }

        public Dictionary<string, string> GetComposeExports()
        {
            var internalUri = new Uri($"http://{Manager.Instance.Name}:{Settings.InternalPort}");

            return new Dictionary<string, string>()
            {
                { "HTTPURL", GetContainerUri("http").ToString().Trim('/') },
                { "HTTPSURL", GetContainerUri("https").ToString().Trim('/') },
                { "HTTPURL_INTERNAL", internalUri.ToString().Trim('/') }
            };
        }
    }
}
