using System;
using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;
using Microsoft.Azure.Management.Storage;
using Microsoft.Azure.Management.Storage.Models;
using System.Threading.Tasks;

namespace Squadron
{
    internal sealed class StorageAccountManager
    {
        private readonly StorageManagementClient _client;
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
                _client = new ServiceBusManagementClient(token)
                {
                    SubscriptionId = _identifier.SubscriptionId,
                };
            }
        }
    }
}
