using System.Text.Json.Serialization;

namespace Squadron;

public record GiteaRepository(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("clone_url")] string CloneUrl,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("default_branch")] string DefaultBranch);
