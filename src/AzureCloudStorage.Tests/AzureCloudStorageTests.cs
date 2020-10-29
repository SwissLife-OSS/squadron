using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using FluentAssertions;
using Squadron;
using Xunit;

namespace Squadron.AzureCloudStorage.Tests
{
    public class TestNewStorageAzureOptions : AzureCloudStorageAccountOptions
    {
        public override void Configure(StorageAccountOptionsBuilder builder)
        {
            builder
                .AddBlobContainer("foo", isLegalHold: true)
                .AddBlobContainer("bard", isLegalHold: false);
        }
    }

    public class AzureCloudStorageTests
        : IClassFixture<AzureCloudStorageAccountResource<TestNewStorageAzureOptions>>
    {
        private readonly AzureCloudStorageAccountResource<TestNewStorageAzureOptions> _storageAccount;

        public AzureCloudStorageTests(
            AzureCloudStorageAccountResource<TestNewStorageAzureOptions> storageAccount)
        {
            _storageAccount = storageAccount;
        }

        [Fact(Skip = "Can not run without Azure credentials")]
        public async Task CreateBlobClient_UploadFile_ContentMatch()
        {
            //Arrange
            BlobContainerClient container = _storageAccount.CreateBlobContainerClient("foo");
            await container.CreateIfNotExistsAsync();
            var inputText = "Hello_AzureCloudStorage";
            var data = Encoding.UTF8.GetBytes(inputText);
            var inputStream = new MemoryStream(data);

            //Act
            BlobClient blobClient = container.GetBlobClient("test.txt");
            await blobClient.UploadAsync(inputStream);

            //Assert
            BlobDownloadInfo downloaded = blobClient.Download();
            var reader = new StreamReader(downloaded.Content);
            var downloadedText = reader.ReadToEnd();
            downloadedText.Should().Be(inputText);
        }

        [Fact(Skip = "Can not run without Azure credentials")]
        public void ConnectionString_NotNull()
        {
            //Arrange & Act
            var connectionString = _storageAccount.ConnectionString;

            //Assert
            connectionString.Should().NotBeNull();
            connectionString.Should().Contain("BlobEndpoint");
        }
    }
}
