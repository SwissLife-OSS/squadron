using Azure.Messaging.ServiceBus;
using SharpCompress;

namespace Squadron;

public class AzureServiceBusResources : 
    AzureServiceBusResources<AzureServiceBusConfig>;

public class AzureServiceBusResources<TConfig> : 
    ComposeResource<AzureServiceBusDefaultOptions<TConfig>> 
    where TConfig : AzureServiceBusConfig, new()
{
    private readonly Lazy<AzureServiceBusEmulatorResource<TConfig>> _azureServiceBusEmulatorResource;

    public AzureServiceBusResources()
    {
        _azureServiceBusEmulatorResource = new Lazy<AzureServiceBusEmulatorResource<TConfig>>(() =>
            (Managers[AzureServiceBusConstants.AzureServiceBusEmulatorResourceName].Resource
                as AzureServiceBusEmulatorResource<TConfig>)!);
    }
    
    public string ConnectionString => _azureServiceBusEmulatorResource.Value.ConnectionString!;
    
    public ServiceBusClient Client => _azureServiceBusEmulatorResource.Value.Client!;
}
