using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Squadron
{
    public class Neo4jResourceTests
        : IClassFixture<Neo4jResource>
    {
        public Neo4jResourceTests(Neo4jResource neo4jResource)
        {
            Neo4jResource = neo4jResource;
        }

        private Neo4jResource Neo4jResource { get; }
    }
}
