using System.Collections.Generic;

namespace Squadron
{
    public class MongoImageSettings : IImageSettings
    {
        public string Name { get; } = ContainerName.Create();
        public string Image { get; } = "mongo:latest";
        public long ContainerPort { get; } = 27017;
        public long HostPort { get; set; }
        public string Username { get; } = string.Empty;
        public string Password { get; } = string.Empty;
        public List<string> EnvironmentVariable { get; } = new List<string>();
        public string ContainerId { get; set; }
        public string ContainerAddress { get; set; }
    }
}
