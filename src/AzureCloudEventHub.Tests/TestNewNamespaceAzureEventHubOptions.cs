namespace Squadron.AzureCloudEventHub.Tests
{
    public class TestNewNamespaceAzureEventHubOptions : AzureCloudEventHubOptions
    {
        public override void Configure(EventHubOptionsBuilder builder)
        {
            builder.AddEventHub("testEventHub");
        }
    }
}
