using System.Collections.Generic;

namespace Squadron
{
    public class ElasticsearchImageSettings : IImageSettings
    {
        public ElasticsearchImageSettings()
        {
            EnvironmentVariable =
                new List<string>
                {
                    "discovery.type=single-node",
                    $"cluster.name={Name}"
                };
        }

        public string Name { get; } = ContainerName.Create();
        public string Image { get; } = "docker.elastic.co/elasticsearch/elasticsearch:6.6.0";
        public long DefaultPort { get; } = 9200;
        public string ContainerId { get; set; }
        public string ContainerIp { get; set; }
        public string Username { get; } = string.Empty;
        public string Password { get; } = string.Empty;
        public List<string> EnvironmentVariable { get; }
    }
}
