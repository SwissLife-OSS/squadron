using System;

namespace Squadron.AzureServiceBus.Tests
{
    public class TestExistingNamespaceAzureServiceBusOptions : AzureCloudServiceBusOptions
    {
        public override void Configure(ServiceBusOptionsBuilder builder)
        {
            builder.Namespace("spc-a-squadron-sb01")
                   .AddTopic("foo")
                   .AddSubscription("test1", "EventType = 'test1'");
            builder.AddQueue("bar");
        }

    }
}
