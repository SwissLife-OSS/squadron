using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.Samples.AzureCloud.ServiceBus
{
    public class UserEvent
    {
        public string Type { get; set; }

        public string UserId { get; set; }
    }
}
