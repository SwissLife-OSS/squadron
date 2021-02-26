using System;
using System.Collections.Generic;

namespace Squadron
{
    /// <summary>
    /// Defaines container resource settings
    /// </summary>
    public class ContainerResourceSettings
    {
        /// <summary>
        /// Container name
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Docker image path
        /// </summary>
        public string Image { get; internal set; }

        /// <summary>
        /// Returns the main internal port of the container
        /// </summary>
        public int InternalPort { get; internal set; }

        /// <summary>
        /// Returns the main external port of the container
        /// </summary>
        /// <value>
        /// The external port.
        /// </value>
        public int ExternalPort { get; internal set; }

        /// <summary>
        /// A list of all additional port mappings that are active for this resources container
        /// </summary>
        public IList<ContainerPortMapping> AdditionalPortMappings { get; internal set; } =
            new List<ContainerPortMapping>();

        /// <summary>
        /// Docker image tag
        /// </summary>
        public string Tag { get; internal set; }

        /// <summary>
        /// Gets or sets the name of the Container registry as defined in configuation
        /// Defauls is DockerHub
        /// </summary>
        /// <value>
        /// The name of the registry.
        /// </value>
        public string RegistryName { get; internal set; }

        public ContainerAddressMode AddressMode { get; internal set; }

        /// <summary>
        /// Environment variables
        /// </summary>
        public IList<string> EnvironmentVariables { get; internal set; }
            = new List<string>();

        /// <summary>
        /// Volumes
        /// </summary>
        public IList<string> Volumes { get; internal set; }
            = new List<string>();

        public IList<string> Cmd { get; internal set; }

        /// <summary>
        /// Password to access resource such as database password
        /// </summary>
        public string Password { get; internal set; }

        /// <summary>
        /// Username to access resource such as a database user
        /// </summary>
        public string Username { get; internal set; }

        /// <summary>
        /// Image name including tag
        /// </summary>
        public string ImageFullname => $"{Image}:{Tag ?? "latest"}";

        /// <summary>
        /// Time to wait until "readyness" of container
        /// </summary>
        public TimeSpan WaitTimeout { get; internal set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// The docker networks that the container should be part of
        /// </summary>
        public IList<string> Networks { get; internal set; } =
            new List<string>();

        /// <summary>
        /// Unique container name
        /// </summary>
        public string UniqueContainerName { get; internal set; }

        /// <summary>
        /// Wether to always pull an image or look for a local one
        /// </summary>
        /// <value></value>
        public bool PreferLocalImage {get; internal set;} = true;

        /// <summary>
        /// Files to copy from local to the container
        /// </summary>
        public IList<CopyContext> FilesToCopy { get; internal set; } = new List<CopyContext>();

        /// <summary>
        /// Gets the docker configuration resolver.
        /// </summary>
        /// <value>
        /// The docker configuration resolver.
        /// </value>
        public Func<DockerConfiguration> DockerConfigResolver { get; internal set; }

    }
}
