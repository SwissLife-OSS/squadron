using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud
{
    public interface IAzureResourceConfigurationProvider
    {
        AzureResourceConfiguration GetAzureConfiguration();
    }
}
