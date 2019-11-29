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

        public OrderBrokerTests(
            RabbitMQResource<RabbitMQDefaultOptions> rabbitMQResource)
        {
            _rabbitMQResource = rabbitMQResource;
        }

        [Fact]
        public async Task SendEvent_Received()
        {
            // arrange
            var ev = new OrderEvent()
            {
                ProductId = "Product 1",
                Price = 100.01m
            };

            IConnectionFactory connectionFactory = _rabbitMQResource.CreateConnectionFactory();
            var broker = new OrderEventBroker(connectionFactory);

            //act
            broker.SendEvent(ev);

            OrderEvent resultEvent = null;

            using IConnection connection = connectionFactory.CreateConnection();
            using IModel channel = connection.CreateModel();
            channel.QueueDeclare(
                OrderEventBroker.QueueName,
                false,
                false,
                false,
                null);

            BasicGetResult queueResult = channel.BasicGet(OrderEventBroker.QueueName, true);
            if (queueResult != null)
            {
                var json = Encoding.UTF8.GetString(queueResult.Body);
                resultEvent = JsonSerializer.Deserialize<OrderEvent>(json);
            }

            //assert
            resultEvent.Should().BeEquivalentTo(ev);
        }
    }
}
