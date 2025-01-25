using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Squadron;

/// <inheritdoc/>
public class GiteaResource : GiteaResource<GiteaDefaultOptions>
{
}

/// <summary>
/// Represents a Gitea database that can be used by unit tests.
/// </summary>
public class GiteaResource<TOptions>
    : ContainerResource<TOptions>, IAsyncLifetime
    where TOptions : ContainerResourceOptions, new()
{
    /// <summary>
    /// Connection string to access to database
    /// </summary>
    public string Url { get; private set; }

    public string Token { get; private set; }
    
    public HttpClient CreateApiClient()
    {
        var client = new HttpClient { BaseAddress = new Uri(Url) };
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", Token);
        
        return client;
    }
    
    public async Task<GiteaRepository> CreateRepositoryAsync(string name)
    {
        HttpClient client = CreateApiClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/user/repos");
        request.Content = JsonContent.Create(new
        {
            name,
            auto_init = true
        });
        
        HttpResponseMessage response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to create repository: {result}");
        }
        var repo = await response.Content.ReadFromJsonAsync<GiteaRepository>();

        return repo;
    }

    /// <inheritdoc/>
    protected override void OnSettingsBuilded(ContainerResourceSettings settings)
    {
    }
    
    /// <inheritdoc cref="IAsyncLifetime"/>
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        Url = $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";
        await Initializer.WaitAsync(new GiteaStatus(Url));
        
        var result = await Manager.InvokeCommandAsync(CreateUserCommand.Execute(Settings));
        if (!result.Contains("successfully created!"))
        {
            throw new InvalidOperationException($"Failed to create user: {result}");
        }
        Token = await CreateTokenAsync();
    }

    private async Task<string> CreateTokenAsync()
    {
        var client = new HttpClient { BaseAddress = new Uri(Url) };

        var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/users/{Settings.Username}/tokens");
        request.Content = request.Content =
            JsonContent.Create(new
            {
                name = "squadron", scopes = new[]
                {
                    "write:user", 
                    "write:repository", 
                    "write:admin", 
                    "write:organization"
                }
            });
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Settings.Username}:{Settings.Password}")));

        HttpResponseMessage response = await client.SendAsync(request);
        
        var result = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            using var doc = JsonDocument.Parse(result);
            return doc.RootElement.GetProperty("sha1").GetString();
        }
        
        throw new InvalidOperationException($"Failed to create token: {result}");
    }
}

public record GiteaRepository(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("clone_url")] string CloneUrl,
    [property: JsonPropertyName("default_branch")] string DefaultBranch);