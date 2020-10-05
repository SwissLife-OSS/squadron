using System;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;


namespace Squadron
{

    /// <inheritdoc/>
    public class AzureStorageBlobResource
        : AzureStorageBlobResource<AzureStorageBlobDefaultOptions>
    {
    }


    /// <summary>
    /// Represents a AzureStorage blob resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageBlobResource<TOptions>
        : ContainerResource<TOptions>,
          ISquadronAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        CloudStorageAccount _storageAccount = null;

        /// <summary>
        /// Connection string to the blob
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = CloudStorageAccountBuilder.GetForBlob(Manager.Instance);
            _storageAccount = CloudStorageAccount.Parse(ConnectionString);

            await Initializer.WaitAsync(
                new AzureStorageBlobStatus(_storageAccount));
        }

        /// <summary>
        /// Creates a Blob client
        /// </summary>
        /// <returns></returns>
        public CloudBlobClient CreateBlobClient()
        {
            return _storageAccount.CreateCloudBlobClient();
        }
    }
}
