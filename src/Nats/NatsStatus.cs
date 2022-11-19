using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Status checker for Nats
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class NatsStatus : IResourceStatusProvider, IDisposable
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="NatsStatus"/> class.
        /// </summary>
        /// <param name="host">Hostname</param>
        public NatsStatus(string host)
        {
            if (!Uri.TryCreate($"http://{host}/", UriKind.Absolute, out Uri uri))
            {
                throw new InvalidOperationException($"Bad host string '{host}'");
            }

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(5),
                BaseAddress = uri
            };
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            try {
                HealthzResponse response = await _httpClient.GetFromJsonAsync<HealthzResponse>(
                    new Uri("/healthz", UriKind.Relative), cancellationToken);

                Debug.Assert(response?.Status == "ok", "status == 'ok'");

                return new Status
                {
                    IsReady = true,
                    Message = "NATS is ready!"
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

        // ReSharper disable once IdentifierTypo
        private class HealthzResponse
        {
            public string Status { get; set; }
        }
    }
}
