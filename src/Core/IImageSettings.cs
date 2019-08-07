using System.Collections.Generic;

namespace Squadron
{
    public interface IImageSettings
    {
        string Name { get; }
        string Image { get; }
        long DefaultPort { get; }
        string Username { get; }
        string Password { get; }
        List<string> EnvironmentVariable { get; }
        string ContainerId { get; set; }
        string ContainerIp { get; set; }
    }
}
