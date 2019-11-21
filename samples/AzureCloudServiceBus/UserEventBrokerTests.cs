using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.ServiceBus;
using Xunit;

namespace Squadron.Samples.AzureCloud.ServiceBus
{
    public class UserEventBrokerTests :
        IClassFixture<AzureCloudServiceBusResource<UserEventServiceBusOptions>>
    {
        private readonly AzureCloudServiceBusResource<UserEventServiceBusOptions> _serviceBusResource;

        public UserEventBrokerTests(
            AzureCloudServiceBusResource<UserEventServiceBusOptions> serviceBusResource)
        {
            _serviceBusResource = serviceBusResource;
        }


        [Fact]
        public async Task SendEvent_Received()
        {
            // arrange
            var ev = new UserEvent()
            {
                Type = "USER_ADDED",
                UserId = "A1"
            };
            ITopicClient topicClient = _serviceBusResource.GetTopicClient("userevents");
            var broker = new UserEventBroker(topicClient);

            //act
            await broker.SendEventAsync(ev);

            //assert
            ISubscriptionClient subscriptionClient =
                _serviceBusResource.GetSubscriptionClient("userevents", "audit");

            var completion = new TaskCompletionSource<UserEvent>();

            subscriptionClient.RegisterMessageHandler((message, ct) =>
            {
                var json = Encoding.UTF8.GetString(message.Body);
                UserEvent ev = JsonSerializer.Deserialize<UserEvent>(json);
                completion.SetResult(ev);
                return Task.CompletedTask;

            }, new MessageHandlerOptions(ExceptionReceivedHandler));

            UserEvent reveivedEvent = await completion.Task;
            reveivedEvent.Should().BeEquivalentTo(ev);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs args)
        {

            return Task.CompletedTask;
        }

    }
}
