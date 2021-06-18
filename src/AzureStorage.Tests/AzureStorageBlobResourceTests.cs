using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Microsoft.Azure.Storage.Blob;
using Xunit;

namespace Squadron.AzureStorage.Tests
{
    public class AzureStorageBlobResourceTests : IClassFixture<AzureStorageBlobResource>
    {
        private readonly AzureStorageBlobResource _azureStorageResource;

        public AzureStorageBlobResourceTests(AzureStorageBlobResource azureStorageResource)
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
        public async Task BlobClient_UploadFile_ContentMatch()
        {
            //Arrange
            var blobServiceClient = new BlobServiceClient(_azureStorageResource.ConnectionString);
            BlobContainerClient containerClient =
                blobServiceClient.GetBlobContainerClient("foo");
            await containerClient.CreateIfNotExistsAsync();

            string inputText = "Hello_AzureStorage";
            var data = Encoding.UTF8.GetBytes(inputText);
            var memoryStream = new MemoryStream(data);

            //Act
            BlobClient textFile = containerClient.GetBlobClient("test.txt");
            await textFile.UploadAsync(memoryStream, overwrite: true, default);

            //Assert
            Azure.Response<BlobDownloadResult> downloadResult =
                await textFile.DownloadContentAsync();

            downloadResult.Value.Content.ToArray().Should().BeEquivalentTo(data);
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
