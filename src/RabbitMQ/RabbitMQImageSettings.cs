using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class RabbitMQImageSettings : IImageSettings
    {
        public string Name { get; } = ContainerName.Create();
        public string Image { get; } = "rabbitmq:3";
        public long ContainerPort { get; } = 5672;
        public long HostPort { get; set; }
        public string Username { get; } = "guest";
        public string Password { get; } = "guest";
        public List<string> EnvironmentVariable { get; } = new List<string>();
        public string ContainerId { get; set; }
        public string ContainerAddress { get; set; }
        public string Logs { get; set; }
    }
}
