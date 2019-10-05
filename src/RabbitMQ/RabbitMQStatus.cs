using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace Squadron
{
    /// <summary>
    /// Status checker for RabbitMQ
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public class RabbitMQStatus : IResourceStatusProvider
    {
        private readonly string _connectionString;

        public RabbitMQStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Task<Status> IsReadyAsync()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_connectionString)
            };
            using (IConnection connection = CreateConnection(factory))
            using (IModel channel = connection.CreateModel())
            {
                return Task.FromResult(new Status
                {
                    IsReady = channel.IsOpen,
                    Message = connection.ToString()
                });
            }
        }

        private static IConnection CreateConnection(
            IConnectionFactory connectionFactory)
        {
            string hostname = ((ConnectionFactory)connectionFactory).HostName;
            return connectionFactory
                .CreateConnection(new List<AmqpTcpEndpoint>
                {
                    new AmqpTcpEndpoint(
                    connectionFactory.Uri,
                    new SslOption(
                        serverName:hostname,
                        enabled: false))
                });
        }
    }
}
