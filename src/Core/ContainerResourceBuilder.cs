using System;

namespace Squadron
{
    public class ContainerResourceBuilder
    {
        private readonly ContainerResourceSettings _options = new ContainerResourceSettings();

        public static ContainerResourceBuilder New() => new ContainerResourceBuilder();

        public ContainerResourceBuilder Name(string name)
        {
            _options.Name = name;
            return this;
        }

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

        public ContainerResourceBuilder AddEnvironmentVariable(string variable)
        {
            _options.EnvironmentVariables.Add(variable);
            return this;
        }

        public ContainerResourceBuilder Tag(string tag)
        {
            _options.Tag = tag;
            return this;
        }

        public ContainerResourceBuilder Username(string username)
        {
            _options.Username = username;
            return this;
        }

        public ContainerResourceBuilder Password(string password)
        {
            _options.Password = password;
            return this;
        }

        public ContainerResourceBuilder InternalPort(int port)
        {
            _options.InternalPort = port;
            return this;
        }

        public ContainerResourceBuilder WaitTimout(int seconds)
        {
            _options.WaitTimeout = TimeSpan.FromSeconds(seconds);
            return this;
        }

        public ContainerResourceSettings Build()
        {
            return _options;
        }
    }
}
