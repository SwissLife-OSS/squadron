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
            _options.UniqueContainerName = UniqueNameGenerator.CreateContainerName(name);
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

        public ContainerResourceBuilder AddVolume(string mapping)
        {
            _options.Volumes.Add(mapping);

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
        /// Sets the main internal port of this container to the given value.
        ///
        /// If you want to expose multiple ports, you can use the <see cref="AddPortMapping"/>
        /// method, to register additional port mappings.
        /// </summary>
        /// <param name="port">The internal port of a container that shall be exposed. </param>
        public ContainerResourceBuilder InternalPort(int port)
        {
            _options.InternalPort = port;
            return this;
        }

        /// <summary>
        /// Sets the main external port of this container to the given value.
        /// Use this setting only if a static external port is required.
        /// If the given port is already in use by a container, the creation will fail.
        ///
        /// If you want to expose multiple ports, you can use the <see cref="AddPortMapping"/>
        /// method, to register additional port mappings
        /// </summary>
        /// <param name="port">
        /// The main external static port, to which the main internal port
        /// of the container will be mapped.
        /// </param>
        public ContainerResourceBuilder ExternalPort(int port)
        {
            _options.ExternalPort = port;
            return this;
        }


        /// <summary>
        /// If you only want to expose one port, please use <see cref="InternalPort"/> and
        /// <see cref="ExternalPort"/> to so do!
        /// Exposes additional port mappings for this container.
        /// </summary>
        /// <param name="internalPort">
        /// The internal port of a container that shall be exposed.
        /// </param>
        /// <param name="externalPort">
        /// The external static port of a container that the internal port will be mapped to.
        /// Defaults to 0, which will let the OS choose a free port for you.
        ///
        /// Only provide an external port if a static external port is required.
        /// When the given external port is already in use by a container, the creation will fail.
        /// </param>
        /// <returns></returns>
        public ContainerResourceBuilder AddPortMapping(int internalPort, int externalPort = 0)
        {
            _options.AdditionalPortMappings.Add(
                new ContainerPortMapping()
                {
                    ExternalPort = externalPort, InternalPort = internalPort
                });
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
        /// Sets the setting, wheter squadron should pull the image or look for a local image first.
        /// If not set, squadron will always pull the image
        /// </summary>
        /// <returns></returns>
        public ContainerResourceBuilder PreferLocalImage()
        {
            _options.PreferLocalImage = true;
            return this;
        }

        /// <summary>
        /// Copies a file from the host to the container
        /// </summary>
        /// <param name="pathToLocalFile">Path to the file on the host</param>
        /// <param name="destinationOnContainer">Destination of the file in the container</param>
        /// <returns></returns>
        public ContainerResourceBuilder CopyFileToContainer(string pathToLocalFile, string destinationOnContainer)
        {
            _options.FilesToCopy.Add(new CopyContext(pathToLocalFile, destinationOnContainer));
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
