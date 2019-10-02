using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a AzureStorage queue resource that can be used by unit tests.
    /// Currenty Blob and Queues are supported by this resource
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageQueueResource
        : ResourceBase<AzureStorageQueueImageSettings>, IAsyncLifetime
    {
        CloudStorageAccount _storageAccount = null;

        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            ConnectionString = CloudStorageAccountBuilder.GetForQueue(Settings);
            _storageAccount = CloudStorageAccount.Parse(ConnectionString);

            await Initializer.WaitAsync(
                new AzureStorageQueueStatus(_storageAccount), Settings);
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
