using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage.Shared.Protocol;

namespace Squadron
{
    /// <summary>
    /// Status checker for AzureStorage Queues
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class AzureStorageQueueStatus : IResourceStatusProvider
    {
        private readonly CloudStorageAccount _account;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageQueueStatus"/> class.
        /// </summary>
        public AzureStorageQueueStatus(CloudStorageAccount account)
        {
            _account = account;
        }

        /// <summary>
        /// Determines whether Azure Queue is ready
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            CloudQueueClient blobClient = _account.CreateCloudQueueClient();
            ServiceProperties serviceProperties =
                            await blobClient.GetServicePropertiesAsync(
                                                            new QueueRequestOptions(),
                                                            default);
            return new Status
            {
                IsReady = serviceProperties != null,
                Message = _account.QueueStorageUri.ToString()
            };
        }
    }
}
