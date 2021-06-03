using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Squadron.AzureCloudEventHub.Tests
{
    public class AzureNewEventHubResourceTests
        : IClassFixture<AzureCloudEventHubResource<TestNewNamespaceAzureEventHubOptions>>
    {
        private readonly AzureCloudEventHubResource<TestNewNamespaceAzureEventHubOptions> _eventHubResource;

        public AzureNewEventHubResourceTests(AzureCloudEventHubResource<TestNewNamespaceAzureEventHubOptions> eventHubResource)
        {
            _eventHubResource = eventHubResource;
        }

        [Fact]
        public async Task PrepareAzureEventHubResource_NewNamespace_NoError()
        {

        }
    }
}
