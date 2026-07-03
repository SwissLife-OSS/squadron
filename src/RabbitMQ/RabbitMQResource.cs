using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Xunit;

namespace Squadron;

/// <inheritdoc/>
public class RabbitMQResource : RabbitMQResource<RabbitMQDefaultOptions>
{

}


/// <summary>
/// Represents a RabbitMQ resource that can be used by unit tests.
/// </summary>
/// <seealso cref="IDisposable"/>
public class RabbitMQResource<TOptions>
    : ContainerResource<TOptions>,
        IAsyncLifetime
    where TOptions : ContainerResourceOptions, new()
{ 
    /// <summary>
    /// Connection string to access to queue
    /// </summary>
    public string ConnectionString { get; private set; }

    /// <inheritdoc cref="IAsyncLifetime"/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        ConnectionString = $"amqp://{Settings.Username}:{Settings.Password}@" +
                           $"{Manager.Instance.Address}:{Manager.Instance.HostPort}/";

        await Initializer.WaitAsync(
            new RabbitMQStatus(ConnectionString));
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
}