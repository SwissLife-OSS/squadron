using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Squadron.AzureCloud
{
    /// <summary>
    /// Base class to use with Azure Cloud resources
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    public class AzureResource<TOptions>
        where TOptions : AzureResourceOptions, IAzureResourceConfigurationProvider, new()
    {
        protected AzureResourceConfiguration AzureConfig { get; private set; }

        public virtual Task InitializeAsync()
        {
            var options = new TOptions();
            AzureConfig = options.GetAzureConfiguration();
            Trace.WriteLine("Loading Azure Configuration");
            return Task.CompletedTask;
        }

    }

}
