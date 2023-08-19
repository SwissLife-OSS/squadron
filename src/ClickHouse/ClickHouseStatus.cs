using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Status checker for ClickHouse
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class ClickHouseStatus : IResourceStatusProvider
    {
        private readonly string _address;

        public ClickHouseStatus(string address)
        {
            _address = address;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();

            // Setting headers
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");

            var content = new StringContent("SELECT version();");

            HttpResponseMessage response =
                await httpClient.PostAsync(_address, content, cancellationToken);

            return response.IsSuccessStatusCode switch
            {
                true => new Status() { IsReady = true },
                _ => Status.NotReady
            };
        }
    }
}
