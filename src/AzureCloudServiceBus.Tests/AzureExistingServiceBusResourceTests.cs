using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Xunit;
using Xunit.Abstractions;

namespace Squadron.AzureServiceBus.Tests
{
    public class AzureExistingServiceBusResourceTests
        : IClassFixture<AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions>>
    {
        private readonly AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> _resource;

        public AzureExistingServiceBusResourceTests(
            AzureCloudServiceBusResource<TestExistingNamespaceAzureServiceBusOptions> resource,
            ITestOutputHelper outputHelper)
        {
            _resource = resource;
            var converter = new Converter(outputHelper);
            Console.SetOut(converter);
        }


        [Fact( Skip ="Can not run without Azure credentials")]
        public async Task PrepareAzureServiceBusResource_ExistingNamespace_NoError()
        {
            ITopicClient topicClient = _resource.GetTopicClient("foo");
            ISubscriptionClient subscriptionClient =
                _resource.GetSubscriptionClient("foo", "test1");

            IQueueClient queueClient = _resource.GetQueueClient("bar");

            //subscriptionClient.RegisterMessageHandler()

            await topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes("Hello")));

            ITopicClient newTopic = await _resource.CreateTopicAsync(b => b
                                            .Name("adhoc")
                                            .AddSubscription("test1"));
        }
    }


    internal class Converter : TextWriter
    {
        ITestOutputHelper _output;
        public Converter(ITestOutputHelper output)
        {
            _output = output;
        }
        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }
        public override void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }
    }
}
