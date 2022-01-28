using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;

namespace Squadron
{
    /// <summary>
    /// Status checker for AzureStorage Queues
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class AzureStorageQueueStatus : IResourceStatusProvider
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageQueueStatus"/> class.
        /// </summary>
        public AzureStorageQueueStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Determines whether Azure Queue is ready
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            QueueServiceClient queueServiceClient = new QueueServiceClient(_connectionString);
            Response<QueueServiceProperties> serviceProperties =
                            await queueServiceClient.GetPropertiesAsync(cancellationToken);
            return new Status
            {
                IsReady = serviceProperties != null,
                Message =
                    $"MinuteMetrics: {serviceProperties.Value.MinuteMetrics}, " +
                    $"HourMetrics: {serviceProperties.Value.HourMetrics}, " +
                    $"Cors: {serviceProperties.Value.Cors}"
            };
        }
    }
}
