using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.Samples.AzureCloud.ServiceBus
{
    public class UserEventServiceBusOptions : AzureCloudServiceBusOptions
    {
        public override void Configure(ServiceBusOptionsBuilder builder)
        {
            builder.Namespace("spc-a-squadron-sb01")
                   .AddTopic("userevents")
                   .AddSubscription("audit", "EventType = 'USER_ADDED'");
        }
    }
}
