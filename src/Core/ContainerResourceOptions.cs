namespace Squadron
{

    /// <summary>
    /// Abstract base class for container resource options
    /// </summary>
    public abstract class ContainerResourceOptions
    {
        /// <summary>
        /// Configures the resource
        /// </summary>
        /// <param name="builder">The builder.</param>
        public abstract void Configure(ContainerResourceBuilder builder);
    }
}
