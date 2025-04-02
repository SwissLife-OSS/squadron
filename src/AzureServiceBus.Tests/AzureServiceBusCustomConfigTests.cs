using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Xunit;

namespace Squadron.AzureServiceBus.Tests;

public class AzureServiceBusCustomConfigTests(
    AzureServiceBusResources<CustomConfig> azureServiceBusResource) :
    IClassFixture<AzureServiceBusResources<CustomConfig>>
{
    [Fact]
    public async Task Send_And_Receive()
    {
        var sender = azureServiceBusResource.Client
            .CreateSender("custom.topic");
        var serviceBusMessage = new ServiceBusMessage(BinaryData.FromString("custom_message"));
        await sender.SendMessageAsync(serviceBusMessage);
        
        var receiver = azureServiceBusResource.Client
            .CreateReceiver("custom.queue");
        var message = await receiver.ReceiveMessageAsync();
        var receivedMessage = message.Body.ToString();

        receivedMessage.Should().Be("custom_message");
    }
}

public class CustomConfig : AzureServiceBusConfig
{
    protected override Queue[] CreateQueues()
    {
        return [new Queue("custom.queue")];
    }

    protected override Topic[] CreateTopics()
    {
        var subscription = new Subscription(
            "custom.subscription", 
            new SubscriptionProperties(ForwardTo: "custom.queue"));
        
        return [new Topic("custom.topic", Subscriptions: [subscription])];
    }
}
