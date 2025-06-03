using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using FluentAssertions;
using Xunit;

namespace Squadron.AzureStorage.Tests;

public class AzureStorageQueueResourceTests(AzureStorageQueueResource azureStorageResource)
    : IClassFixture<AzureStorageQueueResource>
{
    [Fact]
    public async Task CreateQueueClient_AddMessage_Peeked()
    {
        //Arrange
        QueueServiceClient queueServiceClient =
            azureStorageResource.CreateQueueServiceClient();
        QueueClient queue = queueServiceClient.GetQueueClient("foo");
        string messageText = "Hello_AzureStorage";
        await queue.CreateIfNotExistsAsync();

        //Act
        await queue.SendMessageAsync(messageText);

        //Assert
        Response<PeekedMessage> peekedMessage = await queue.PeekMessageAsync();
        Encoding.UTF8.GetString(peekedMessage.Value.Body.ToArray()).Should().Be(messageText);
    }

    [Fact]
    public void ConnectionString_NotNull()
    {
        //Arrange & Act
        string connectionString = azureStorageResource.ConnectionString;

        //Assert
        connectionString.Should().NotBeNull();
        connectionString.Should().Contain("QueueEndpoint");
    }

}