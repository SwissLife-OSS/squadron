using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
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
            var message = "Hello";
            EventHubClient eventHubClient = await _eventHubResource.GetEventHubClientAsync("testEventHub");
            await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));

            PartitionReceiver receiver = await _eventHubResource.GetEventHubReceiverAsync("testEventHub");

            IEnumerable<EventData> events = await receiver.ReceiveAsync(1);
            EventData eventData = events.FirstOrDefault();

            Assert.NotNull(eventData);
            var result = Encoding.UTF8.GetString(eventData.Body);
            Assert.Equal(message, result);
        }
    }
}
