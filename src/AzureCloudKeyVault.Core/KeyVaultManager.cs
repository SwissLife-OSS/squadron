using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.KeyVault;
using Microsoft.Azure.Management.KeyVault.Models;
using Microsoft.Rest;
using Squadron.AzureCloud;

namespace Squadron;

public class KeyVaultManager
{
    IKeyVaultManagementClient _client = null;
    private readonly AzureCredentials _azureCredentials;
    private readonly AzureResourceIdentifier _identifier;

    internal KeyVaultManager(AzureCredentials azureCredentials,
        AzureResourceIdentifier identifier)
    {
        _azureCredentials =
            azureCredentials ?? throw new ArgumentNullException(nameof(azureCredentials));
        _identifier =
            identifier ?? throw new ArgumentNullException(nameof(identifier));
    }

    private async Task EnsureAuthenticatedAsync()
    {
        if (_client is null)
        {
            var tm = new AzureAdTokenManager();
            TokenCredentials token = await tm.RequestTokenAsync(_azureCredentials);
            _client = new KeyVaultManagementClient(token)
            {
                SubscriptionId = _identifier.SubscriptionId,
            };
        }
    }

    public async Task<string> CreateAsync(string location)
    {
        await EnsureAuthenticatedAsync();
        var name = $"squadron-{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        _identifier.Name = name;

        await _client.Vaults.CreateOrUpdateAsync(
            _identifier.ResourceGroupName,
            name, new VaultCreateOrUpdateParameters
            {
                Location = location,
                Properties = new VaultProperties()
                {
                    TenantId = Guid.Parse(_azureCredentials.TenantId),
                    Sku = new Sku(SkuName.Standard),
                    AccessPolicies = new[] { new AccessPolicyEntry(
                        Guid.Parse(_azureCredentials.TenantId),
                        _azureCredentials.ClientId,
                        new Permissions(null, new[] { "all" })) }
                }
            });

        return name;
    }

    public async Task<Uri> GetUriAsync()
    {
        Vault vault = await _client.Vaults.GetAsync(
            _identifier.ResourceGroupName,
            _identifier.Name);

        return new Uri(vault.Properties.VaultUri);
    }

    internal async Task DeleteAsync(string name)
    {
        await EnsureAuthenticatedAsync();
        await _client.Vaults.DeleteAsync(_identifier.ResourceGroupName, name);
    }
}