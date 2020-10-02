using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.AzureCloud
{
    /// <summary>
    /// Defines ServiceBUs provisioning modes
    /// </summary>
    public enum AzureResourceProvisioningMode
    {
        /// <summary>
        /// Use an existing Azure resource
        /// </summary>
        UseExisting,

        /// <summary>
        /// Provision and delete resource
        /// </summary>
        CreateAndDelete
    }
}
