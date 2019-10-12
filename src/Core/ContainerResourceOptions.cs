using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Squadron
{
    /// <summary>
    /// Abstract base class for container resource options
    /// </summary>
    public abstract class ContainerResourceOptions : IDockerConfigurationProvider
    {
        /// <summary>
        /// Configures the resource
        /// </summary>
        /// <param name="builder">The builder.</param>
        public abstract void Configure(ContainerResourceBuilder builder);

        /// <summary>
        /// Gets the docker configuration from appsettings and environment variables
        /// </summary>
        /// <returns></returns>
        public DockerConfiguration GetDockerConfiguration()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.user.json", true)
                .AddEnvironmentVariables()
                .Build();

            IConfigurationSection section = configuration.GetSection("Squadron:Docker");

            DockerConfiguration containerConfig = section.Get<DockerConfiguration>();
            return containerConfig ?? new DockerConfiguration();
        }
    }
}
