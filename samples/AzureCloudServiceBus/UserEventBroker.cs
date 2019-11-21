using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Squadron.Samples.AzureCloud.ServiceBus
{
    public class UserEventBroker
    {
        private readonly ITopicClient _topicClient;

        public UserEventBroker(ITopicClient topicClient)
        {
            _topicClient = topicClient;
        }


        public async Task SendEventAsync(UserEvent @event)
        {
            Message msg = BuildMessage(@event);
            await _topicClient.SendAsync(msg);
        }

        private static Message BuildMessage(UserEvent @event)
        {
            var json = JsonSerializer.Serialize(@event);
            var msg = new Message(Encoding.UTF8.GetBytes(json));
            msg.UserProperties.Add("EventType", @event.Type);
            return msg;
        }
    }
}
