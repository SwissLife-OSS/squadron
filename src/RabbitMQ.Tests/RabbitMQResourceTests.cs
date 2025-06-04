using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Squadron;
using Xunit;

namespace RabbitMQ.Tests;

public class RabbitMQResourceTests(RabbitMQResource rabbitMqResource) : IClassFixture<RabbitMQResource>
{
    [Fact]
    public void CreateConnectionFactory_SendMessage_NoError()
    {
        //Act
        ConnectionFactory factory = rabbitMqResource.CreateConnectionFactory();

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