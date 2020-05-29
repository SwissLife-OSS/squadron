using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Squadron.Samples.RabbitMQ
{
    public class OrderEventBroker
    {
        public static string QueueName => "neworders";

        private readonly IConnectionFactory _connectionFactory;

        public OrderEventBroker(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void SendEvent(OrderEvent @event)
        {
            using IConnection connection = _connectionFactory.CreateConnection();
            using IModel channel = connection.CreateModel();
            channel.QueueDeclare(
                QueueName,
                false,
                false,
                false,
                null);

            channel.BasicPublish(
                string.Empty,
                QueueName,
                null,
                BuildMessage(@event));
        }

        private static byte[] BuildMessage(OrderEvent @event)
        {
            var json = JsonSerializer.Serialize(@event);
            var msg = Encoding.UTF8.GetBytes(json);
            return msg;
        }
    }
}
