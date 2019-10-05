using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class ServiceBusModel 
    {
        public string Namespace { get; set; }

        public List<ServiceBusTopicModel> Topics { get; set; }
            = new List<ServiceBusTopicModel>();

        public List<ServiceBusQueueModel> Queues { get; set; }
            = new List<ServiceBusQueueModel>();

        internal ServiceBusProvisioningMode ProvisioningMode { get; set; }
            = ServiceBusProvisioningMode.UseExisting;
    }

    internal enum ServiceBusProvisioningMode
    {
        UseExisting,
        CreateAndDelete
    }
}
