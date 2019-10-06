using System;

namespace Squadron
{
    /// <summary>
    /// Builder for container settings
    /// </summary>
    public class ContainerResourceBuilder
    {
        private readonly ContainerResourceSettings _options = new ContainerResourceSettings();

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
            if ( parts.Length > 1)
            {
                _options.Image = parts[0];
                _options.Tag = parts[1];
            }
            else
                _options.Image = image;
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
        /// Wait timeout in seconds
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <returns></returns>
        public ContainerResourceBuilder WaitTimemout(int seconds)
        {
            _options.WaitTimeout = TimeSpan.FromSeconds(seconds);
            return this;
        }

        /// <summary>
        /// Builds the settings
        /// </summary>
        /// <returns></returns>
        public ContainerResourceSettings Build()
        {
            return _options;
        }
    }
}
