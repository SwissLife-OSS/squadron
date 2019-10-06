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
            var options = new TOptions();
            AzureConfig = options.GetAzureConfiguration();
            Trace.WriteLine("Loading Azure Configuration");
            return Task.CompletedTask;
        }
    }

}
