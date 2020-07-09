using System;
using System.Collections.Generic;

namespace Squadron
{
    /// <summary>
    /// Builder for container settings
    /// </summary>
    public class ContainerResourceBuilder
    {
        protected readonly ContainerResourceSettings _options = new ContainerResourceSettings();

        private readonly List<string> _cmd = new List<string>();

        /// <summary>
        /// Craeates an new empty builder
        /// </summary>
        /// <returns></returns>
        public static ContainerResourceBuilder New() => new ContainerResourceBuilder();


        /// <summary>
        /// Container name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Name(string name)
        {
            _options.Name = name;
            return this;
        }


        /// <summary>
        /// Container image
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Image(string image)
        {
            var parts = image.Split(':');
            if (parts.Length > 1)
            {
                _options.Image = parts[0];
                _options.Tag = parts[1];
            }
            else
                _options.Image = image;
            return this;
        }

        /// <summary>
        /// Container registry name as defined in configurations
        /// Default is DockerHub
        /// </summary>
        /// <param name="registryName">Name of the registry.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Registry(string registryName)
        {
            _options.RegistryName = registryName;
            return this;
        }

        public ContainerResourceBuilder AddressMode(ContainerAddressMode mode)
        {
            _options.AddressMode = mode;
            return this;
        }

        /// <summary>
        /// Adds an environment variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        public ContainerResourceBuilder AddEnvironmentVariable(string variable)
        {
            _options.EnvironmentVariables.Add(variable);
            return this;
        }


        /// <summary>
        /// Container image tag
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Tag(string tag)
        {
            _options.Tag = tag;
            return this;
        }

        public ContainerResourceBuilder AddCmd(params string[] cmd)
        {
            _cmd.AddRange(cmd);
            return this;
        }

        /// <summary>
        /// Username
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Username(string username)
        {
            _options.Username = username;
            return this;
        }

        /// <summary>
        /// Passwords
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public ContainerResourceBuilder Password(string password)
        {
            _options.Password = password;
            return this;
        }

        /// <summary>
        /// Internals port.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public ContainerResourceBuilder InternalPort(int port)
        {
            _options.InternalPort = port;
            return this;
        }

        /// <summary>
        /// Static external port
        /// Use this setting only when a static port is required
        /// When the port is allready in use container creation will fail
        /// </summary>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public ContainerResourceBuilder ExternalPort(int port)
        {
            _options.ExternalPort = port;
            return this;
        }

        /// <summary>
        /// Wait timeout in seconds
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        public ContainerResourceBuilder WaitTimeout(int seconds)
        {
            _options.WaitTimeout = TimeSpan.FromSeconds(seconds);
            return this;
        }

        /// <summary>
        /// Sets the docker configuration resolver.
        /// </summary>
        /// <param name="resolver">The resolver.</param>
        /// <returns></returns>
        public ContainerResourceBuilder SetDockerConfigResolver(
            Func<DockerConfiguration> resolver)
        {
            _options.DockerConfigResolver = resolver;
            return this;
        }

        /// <summary>
        /// Adds a network of which the container should be part of.
        /// </summary>
        /// <param name="network">The network name.</param>
        /// <returns></returns>
        public ContainerResourceBuilder AddNetwork(
            string network)
        {
            _options.Networks.Add(network);
            return this;
        }

        /// <summary>
        /// Builds the settings
        /// </summary>
        /// <returns></returns>
        public virtual ContainerResourceSettings Build()
        {
            if (_options.DockerConfigResolver == null)
                _options.DockerConfigResolver =
                    ContainerResourceOptions.DefaultDockerConfigResolver;
            _options.Cmd = _cmd;
            return _options;
        }
    }
}
