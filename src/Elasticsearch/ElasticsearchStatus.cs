using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace Squadron
{
    /// <summary>
    /// Status checker for Elasticsearch
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class ElasticsearchStatus : IResourceStatusProvider
    {
        private readonly IElasticClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticsearchStatus"/> class.
        /// </summary>
        public ElasticsearchStatus(IElasticClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Determines whether Elasticsearch cluster is ready.
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            var healthRequest = new ClusterHealthRequest
            {
                WaitForStatus = WaitForStatus.Green
            };
            IClusterHealthResponse healthResponse = await _client
                .ClusterHealthAsync(healthRequest, cancellationToken);

            return new Status
            {
                IsReady = healthResponse.IsValid,
                Message = healthResponse.DebugInformation
            };
        }
    }
}
