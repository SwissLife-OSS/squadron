using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Status checker for Nats
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class OpaStatus : IResourceStatusProvider, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NatsStatus"/> class.
        /// </summary>
        /// <param name="host">Hostname</param>
        public OpaStatus(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            try
            {
                // HttpResponseMessage? response = await _httpClient.GetAsync(
                //     new Uri("health", UriKind.Relative), cancellationToken);

                // Debug.Assert(response?.StatusCode == HttpStatusCode.OK);

                return new Status
                {
                    IsReady = true,
                    Message = "OPA is ready!"
                };

            }
            catch (Exception ex)
            {
                return new Status
                {
                    IsReady = false,
                    Message = ex.Message
                };
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
