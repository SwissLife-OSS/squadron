using System;
using System.Collections.Generic;
using System.Text;
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
        /// The external address of the container, that is expposed to the host
        /// </summary>
        public ContainerAddress ExternalAddress { get; private set; }

        /// <summary>
        /// The internal address of the container that is exposed into the container network
        /// </summary>
        public ContainerAddress InternalAddress { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            ExternalAddress = new ContainerAddress
            {
                Address = Manager.Instance.Address,
                Port = Manager.Instance.HostPort
            };

            InternalAddress = new ContainerAddress
            {
                Address = Manager.Instance.Name,
                Port = Settings.InternalPort
            };

            await Initializer.WaitAsync(new GenericContainerStatus(ResourceOptions.StatusChecker, ExternalAddress));
        }

        /// <summary>
        /// Gets the external container URI.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Uri GetExternalContainerUri(string scheme = "http")
        {
            return new Uri($"{scheme}://{ExternalAddress.Address}:{ExternalAddress.Port}");
        }

        /// <summary>
        /// Gets the container URI.
        /// </summary>
        /// <param name="scheme">The scheme.</param>
        /// <returns></returns>
        public Uri GetInternalContainerUri(string scheme = "http")
        {
            return new Uri($"{scheme}://{InternalAddress.Address}:{InternalAddress.Port}");
        }

        public Task WaitUntilReadyAsync()
        {
            return Task.CompletedTask;
        }

        public Dictionary<string, string> GetComposeExports()
        {
            return new Dictionary<string, string>()
            {
                { "HTTPURL", GetExternalContainerUri("http").ToString() },
                { "HTTPSURL", GetExternalContainerUri("https").ToString() },
                { "HTTPURL_INTERNAL", GetExternalContainerUri("http").ToString() },
            };
        }
    }
}
