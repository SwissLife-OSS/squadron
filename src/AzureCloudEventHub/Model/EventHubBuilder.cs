namespace Squadron;

/// <summary>
/// ServiceBusQueue builder
/// </summary>
public class EventHubBuilder
{
    EventHubModel _eventHub = null;

    public static EventHubBuilder New(string eventHubName)
    {
        return new EventHubBuilder(eventHubName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubBuilder"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    private EventHubBuilder(string name)
    {
        _eventHub = new EventHubModel(name);
    }

    internal EventHubModel Build()
    {
        return _eventHub;
    }
}