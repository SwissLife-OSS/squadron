namespace Squadron;

/// <summary>
/// Docker configuration
/// </summary>
public class DockerConfiguration
{
    /// <summary>
    /// Gets or sets the default address mode for containers.
    /// </summary>
    public ContainerAddressMode DefaultAddressMode { get; internal set; } = ContainerAddressMode.Port;
}

public enum ContainerAddressMode
{
    Auto,
    IpAddress,
    Port
}