using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron;

/// <summary>
/// Container address
/// </summary>
public class ContainerAddress
{
    /// <summary>
    /// Gets or sets the address. (Fqdn or IP)
    /// </summary>
    /// <value>
    /// The address.
    /// </value>
    public string Address { get; internal set; }

    /// <summary>
    /// Gets the port.
    /// </summary>
    /// <value>
    /// The port.
    /// </value>
    public int Port { get; internal set; }
}