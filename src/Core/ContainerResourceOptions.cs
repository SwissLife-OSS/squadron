using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

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


        public static DockerConfiguration DefaultDockerConfigResolver()
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
