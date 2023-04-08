
using System.Threading.Tasks;
using Squadron;
using Xunit;
using Xunit.Abstractions;

namespace Opa.Tests
{
    public class OpaResourceTest : IClassFixture<OpaResource>
    {
        private readonly OpaResource _opaResource;
        private readonly ITestOutputHelper _helper;

        public OpaResourceTest(OpaResource opaResource, ITestOutputHelper helper)
        {
            _opaResource = opaResource;
            _helper = helper;
        }

        [Fact]
        public async Task Initializes_Container()
        {
            // Act
            await _opaResource.InitializeAsync();
            _helper.WriteLine($"Client address: {_opaResource.Client.BaseAddress}");

            // Assert
            Assert.True(true);
        }
    }
}
