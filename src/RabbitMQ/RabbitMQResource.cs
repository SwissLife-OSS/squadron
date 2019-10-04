using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// Represents a RabbitMQ resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class RabbitMQResource
        : ResourceBase<RabbitMQImageSettings>, IAsyncLifetime
    {
        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task InitializeAsync()
        {
            await StartContainerAsync();

            ConnectionString = $"amqp://{Settings.Username}:{Settings.Password}@" +
                               $"{Settings.ContainerAddress}:{Settings.HostPort}/";

            await Initializer.WaitAsync(
                new RabbitMQStatus(ConnectionString), Settings);
        }

        /// <summary>
        /// Creates RabbitMQ ConnectionFactory
        /// </summary>
        /// <returns></returns>
        public ConnectionFactory CreateConnectionFactory()
        {
            return new ConnectionFactory()
            {
                Uri = new Uri(ConnectionString),
            };
        }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public async Task DisposeAsync()
        {
            await StopContainerAsync();
        }
    }
}
