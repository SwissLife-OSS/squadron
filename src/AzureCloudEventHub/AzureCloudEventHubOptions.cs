using Squadron.AzureCloud;

namespace Squadron;

public abstract class AzureCloudEventHubOptions : AzureResourceOptions
{
    /// <summary>
    /// Configures the EventHub
    /// </summary>
    /// <param name="builder">The builder.</param>
    public abstract void Configure(EventHubOptionsBuilder builder);
}