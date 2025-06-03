using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Squadron.AzureCloud;

namespace Squadron;

/// <summary>
/// ServiceBus resources options
/// </summary>
/// <seealso cref="Squadron.AzureCloud.AzureResourceOptions" />
public abstract class AzureCloudServiceBusOptions : AzureResourceOptions
{
    /// <summary>
    /// Configures the ServiceBus
    /// </summary>
    /// <param name="builder">The builder.</param>
    public abstract void Configure(ServiceBusOptionsBuilder builder);

}