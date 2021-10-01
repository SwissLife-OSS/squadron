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
        /// Internal port that value will be resolved from variable
        /// </summary>
        public string InternalPortVariableName { get; internal set; }

        /// <summary>
        /// Gets the external port (Static).
        /// </summary>
        /// <value>
        /// The external port.
        /// </value>
        public int ExternalPort { get; internal set; }

        /// <summary>
        /// External port that value will be resolved from variable
        /// </summary>
        public string ExternalPortVariableName { get; internal set; }
    }
}
