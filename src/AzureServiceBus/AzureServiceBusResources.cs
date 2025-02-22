using Azure.Messaging.ServiceBus;
using SharpCompress;

namespace Squadron;

public class AzureServiceBusResources : ComposeResource<AzureServiceBusDefaultOptions>
{
    private readonly Lazy<AzureServiceBusEmulatorResource> _azureServiceBusEmulatorResource;

    public AzureServiceBusResources()
    {
        _azureServiceBusEmulatorResource = new Lazy<AzureServiceBusEmulatorResource>(() =>
            (Managers[AzureServiceBusDefaultOptions.AzureServiceBusEmulatorResourceName].Resource
                as AzureServiceBusEmulatorResource)!);
    }
    
    public string ConnectionString => _azureServiceBusEmulatorResource.Value.ConnectionString!;
    
    public ServiceBusClient Client => _azureServiceBusEmulatorResource.Value.Client!;
}
