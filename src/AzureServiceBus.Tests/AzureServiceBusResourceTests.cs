using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Xunit;

namespace Squadron.AzureServiceBus.Tests;

public class AzureServiceBusResourceTests(
    AzureServiceBusResources azureServiceBusResource) :
    IClassFixture<AzureServiceBusResources>
{
    [Fact]
    public async Task Send_And_Receive()
    {
        var sender = azureServiceBusResource.Client.CreateSender("topic.1");
        var sendMessage = new TestMessage(Guid.NewGuid());
        var serviceBusMessage = new ServiceBusMessage(BinaryData.FromObjectAsJson(sendMessage));
        await sender.SendMessageAsync(serviceBusMessage, CancellationToken.None);
        
        var receiver = azureServiceBusResource.Client.CreateReceiver("topic.1", "subscription.3");
        var message = await receiver.ReceiveMessageAsync();
        var receivedMessage = message.Body.ToObjectFromJson<TestMessage>();

        receivedMessage.Id.Should().Be(sendMessage.Id);
    }

    public record TestMessage(Guid Id);
}