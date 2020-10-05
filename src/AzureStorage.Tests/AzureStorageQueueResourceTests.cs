using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage.Queue;
using Xunit;

namespace Squadron.AzureStorage.Tests
{
    public class AzureStorageQueueResourceTests : IResourceFixture<AzureStorageQueueResource>
    {
        private readonly AzureStorageQueueResource _azureStorageResource;

        public AzureStorageQueueResourceTests(XUnitResource<AzureStorageQueueResource> azureStorageResource)
        {
            _azureStorageResource = azureStorageResource.Resource;
        }

        [Fact]
        public async Task CreateQueueClient_AddMessage_Peeked()
        {
            //Arrange
            CloudQueueClient queueClient = _azureStorageResource.CreateQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("foo");
            string messageText = "Hello_AzureStorage";

            await queue.CreateIfNotExistsAsync();
            var message = new CloudQueueMessage(messageText);

            //Act
            queue.AddMessage(message);

            //Assert
            CloudQueueMessage peekedMessage = await queue.PeekMessageAsync();
            peekedMessage.AsString.Should().Be(messageText);
        }

        [Fact]
        public void ConnectionString_NotNull()
        {
            //Arrange & Act
            string connectionString = _azureStorageResource.ConnectionString;

            //Assert
            connectionString.Should().NotBeNull();
            connectionString.Should().Contain("QueueEndpoint");
        }

    }
}
