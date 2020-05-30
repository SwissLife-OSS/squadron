using System;

namespace Squadron
{
    /// <summary>
    /// Default Neo4j resource options
    /// </summary>
    public class Neo4jDefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {
        public Type ResourceType => typeof(Neo4jResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("neo4j")
                .Image("neo4j:latest")
                .InternalPort(7474);
        }
    }
}