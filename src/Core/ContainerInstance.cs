using System.Collections.Generic;

namespace Squadron
{
    /// <summary>Respresents a created docker container</summary>
    public class ContainerInstance
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the ip address.
        /// </summary>
        /// <value>
        /// The ip address.
        /// </value>
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the host port.
        /// </summary>
        /// <value>
        /// The host port.
        /// </value>
        public int HostPort { get; set; }

        /// <summary>
        /// A list of additional ports, that were exposed in addition to the main port
        /// </summary>
        public IList<ContainerPortMapping> AdditionalPorts { get; } =
            new List<ContainerPortMapping>();

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; internal set; }

        /// <summary>
        /// Gets or sets the container logs.
        /// </summary>
        /// <value>
        /// The logs.
        /// </value>
        public IList<string> Logs { get; set; } = new List<string>();
    }
}
