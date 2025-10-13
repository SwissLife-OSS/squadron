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

        public async Task SendEventAsync(OrderEvent @event)
        {
            await using IConnection connection = await _connectionFactory.CreateConnectionAsync();
            await using IChannel channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                QueueName,
                false,
                false,
                false,
                null);

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: BuildMessage(@event));
        }

        private static byte[] BuildMessage(OrderEvent @event)
        {
            var json = JsonSerializer.Serialize(@event);
            var msg = Encoding.UTF8.GetBytes(json);
            return msg;
        }
    }
}
