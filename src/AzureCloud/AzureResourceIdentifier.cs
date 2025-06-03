using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud;

/// <summary>
/// Azure resource identifier
/// </summary>
public class AzureResourceIdentifier
{
    /// <summary>
    /// Gets or sets the Azure subscriptionId.
    /// </summary>
    /// <value>
    /// The subscriptionId.
    /// </value>
    public string SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the name of the resource group.
    /// </summary>
    /// <value>
    /// The name of the resource group.
    /// </value>
    public string ResourceGroupName { get; set; }


    /// <summary>
    /// Gets or sets the name of the resource
    /// </summary>
    /// <value>
    /// The name.
    /// </value>
    public string Name { get; set; }
}