
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Text.Json;
using System.IO;
using System.Net.Http.Headers;

namespace Squadron
{

    /// <inheritdoc/>
    public class OpaResource : OpaResource<OpaDefaultOptions> { }

    /// <summary>
    /// Represents a mongo database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class OpaResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime,
          IComposableResource
        where TOptions : ContainerResourceOptions, new()
    {
        private HttpClient _client = new();
        public HttpClient Client => _client;

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var baseAddress =
                $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";

            _client = GetClient(baseAddress);
            await Initializer.WaitAsync(new OpaStatus(_client));
        }

        public async Task<T?> ListPolicies<T>()
        {
            Stream contentStream = await ListPolicies();
            T? result = await JsonSerializer.DeserializeAsync<T>(contentStream);

            return result;
        }

        public async Task<Stream> ListPolicies()
        {
            return await _client.GetStreamAsync("/v1/policies");
        }

        public async Task SetPolicy(string policyContent)
        {
            HttpContent content = new StringContent(policyContent);
            await _client.PutAsync("/v1/policies", content);
        }

        public async Task EvalPolicy(string policyId, string input)
        {
            HttpContent content = new StringContent(input);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            await _client.PostAsync($"/v1/data/{policyId}", content);
        }

        private HttpClient GetClient(string baseAddress)
        {
            return new()
            {
                BaseAddress = new Uri(baseAddress),
            };
        }
    }
}
