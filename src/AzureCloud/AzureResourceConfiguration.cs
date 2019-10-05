using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud
{
    /// <summary>
    /// AzureCloud configuration
    /// </summary>
    public class AzureResourceConfiguration
    {
        /// <summary>
        /// Azure Subsccription
        /// </summary>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// ResourceGroup name where the resource well be created
        /// </summary>
        public string ResourceGroup { get; set; }

        /// <summary>
        /// Azure Credentials to use to access to management apis
        /// </summary>
        public AzureCredentials Credentials { get; set; }

        /// <summary>
        /// Azure Location where the resources will be created
        /// </summary>
        public string DefaultLocation { get; set; }
    }
}
