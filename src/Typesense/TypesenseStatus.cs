using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Status checker for Typesense
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class TypesenseStatus : IResourceStatusProvider
    {
        private readonly string _baseUrl;
        private readonly string _apiKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypesenseStatus"/> class.
        /// </summary>
        /// <param name="baseUrl">The ConnectionString</param>
        public TypesenseStatus(string baseUrl, string apiKey)
        {
            _baseUrl = baseUrl;
            _apiKey = apiKey;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient() { BaseAddress = new Uri(_baseUrl) };
            httpClient.DefaultRequestHeaders.Add("X-TYPESENSE-API-KEY", _apiKey);
            HttpResponseMessage response = await httpClient.GetAsync("/health", cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                Stream stream = await response.Content.ReadAsStreamAsync();
                using JsonDocument jsonDocument = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
                JsonElement jsonResponse = jsonDocument.RootElement;

                return new Status { IsReady = jsonResponse.GetProperty("ok").GetBoolean() };
            }

            return new Status { IsReady = false };
        }
    }
}
