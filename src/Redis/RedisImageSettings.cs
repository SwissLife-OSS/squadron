using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class RedisImageSettings : IImageSettings
    {
        public string Name { get; } = ContainerName.Create();
        public string Image { get; } = "redis:latest";
        public long ContainerPort { get; } = 6379;
        public long HostPort { get; set; }
        public string Username { get; }
        public string Password { get; }
        public List<string> EnvironmentVariable { get; } = new List<string>();
        public string ContainerId { get; set; }
        public string ContainerAddress { get; set; }
        public string Logs { get; set; }
    }
}
