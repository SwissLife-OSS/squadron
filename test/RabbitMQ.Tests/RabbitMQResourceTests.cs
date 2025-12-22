using System.Threading.Tasks;
using RabbitMQ.Client;
using Squadron;
using Xunit;

namespace RabbitMQ.Tests;

public class RabbitMQResourceTests(RabbitMQResource rabbitMqResource) : IClassFixture<RabbitMQResource>
{
    [Fact]
    public async Task CreateConnectionFactory_SendMessage_NoError()
    {
        //Act
        ConnectionFactory factory = rabbitMqResource.CreateConnectionFactory();

        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(queue: "foo",
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: "bar",
            mandatory: false,
            basicProperties: new BasicProperties(),
            body: "Hello RabbitMQ"u8.ToArray());
    }
}