using Microsoft.Extensions.Configuration;

namespace Squadron;

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

    /// <summary>
    /// Default resolver for Docker configuration.
    /// Testcontainers handles registry authentication internally via Docker's
    /// credential helpers, credential store, and config file.
    /// </summary>
    public static DockerConfiguration DefaultDockerConfigResolver()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile("appsettings.user.json", true)
            .AddEnvironmentVariables()
            .Build();

        IConfigurationSection section = configuration.GetSection("Squadron:Docker");

        DockerConfiguration containerConfig = section.Get<DockerConfiguration>() ?? new DockerConfiguration();

        return containerConfig;
    }
}