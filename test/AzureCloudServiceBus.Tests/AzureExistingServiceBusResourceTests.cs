using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Xunit;
using Xunit.Abstractions;

namespace Squadron.AzureServiceBus.Tests;

public class AzureExistingServiceBusResourceTests
    : IClassFixture<AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions>>
{
    private readonly AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> _resource;

    public AzureExistingServiceBusResourceTests(
        AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> resource,
        ITestOutputHelper outputHelper)
    {
        _resource = resource;
    }


    [Fact( Skip ="Can not run without Azure credentials")]
    public async Task PrepareAzureServiceBusResource_NewNamespace_NoError()
    {
        ITopicClient topicClient = _resource.GetTopicClient("foo");
        await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes("Hello")));
        IQueueClient queueClient = _resource.GetQueueClient("bar");
    }
}