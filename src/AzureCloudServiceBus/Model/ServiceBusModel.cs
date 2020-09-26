using System;
using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;

namespace Squadron
{
    /// <summary>
    /// Azure ServiceBus model
    /// </summary>
    public class ServiceBusModel 
    {
        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public string Namespace { get; set; }

        /// <summary>
        /// Gets or sets the topics.
        /// </summary>
        /// <value>
        /// The topics.
        /// </value>
        public List<ServiceBusTopicModel> Topics { get; set; }
            = new List<ServiceBusTopicModel>();

        /// <summary>
        /// Gets or sets the queues.
        /// </summary>
        /// <value>
        /// The queues.
        /// </value>
        public List<ServiceBusQueueModel> Queues { get; set; }
            = new List<ServiceBusQueueModel>();

        /// <summary>
        /// Gets or sets the provisioning mode.
        /// </summary>
        /// <value>
        /// The provisioning mode.
        /// </value>
        internal AzureResourceProvisioningMode ProvisioningMode { get; set; }
            = AzureResourceProvisioningMode.UseExisting;
    }
}
