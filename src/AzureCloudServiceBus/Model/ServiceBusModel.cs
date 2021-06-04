using System;
using System.Collections.Generic;
using System.Text;

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
        internal ServiceBusProvisioningMode ProvisioningMode { get; set; }
            = ServiceBusProvisioningMode.UseExisting;
    }

    /// <summary>
    /// Defines ServiceBUs provisioning modes
    /// </summary>
    internal enum ServiceBusProvisioningMode
    {
        /// <summary>
        /// The uan existing Azure resource
        /// </summary>
        UseExisting,

        /// <summary>
        /// Provision and delete resource
        /// </summary>
        CreateAndDelete,

        /// <summary>
        /// Create or update resource
        /// </summary>
        CreateIfNotExists
    }
}
