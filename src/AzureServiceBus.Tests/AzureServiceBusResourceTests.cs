using System.Threading.Tasks;
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
        var receiver = azureServiceBusResource.Client
            .CreateReceiver(AzureServiceBusStatus.QueueName);
        var message = await receiver.ReceiveMessageAsync();
        var receivedMessage = message.Body.ToString();

        receivedMessage.Should().Be("status_check");
    }
}