using System.Collections.Generic;

namespace Squadron
{
    /// <summary>
    /// Docker configuration
    /// </summary>
    public class DockerConfiguration
    {
        /// <summary>
        /// Gets or sets the registries.
        /// </summary>
        /// <value>
        /// The registries.
        /// </value>
        public IEnumerable<DockerRegistryConfiguration> Registries { get; set; }

        public ContainerAddressMode DefaultAddressMode { get; internal set; } = ContainerAddressMode.Port;
    }

    public enum ContainerAddressMode
    {
        Auto,
        IpAddress,
        Port
    }
}
