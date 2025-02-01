using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Status checker for Gitea
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class GiteaStatus : IResourceStatusProvider
    {
        private readonly HttpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="GiteaStatus" /> class.
        /// </summary>
        /// <param name="url">The connection string.</param>
        public GiteaStatus(string url)
        {
            _client = new HttpClient() { BaseAddress = new Uri(url) };
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync("/api/v1/version", cancellationToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var jsonDoc = System.Text.Json.JsonDocument.Parse(content);
                return new Status
                {
                    IsReady = true, 
                    Message = jsonDoc.RootElement.GetProperty("version").GetString()
                };
            }
            catch (Exception ex)
            {
                return new Status { IsReady = false, Message = ex.Message };
            }
        }
    }
}
