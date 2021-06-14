using System.Collections.Generic;
using Squadron.AzureCloud;
using Squadron.Model;

namespace Squadron
{
    /// <summary>
    /// Event Hub options builder
    /// </summary>
    public class EventHubOptionsBuilder : AzureResourceOptionsBuilder
    {
        private EventHubNamespaceModel _model = new EventHubNamespaceModel();
        private List<EventHubBuilder> _eventHubs = new List<EventHubBuilder>();

        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static EventHubOptionsBuilder New() => new EventHubOptionsBuilder();


        private EventHubOptionsBuilder()
            : base()
        {

        }

        /// <summary>
        /// Namespace
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <returns></returns>
        public EventHubOptionsBuilder Namespace(string ns)
        {
            _model.Namespace = ns;
            return this;
        }

        public EventHubBuilder AddEventHub(string eventHubName)
        {
            var eventHubBuilder = EventHubBuilder.New(eventHubName);
            _eventHubs.Add(eventHubBuilder);
            return eventHubBuilder;
        }

        /// <summary>
        /// Build an <see cref="EventHubNamespaceModel"/> from options
        /// </summary>
        public EventHubNamespaceModel Build()
        {
            foreach (EventHubBuilder ev in _eventHubs)
            {
                _model.AddEventHub(ev.Build());
            }
            return _model;
        }
    }
}
