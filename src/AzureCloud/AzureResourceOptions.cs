using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Squadron.AzureCloud
{
    /// <summary>
    /// Base options to use with Azure resources
    /// </summary>
    /// <seealso cref="Squadron.AzureCloud.IAzureResourceConfigurationProvider" />
    public class AzureResourceOptions : IAzureResourceConfigurationProvider
    {

        /// <summary>
        /// Gets the azure configuration using the .NET configuration system
        /// </summary>
        /// <returns></returns>
        public virtual AzureResourceConfiguration GetAzureConfiguration()
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
