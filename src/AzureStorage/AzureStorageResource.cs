using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a AzureStorage resource that can be used by unit tests.
    /// Currenty Blob and Queues are supported by this resource
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageResource
        : ResourceBase<AzureStorageImageSettings>, IAsyncLifetime
    {

        CloudStorageAccount _storageAccount = null;

        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            var connectionString =
                "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;" +
                "AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4" +
                "I6tq/K1SZFPTOtr/KBHBeksoGMGw==;" +
                $"BlobEndpoint=http://{Settings.ContainerAddress}:10000/devstoreaccount1;" +
                $"QueueEndpoint=http://{Settings.ContainerAddress}:10001/devstoreaccount1;";

            _storageAccount = CloudStorageAccount.Parse(connectionString);
            await Initializer.WaitAsync(
                new AzureStorageStatus(_storageAccount), Settings);
        }

        /// <summary>
        /// Creates a Blob client
        /// </summary>
        /// <returns></returns>
        public CloudBlobClient CreateBlobClient()
        {
            return _storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Creates a Queue client
        /// </summary>
        /// <returns></returns>
        public CloudQueueClient CreateQueueClient()
        {
            return _storageAccount.CreateCloudQueueClient();
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
