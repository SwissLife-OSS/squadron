using System.Collections.Generic;

namespace Squadron
{
    public interface IImageSettings
    {
        string Name { get; }
        string Image { get; }
        long ContainerPort { get; }
        long HostPort { set; get; }
        string Username { get; }
        string Password { get; }
        List<string> EnvironmentVariable { get; }
        string ContainerId { get; set; }
        string ContainerAddress { get; set; }
    }
}
