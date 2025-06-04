using Microsoft.Extensions.Configuration;
using Squadron.AzureCloud;

namespace Squadron.AzureKeyVault.Tests;

public class TestAzureConfigResolver
{
    internal static AzureResourceConfiguration Resolver()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true)
            .AddUserSecrets<TestAzureConfigResolver>()
            .AddEnvironmentVariables()
            .Build();

        IConfigurationSection section = configuration.GetSection("Squadron:Azure");
        AzureResourceConfiguration azureConfig = section.Get<AzureResourceConfiguration>();
        return azureConfig;
    }
}