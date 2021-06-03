using Squadron.AzureCloud;
using Squadron.Model;

namespace Squadron
{
    /// <summary>
    /// Event Hub options builder
    /// </summary>
    public class EventHubOptionsBuilder : AzureResourceOptionsBuilder
    {
        /// <summary>
        /// Build an <see cref="EventHubModel"/> from options
        /// </summary>
        public EventHubModel Build()
        {
            return new EventHubModel();
        }
    }
}
