namespace Squadron
{
    /// <summary>
    /// Defines an Inteface to provide docker configurations
    /// </summary>
    public interface IDockerConfigurationProvider
    {
        /// <summary>
        /// Gets the docker configuration.
        /// </summary>
        /// <returns></returns>
        DockerConfiguration GetDockerConfiguration();
    }
}
