using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Squadron;
using Xunit;

namespace RabbitMQ.Tests
{
    public class RabbitMqSquadronResourceTests : ISquadronResourceFixture<RabbitMQResource>
    {
        private readonly RabbitMQResource _rabbitMQResource;

        public RabbitMqSquadronResourceTests(SquadronResource<RabbitMQResource> rabbitMQResource)
        {
            _rabbitMQResource = rabbitMQResource.Resource;
        }

        [Fact]
        public void CreateConnectionFactory_SendMessage_NoError()
        {
            //Act
            ConnectionFactory factory = _rabbitMQResource.CreateConnectionFactory();

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "foo",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

                channel.BasicPublish(exchange: "",
                             routingKey: "bar",
                             basicProperties: null,
                             body: Encoding.UTF8.GetBytes("Hello RabbitMQ"));
            }
        }
    }
}
