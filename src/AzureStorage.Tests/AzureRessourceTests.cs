using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Squadron;
using Xunit;

namespace AzureStorage.Tests
{
    public class AzureRessourceTests : IClassFixture<AzureStorageResource>
    {
        private readonly AzureStorageResource _azureStorageResource;

        public AzureRessourceTests(AzureStorageResource azureStorageResource)
        {
            _azureStorageResource = azureStorageResource;
        }

        [Fact]
        public async Task CreateBlobClient_UploadFile_ContentMatch()
        {
            //Arrange
            CloudBlobClient blobClient = _azureStorageResource.CreateBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("foo");
            await container.CreateIfNotExistsAsync();
            string inputText = "Hello_AzureStorage";
            var data = Encoding.UTF8.GetBytes(inputText);

            //Act
            CloudBlockBlob textFile = container.GetBlockBlobReference("test.txt");
            await textFile.UploadFromByteArrayAsync(data, 0, data.Length);

            //Assert
            string downloaded = await textFile.DownloadTextAsync();
            downloaded.Should().Be(inputText);
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
    }
}
