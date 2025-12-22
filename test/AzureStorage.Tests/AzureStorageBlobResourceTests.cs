using System;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Xunit;

namespace Squadron.AzureStorage.Tests;

public class AzureStorageBlobResourceTests(AzureStorageBlobResource azureStorageResource)
    : IClassFixture<AzureStorageBlobResource>
{
    [Fact]
    public async Task CreateBlobClient_UploadFile_ContentMatch()
    {
        //Arrange
        BlobServiceClient blobServiceClient = azureStorageResource.CreateBlobServiceClient();
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("foo");
        await containerClient.CreateIfNotExistsAsync();
        string inputText = "Hello_AzureStorage";
        var data = Encoding.UTF8.GetBytes(inputText);

        //Act
        BlobClient textFile = containerClient.GetBlobClient("test.txt");
        await textFile.UploadAsync(new BinaryData(data));

        //Assert
        Response<BlobDownloadResult> downloaded = await textFile.DownloadContentAsync();
        var downloadedFileContent = Encoding.UTF8.GetString(downloaded.Value.Content.ToArray());
        downloadedFileContent.Should().Be(inputText);
    }

    [Fact]
    public void ConnectionString_NotNull()
    {
        //Arrange & Act
        string connectionString = azureStorageResource.ConnectionString;

        //Assert
        connectionString.Should().NotBeNull();
        connectionString.Should().Contain("BlobEndpoint");
    }
}