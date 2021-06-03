using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventHub;
using Microsoft.Rest;
using Squadron.AzureCloud;

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
    }
}
