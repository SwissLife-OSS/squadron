using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron;

/// <summary>
/// Status checker for ClickHouse
/// </summary>
/// <seealso cref="IResourceStatusProvider" />
public class ClickHouseStatus(string address, string username, string password) : IResourceStatusProvider
{
    public ClickHouseStatus(string address) : this(address, "default", "")
    {
    }

    /// <inheritdoc/>
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient();
            
        httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        var content = new StringContent("SELECT version();");
            
        if (!string.IsNullOrEmpty(username))
        {
            content.Headers.Add("X-ClickHouse-User", username);
            if (!string.IsNullOrEmpty(password))
            {
                content.Headers.Add("X-ClickHouse-Key", password);
            }
        }

        HttpResponseMessage response =
            await httpClient.PostAsync(address, content, cancellationToken);

        return response.IsSuccessStatusCode switch
        {
            true => new Status() { IsReady = true },
            _ => Status.NotReady
        };
    }
}