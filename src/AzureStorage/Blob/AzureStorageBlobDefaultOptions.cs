using System;

namespace Squadron;

/// <summary>
/// Default AzureStorage blob resource options
/// </summary>
public class AzureStorageBlobDefaultOptions
    : ContainerResourceOptions,
        IComposableResourceOption
{
    public Type ResourceType => typeof(AzureStorageBlobResource);

    /// <summary>
    /// Configure resource options
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(ContainerResourceBuilder builder)
    {
        var name = "azurite_blob";
        builder
            .Name(name)
            .Image("mcr.microsoft.com/azure-storage/azurite")
            .InternalPort(10000)
            .AddCmd(
                "azurite",
                "-l",
                "/data",
                "--blobHost",
                "0.0.0.0",
                "--queueHost",
                "0.0.0.0",
                "--tableHost",
                "0.0.0.0",
                "--skipApiVersionCheck")
            .PreferLocalImage();
    }
}