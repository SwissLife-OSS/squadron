namespace Squadron
{
    /// <summary>
    /// Default Elasticsearch resource options
    /// </summary>
    public class ElasticsearchDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            var name = "elastic";
            builder
                .Name(name)
                .Image("docker.elastic.co/elasticsearch/elasticsearch:7.17.2")
                .InternalPort(9200)
                .WaitTimeout(60 * 5)
                .AddEnvironmentVariable("discovery.type=single-node")
                .AddEnvironmentVariable($"cluster.name={name}");
        }
    }
}
