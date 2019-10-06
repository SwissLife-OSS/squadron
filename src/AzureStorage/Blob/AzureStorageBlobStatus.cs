using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;

namespace Squadron
{
    /// <summary>
    /// Status checker for AzureStorage Blob
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class AzureStorageBlobStatus : IResourceStatusProvider
    {
        private readonly CloudStorageAccount _account;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobStatus"/> class.
        /// </summary>
        public AzureStorageBlobStatus(CloudStorageAccount account)
        {
            _account = account;
        }

        /// <summary>
        /// Determines whether Azure Blob is ready
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            CloudBlobClient blobClient = _account.CreateCloudBlobClient();
            ServiceProperties serviceProperties =
                            await blobClient.GetServicePropertiesAsync(
                                                            new BlobRequestOptions(),
                                                            default);
            return new Status
            {
                IsReady = serviceProperties != null,
                Message = _account.BlobStorageUri.ToString()
            };
        }
    }
}
