using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage.Blob;
using Xunit;

namespace Squadron.AzureStorage.Tests
{
    public class AzureStorageBlobSquadronResourceTests : ISquadronResourceFixture<AzureStorageBlobResource>
    {
        private readonly AzureStorageBlobResource _azureStorageResource;

        public AzureStorageBlobSquadronResourceTests(SquadronResource<AzureStorageBlobResource> azureStorageResource)
        {
            _azureStorageResource = azureStorageResource.Resource;
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
        public void ConnectionString_NotNull()
        {
            //Arrange & Act
            string connectionString = _azureStorageResource.ConnectionString;

            //Assert
            connectionString.Should().NotBeNull();
            connectionString.Should().Contain("BlobEndpoint");
        }
    }
}
