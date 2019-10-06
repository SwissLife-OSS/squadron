namespace Squadron
{
    public class AzureStorageQueueDefaultOptions : ContainerResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            var name = "azurite_queue";
            builder
                .Name(name)
                .Image("mcr.microsoft.com/azure-storage/azurite")
                .InternalPort(10001);
        }
    }
}
