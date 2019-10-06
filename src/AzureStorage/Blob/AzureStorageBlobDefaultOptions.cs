namespace Squadron
{
    public class AzureStorageBlobDefaultOptions : ContainerResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            var name = "azurite_blob";
            builder
                .Name(name)
                .Image("mcr.microsoft.com/azure-storage/azurite")
                .InternalPort(10000);
        }
    }
}
