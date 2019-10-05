using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud
{
    public class AzureResourceIdentifier
    {
        public string SubscriptionId { get; set; }
        public string ResourceGroupName { get; set; }

        public string Name { get; set; }
    }
}
