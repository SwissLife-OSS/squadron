using System;

namespace Squadron;

public class AzureServiceBusEmulatorDefaultOptions : 
    ContainerResourceOptions,
    IComposableResourceOption
{
    public Type ResourceType => typeof(AzureServiceBusEmulatorResource);

    /// <summary>
    /// Configure resource options for AzureServiceBusEmulatorDefaultOptions
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(ContainerResourceBuilder builder)
    {
        builder
            .Name("asb_emulator")
            .Image("mcr.microsoft.com/azure-messaging/servicebus-emulator")
            .InternalPort(5672)
            .WaitTimeout(120)
            .AddEnvironmentVariable("ACCEPT_EULA=Y")
            .PreferLocalImage();
    }
}
