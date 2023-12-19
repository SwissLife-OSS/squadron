// using System.Text;
// using System.Threading.Tasks;
// using Microsoft.Azure.ServiceBus;
// using Xunit;
//
// namespace Squadron.AzureServiceBus.Tests
// {
//     public class AzureNewServiceBusResourceTests
//         : IClassFixture<AzureCloudServiceBusResource<TestNewNamespaceAzureServiceBusOptions>>
//     {
//         private readonly AzureCloudServiceBusResource<TestNewNamespaceAzureServiceBusOptions> _resource;
//
//         public AzureNewServiceBusResourceTests(
//             AzureCloudServiceBusResource<TestNewNamespaceAzureServiceBusOptions> resource)
//         {
//             _resource = resource;
//         }
//
//
//         [Fact(Skip = "Can not run without Azure credentials")]
//         public async Task PrepareAzureServiceBusResource_NewNamespace_NoError()
//         {
//             ITopicClient topicClient = _resource.GetTopicClient("foo");
//             await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes("Hello")));
//             IQueueClient queueClient = _resource.GetQueueClient("bar");
//         }
//     }
// }
