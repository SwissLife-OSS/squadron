
namespace Squadron.AzureServiceBus.Tests;

public class TestNewNamespaceAzureServiceBusOptions : AzureCloudServiceBusOptions
{
    public override void Configure(ServiceBusOptionsBuilder builder)
    {
        builder.AddTopic("foo")
            .AddSubscription("test1", "EventType = 'test1'");
        builder.AddQueue("bar");
    }
}