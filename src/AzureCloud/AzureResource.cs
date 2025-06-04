using System.Threading.Tasks;

namespace Squadron.AzureCloud;

/// <summary>
/// Base class to use with Azure Cloud resources
/// </summary>
/// <typeparam name="TOptions"></typeparam>
public class AzureResource<TOptions>
    where TOptions : AzureResourceOptions, new()
{
    /// <summary>
    /// Azure configuration to work with Azure management api
    /// </summary>
    protected AzureResourceConfiguration AzureConfig { get; private set; }

    /// <summary>
    /// Initialize the resource
    /// </summary>
    /// <returns></returns>
    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Loads the azure resource configuration.
    /// </summary>
    /// <param name="builder">The builder.</param>
    protected void LoadResourceConfiguration(AzureResourceOptionsBuilder builder)
    {
        AzureConfig = builder.ConfigResolver();
    }
}