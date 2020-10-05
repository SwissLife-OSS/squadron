using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;

namespace Squadron
{
    /// <inheritdoc/>
    public class AzureStorageQueueResource
        : AzureStorageQueueResource<AzureStorageQueueDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a AzureStorage queue resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageQueueResource<TOptions>
        : ContainerResource<TOptions>,
          ISquadronAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        CloudStorageAccount _storageAccount = null;

        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            ConnectionString = CloudStorageAccountBuilder.GetForQueue(Manager.Instance);
            _storageAccount = CloudStorageAccount.Parse(ConnectionString);

            await Initializer.WaitAsync(
                new AzureStorageQueueStatus(_storageAccount));
        }


        /// <summary>
        /// Creates a Queue client
        /// </summary>
        /// <returns></returns>
        public CloudQueueClient CreateQueueClient()
        {
            return _storageAccount.CreateCloudQueueClient();
        }
    }
}
