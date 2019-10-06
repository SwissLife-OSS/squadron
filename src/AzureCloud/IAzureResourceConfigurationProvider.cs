using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud
{

    /// <summary>
    /// Defines an Interface to provide the Azure configurations
    /// </summary>
    public interface IAzureResourceConfigurationProvider
    {
        /// <summary>
        /// Gets the azure configuration.
        /// </summary>
        /// <returns></returns>
        AzureResourceConfiguration GetAzureConfiguration();
    }
}
