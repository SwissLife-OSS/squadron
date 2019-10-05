using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Squadron.AzureCloud;

namespace Squadron
{
    public abstract class AzureCloudServiceBusOptions : AzureResourceOptions
    {
        public abstract void Configure(ServiceBusOptionsBuilder builder);
    }
}
