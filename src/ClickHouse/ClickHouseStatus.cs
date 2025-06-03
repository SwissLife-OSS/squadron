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
        private readonly string _username;
        private readonly string _password;

        public ClickHouseStatus(string address)
        {
            _address = address;
            _username = "default";
            _password = "";
        }

        public ClickHouseStatus(string address, string username, string password)
        {
            _address = address;
            _username = username;
            _password = password;
        }

        /// <inheritdoc/>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using var httpClient = new HttpClient();
            
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            var content = new StringContent("SELECT version();");
            
            if (!string.IsNullOrEmpty(_username))
            {
                content.Headers.Add("X-ClickHouse-User", _username);
                if (!string.IsNullOrEmpty(_password))
                {
                    content.Headers.Add("X-ClickHouse-Key", _password);
                }
            }

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
