using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    public class Neo4jResource : Neo4jResource<Neo4jDefaultOptions> { }

    /// <summary>
    /// Represents a neo4j resource that can be used by unit tests.
    /// </summary>
    public class Neo4jResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// ConnectionString
        /// </summary>
        public string ConnectionString { get; private set; }

        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
        }
    }
}