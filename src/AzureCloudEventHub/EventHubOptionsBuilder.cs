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
        private EventHubModel _model = new EventHubModel();

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
        /// <summary>
        /// Build an <see cref="EventHubModel"/> from options
        /// </summary>
        public EventHubModel Build()
        {
            return _model;
        }
    }
}
