using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron.Model;

public class KeyVaultModel
{
    public string Name { get; internal set; }

    internal KeyVaultProvisioningMode ProvisioningMode { get; set; }
        = KeyVaultProvisioningMode.UseExisting;
}

/// <summary>
/// Defines ServiceBUs provisioning modes
/// </summary>
internal enum KeyVaultProvisioningMode
{
    /// <summary>
    /// Use existing KeyVault
    /// </summary>
    UseExisting,

    /// <summary>
    /// Provision and delete KeyVault
    /// </summary>
    CreateAndDelete
}