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
        /// Port of application inside container
        /// </summary>
        public int InternalPort { get; internal set; }

        /// <summary>
        /// Docker image tag
        /// </summary>
        public string Tag { get; internal set; }

        /// <summary>
        /// Environment variables
        /// </summary>
        public IList<string> EnvironmentVariables { get; internal set; }
            = new List<string>();

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
        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Unique container name
        /// </summary>
        public string UniqueContainerName
            => $"squa_{Name.ToLowerInvariant()}_{DateTime.UtcNow.Ticks}";
    }
}
