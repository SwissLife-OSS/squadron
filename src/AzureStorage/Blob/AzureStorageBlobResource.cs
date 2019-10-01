using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a AzureStorage blob resource that can be used by unit tests.
    /// Currenty Blob and Queues are supported by this resource
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageBlobResource
        : ResourceBase<AzureStorageBlobImageSettings>, IAsyncLifetime
    {
        CloudStorageAccount _storageAccount = null;

        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            _storageAccount = CloudStorageAccountBuilder.GetForBlob(Settings);

            await Initializer.WaitAsync(
                new AzureStorageBlobStatus(_storageAccount), Settings);
        }

        /// <summary>
        /// Creates a Blob client
        /// </summary>
        /// <returns></returns>
        public CloudBlobClient CreateBlobClient()
        {
            return _storageAccount.CreateCloudBlobClient();
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
