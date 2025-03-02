using System;

namespace Squadron;

public class AzureServiceBusEmulatorDefaultOptions<TConfig> : 
    ContainerResourceOptions,
    IComposableResourceOption
    where TConfig : AzureServiceBusConfig, new()
{
    public Type ResourceType => typeof(AzureServiceBusEmulatorResource<TConfig>);
    
    /// <summary>
    /// Configure resource options for AzureServiceBusEmulatorDefaultOptions
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(ContainerResourceBuilder builder)
    {
        var azureServiceBusConfig = new TConfig();
        var configFile = azureServiceBusConfig.Build();
        
        builder
            .Name("asb_emulator")
            .Image("mcr.microsoft.com/azure-messaging/servicebus-emulator")
            .InternalPort(5672)
            .WaitTimeout(120)
            .AddEnvironmentVariable("ACCEPT_EULA=Y")
            .AddVolume($"{configFile}:/ServiceBus_Emulator/ConfigFiles/Config.json")
            .PreferLocalImage();
    }
}
