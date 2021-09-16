namespace Squadron
{
    /// <summary>
    /// Describes the port mapping of a container
    /// </summary>
    public class ContainerPortMapping
    {
        /// <summary>
        /// Port of application inside container
        /// </summary>
        public int InternalPort { get; internal set; }

        /// <summary>
        /// Gets the external port (Static).
        /// </summary>
        /// <value>
        /// The external port.
        /// </value>
        public int ExternalPort { get; internal set; }
    }
}
