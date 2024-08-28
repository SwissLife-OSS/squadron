using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

            DockerConfiguration containerConfig = section.Get<DockerConfiguration>() ?? new DockerConfiguration();

            AddLocalDockerAuthentication(containerConfig);

            return containerConfig;
        }

        private static void AddLocalDockerAuthentication(DockerConfiguration containerConfig)
        {
            var dockerAuthRootObject = TryGetDockerAuthRootObject();

            if (dockerAuthRootObject != null)
            {
                foreach (var auth in dockerAuthRootObject.Auths)
                {
                    var address = new Uri(auth.Key);

                    if (containerConfig.Registries.Any(p =>
                            p.Address.Equals(address.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        continue;
                    }

                    containerConfig.Registries.Add(new DockerRegistryConfiguration
                    {
                        Name = address.Host,
                        Address = address.ToString(),
                        Auth = auth.Value.Auth,
                        Email = auth.Value.Email
                    });
                }
            }
        }

        private static DockerAuthRootObject? TryGetDockerAuthRootObject()
        {
            var dockerConfigPath = Environment.GetEnvironmentVariable("DOCKER_CONFIG");
            if (string.IsNullOrEmpty(dockerConfigPath))
            {
                return null;
            }

            // Construct the full path to the config.json file
            var configFilePath = Path.Combine(dockerConfigPath, "config.json");

            if (!File.Exists(configFilePath))
            {
                return null;
            }

            try
            {
                var jsonString = File.ReadAllText(configFilePath);

                return JsonSerializer.Deserialize<DockerAuthRootObject>(jsonString);
            }
            catch { }

            return null;
        }
    }
}
