using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Squadron.AzureCloud
{
    public class AzureResourceOptionsBuilder
    {
        internal Func<AzureResourceConfiguration> ConfigResolver { get;  set; }

        public AzureResourceOptionsBuilder()
        {
            ConfigResolver = AzureResourceOptions.DefaultAzureConfigurationResolver;
        }

        public AzureResourceOptionsBuilder SetConfigResolver(
            Func<AzureResourceConfiguration> resolver)
        {
            ConfigResolver = resolver;
            return this;
        }
    }


    /// <summary>
    /// Base options to use with Azure resources
    /// </summary>
    public class AzureResourceOptions 
    {
        public Func<AzureResourceConfiguration> ConfigResolver { get; set; }

        /// <summary>
        /// Gets the azure configuration using the .NET configuration system
        /// </summary>
        /// <returns></returns>
        internal static AzureResourceConfiguration DefaultAzureConfigurationResolver()
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
