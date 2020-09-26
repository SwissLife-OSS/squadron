using System;
using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using System.Threading.Tasks;
using Microsoft.Rest;
using System.Linq;

namespace Squadron
{
    internal sealed class StorageAccountManager
    {
        private StorageManagementClient _client;
        private readonly AzureCredentials _azureCredentials;
        private readonly AzureResourceIdentifier _identifier;

        internal StorageAccountManager(
            AzureCredentials azureCredentials,
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
                _client = new StorageManagementClient(token)
                {
                    SubscriptionId = _identifier.SubscriptionId,
                };
            }
        }

        internal async Task<string> CreateAccountAsync(string location)
        {
            await EnsureAuthenticatedAsync();

            var name = $"squadron{Guid.NewGuid().ToString("N").Substring(0,8)}";

            var createParams = new StorageAccountCreateParameters
            {
                Sku = new Sku(SkuName.StandardLRS),
                Location = location,
                AccessTier = AccessTier.Hot,
                Kind = "BlobStorage"
            };

            await _client.StorageAccounts.CreateAsync(
                _identifier.ResourceGroupName,
                name,
                createParams);

            _identifier.Name = name;

            return name;
        }

        internal async Task DeleteAccountAsync()
        {
            await _client.StorageAccounts.DeleteAsync(
                _identifier.ResourceGroupName,
                _identifier.Name);
        }

        internal async Task CreateContainerAsync(
            BlobContainer container)
        {
            await EnsureAuthenticatedAsync();

            var createModel = new Microsoft.Azure.Management.Storage.Models.BlobContainer
            {
                PublicAccess = PublicAccess.None,
            };

            await _client.BlobContainers.CreateAsync(
                _identifier.ResourceGroupName,
                _identifier.Name,
                container.CreatedName,
                createModel);

            if (container.IsLegalHold)
            {
                await _client.BlobContainers.SetLegalHoldAsync(
                    _identifier.ResourceGroupName,
                    _identifier.Name,
                    container.CreatedName,
                    new List<string> { "Squadron" });
            }
        }

        internal async Task DeleteBlobContainerAsync(BlobContainer container)
        {
            if (container.IsLegalHold)
            {
                await _client.BlobContainers.ClearLegalHoldAsync(
                    _identifier.ResourceGroupName,
                    _identifier.Name,
                    container.CreatedName,
                    new List<string> { "Squadron" });
            }

            await _client.BlobContainers.DeleteAsync(
                _identifier.ResourceGroupName,
                _identifier.Name,
                container.CreatedName);
        }

        internal async Task<string> GetConnectionString()
        {
            StorageAccountListKeysResult keys = await _client.StorageAccounts
                .ListKeysAsync(_identifier.ResourceGroupName, _identifier.Name);

            var key = keys.Keys.First().Value;

            var connectionString = $"DefaultEndpointsProtocol=https;" +
                $"AccountName={_identifier.Name};" +
                $"AccountKey={key};EndpointSuffix=core.windows.net";

            return connectionString;
        }
    }
}
