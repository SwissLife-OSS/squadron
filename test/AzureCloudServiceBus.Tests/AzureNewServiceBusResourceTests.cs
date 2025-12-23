using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Xunit;

namespace Squadron.AzureServiceBus.Tests;

public class AzureNewServiceBusResourceTests(
    AzureCloudServiceBusResource<TestNewNamespaceAzureServiceBusOptions> resource)
    : IClassFixture<AzureCloudServiceBusResource<TestNewNamespaceAzureServiceBusOptions>>
{
    [Fact(Skip = "Can not run without Azure credentials")]
    public async Task PrepareAzureServiceBusResource_NewNamespace_NoError()
    {
        ITopicClient topicClient = resource.GetTopicClient("foo");
        await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes("Hello")));
        IQueueClient queueClient = resource.GetQueueClient("bar");
    }
}