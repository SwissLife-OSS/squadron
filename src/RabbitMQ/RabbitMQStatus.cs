using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMQStatus"/> class.
        /// </summary>
        /// <param name="connectionString">The ConnectionString</param>
        public RabbitMQStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <inheritdoc/>
        public Task<Status> IsReadyAsync(CancellationToken cancellationToken)
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
