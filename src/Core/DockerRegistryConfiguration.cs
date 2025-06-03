namespace Squadron;

/// <summary>
/// Docker registry configuration
/// </summary>
public class DockerRegistryConfiguration
{
    /// <summary>
    /// Name that can be used in the container resource
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }

    /// <summary>
    /// Adress without protocol
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public string Address { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    /// <value>
    /// The username.
    /// </value>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    /// <value>
    /// The password.
    /// </value>
    public string Password { get; set; }
}