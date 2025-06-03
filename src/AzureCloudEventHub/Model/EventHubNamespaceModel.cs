using System.Collections.Generic;

namespace Squadron.Model;

/// <summary>
/// Azure Event Hub model
/// </summary>
public class EventHubNamespaceModel
{
    private List<EventHubModel> eventHubs = new();

    /// <summary>
    /// Gets or sets the namespace.
    /// </summary>
    /// <value>
    /// The namespace.
    /// </value>
    public string Namespace { get; set; }

    /// <summary>
    /// Gets or sets the provisioning mode.
    /// </summary>
    /// <value>
    /// The provisioning mode.
    /// </value>
    internal EventHubProvisioningMode ProvisioningMode { get; set; }
        = EventHubProvisioningMode.UseExisting;


    public void AddEventHub(EventHubModel eventHub)
    {
        eventHubs.Add(eventHub);
    }

    public IEnumerable<EventHubModel> GetEventHubs()
    {
        return eventHubs;
    }
}

/// <summary>
/// Defines Event Hub provisioning modes
/// </summary>
internal enum EventHubProvisioningMode
{
    /// <summary>
    /// The uan existing Azure resource
    /// </summary>
    UseExisting,

    /// <summary>
    /// Provision and delete resource
    /// </summary>
    CreateAndDelete
}