using System;

namespace Squadron
{
    /// <summary>
    /// Default AzureStorage queue resource options
    /// </summary>
    public class AzureStorageQueueDefaultOptions
        : ContainerResourceOptions,
        IComposableResourceOption
    {
        public Type ResourceType => typeof(AzureStorageQueueResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            var name = "azurite_queue";
            builder
                .Name(name)
                .Image("mcr.microsoft.com/azure-storage/azurite")
                .InternalPort(10001)
                .PreferLocalImage();
        }
    }
}
