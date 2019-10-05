using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Squadron.AzureCloud
{
    public class AzureResourceOptions : IAzureResourceConfigurationProvider
    {
        public AzureResourceConfiguration GetAzureConfiguration()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.user.json", true)
                .AddEnvironmentVariables()
                .Build();

            IConfigurationSection section = configuration.GetSection("Squadron:Azure");

            AzureResourceConfiguration azureConfig = section.Get<AzureResourceConfiguration>();
            return azureConfig;
        }
    }
}
