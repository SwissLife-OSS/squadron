using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Squadron;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Squadron.Samples.RabbitMQ
{
    public class OrderBrokerTests : IClassFixture<RabbitMQResource<RabbitMQDefaultOptions>>
    {
        private readonly RabbitMQResource<RabbitMQDefaultOptions> _rabbitMQResource;

        public OrderBrokerTests(RabbitMQResource<RabbitMQDefaultOptions> rabbitMQResource)
        {
            _rabbitMQResource = rabbitMQResource;
        }

        [Fact]
        public async Task SendEvent_Received()
        {
            // Arrange
            var ev = new OrderEvent()
            {
                ProductId = "Product 1",
                Price = 100.01m
            };

            IConnectionFactory connectionFactory = _rabbitMQResource.CreateConnectionFactory();
            var broker = new OrderEventBroker(connectionFactory);

            // Act
            await broker.SendEventAsync(ev);

            OrderEvent resultEvent = null;

            await using IConnection connection = await connectionFactory.CreateConnectionAsync();
            await using IChannel channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                OrderEventBroker.QueueName,
                false,
                false,
                false,
                null);

            BasicGetResult queueResult = await channel.BasicGetAsync(OrderEventBroker.QueueName, true);
            if (queueResult != null)
            {
                var json = Encoding.UTF8.GetString(queueResult.Body.Span);
                resultEvent = JsonSerializer.Deserialize<OrderEvent>(json);
            }

            //Assert
            resultEvent.Should().BeEquivalentTo(ev);
        }
    }
}
