using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Xunit;

namespace Squadron;

/// <inheritdoc/>
public class AzureServiceBusEmulatorResource<TConfig>
    : AzureServiceBusEmulatorResource<AzureServiceBusEmulatorDefaultOptions<TConfig>, TConfig> 
    where TConfig : AzureServiceBusConfig, new();

/// <summary>
/// Represents an AzureServiceBus resource that can be used by unit tests.
/// </summary>
/// <seealso cref="IDisposable"/>
public class AzureServiceBusEmulatorResource<TOptions, TConfig> : 
    ContainerResource<TOptions>,
    IAsyncLifetime,
    IComposableResource
    where TOptions : ContainerResourceOptions, new()
{
    /// <summary>
    /// Connection string to access the Azure Service Bus
    /// </summary>
    public string ConnectionString { get; private set; }
    
    /// <summary>
    /// Azure Service Bus client
    /// </summary>
    public ServiceBusClient Client { get; private set; }
    
    /// <inheritdoc cref="IAsyncLifetime"/>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        ConnectionString =
            $"Endpoint=sb://{Manager.Instance.Address}:{Manager.Instance.HostPort}" +
            ";SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";
        
        Client = new ServiceBusClient(ConnectionString);
        await Initializer.WaitAsync(new AzureServiceBusStatus(Client));
    }
}