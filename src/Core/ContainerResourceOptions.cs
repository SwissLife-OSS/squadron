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
                    if(!Uri.TryCreate(auth.Key, UriKind.RelativeOrAbsolute, out Uri address))
                    {
                        continue;
                    }

                    if (containerConfig.Registries.Any(p =>
                            p.Address.Equals(address.ToString(), StringComparison.InvariantCultureIgnoreCase)) ||
                        string.IsNullOrEmpty(auth.Value.Email) ||
                        string.IsNullOrEmpty(auth.Value.Auth))
                    {
                        continue;
                    }
                    
                    var decryptedToken = Convert.FromBase64String(auth.Value.Auth);
                    var token = System.Text.Encoding.UTF8.GetString(decryptedToken);
                    var parts = token.Split(':');
                    
                    if (parts.Length != 2)
                    {
                        continue;
                    }

                    containerConfig.Registries.Add(new DockerRegistryConfiguration
                    {
                        Name = address.Host,
                        Address = address.ToString(),
                        Username = parts[0],
                        Password = parts[1]
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
