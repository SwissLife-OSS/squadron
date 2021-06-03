using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventHub;
using Microsoft.Azure.Management.EventHub.Models;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Rest;
using Squadron.AzureCloud;
using SkuName = Microsoft.Azure.Management.EventHub.Models.SkuName;

namespace Squadron
{
    internal sealed class EventHubManager
    {
        private IEventHubManagementClient _client;
        private readonly AzureCredentials _azureCredentials;
        private readonly AzureResourceIdentifier _azureResourceIdentifier;

        public EventHubManager(AzureCredentials azureConfigCredentials, AzureResourceIdentifier azureResourceIdentifier)
        {
            _azureCredentials = azureConfigCredentials ?? throw new ArgumentNullException(nameof(azureConfigCredentials));
            _azureResourceIdentifier = azureResourceIdentifier ?? throw new ArgumentNullException(nameof(azureResourceIdentifier));
        }

        private async Task EnsureAuthenticatedAsync()
        {
            if ( _client is null)
            {
                var tm = new AzureAdTokenManager();
                TokenCredentials token = await tm.RequestTokenAsync(_azureCredentials);
                _client = new EventHubManagementClient(token)
                {
                    SubscriptionId = _azureResourceIdentifier.SubscriptionId,
                };
            }
        }

        internal async Task<string> CreateNamespaceAsync(string location)
        {
            await EnsureAuthenticatedAsync();

            var ns = $"squadron-{Guid.NewGuid().ToString("N").Substring(8)}";
            var pars = new EHNamespace
            {
                Sku = new Sku(SkuName.Standard),
                Location = location
            };

            EHNamespace res = await _client.Namespaces
                .CreateOrUpdateAsync(_azureResourceIdentifier.ResourceGroupName, ns, pars);

            _azureResourceIdentifier.Name = res.Name;
            return res.Name;
        }

        internal async Task DeleteNamespaceAsync()
        {
            await _client.Namespaces
                .DeleteAsync(_azureResourceIdentifier.ResourceGroupName, _azureResourceIdentifier.Name);
        }
    }
}
