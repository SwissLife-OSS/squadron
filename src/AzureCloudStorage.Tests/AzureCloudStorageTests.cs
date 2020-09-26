using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public void Foo()
        {
            var a = "";
        }
    }
}
