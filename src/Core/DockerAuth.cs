using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Squadron;

public class DockerAuth
{
    [JsonPropertyName("auth")]
    public string Auth { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}

public class DockerAuthRootObject
{
    [JsonPropertyName("auths")]
    public Dictionary<string, DockerAuth> Auths { get; set; }

    [JsonPropertyName("HttpHeaders")]
    public Dictionary<string, string> HttpHeaders { get; set; }
}