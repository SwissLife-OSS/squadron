using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.Management.EventHub;
using Microsoft.Azure.Management.EventHub.Models;
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
            res.KafkaEnabled = true;

            _azureResourceIdentifier.Name = res.Name;
            return res.Name;
        }

        internal async Task DeleteNamespaceAsync()
        {
            await _client.Namespaces
                .DeleteAsync(_azureResourceIdentifier.ResourceGroupName, _azureResourceIdentifier.Name);
        }

        public async Task CreateEventHubAsync(EventHubModel eventHub)
        {
            await EnsureAuthenticatedAsync();
            Trace.WriteLine($"Create event hub: {eventHub.CreatedName}");

            var newEventHub = new Eventhub{MessageRetentionInDays = 1, PartitionCount = 1};
            await _client.EventHubs.CreateOrUpdateAsync(_azureResourceIdentifier.ResourceGroupName,
                _azureResourceIdentifier.Name, eventHub.CreatedName, newEventHub);

            var autRuleParams = new AuthorizationRule {Rights = new List<string> {"send"}};
            await _client.EventHubs.CreateOrUpdateAuthorizationRuleAsync(_azureResourceIdentifier.ResourceGroupName,
                _azureResourceIdentifier.Name, eventHub.CreatedName, "sender", autRuleParams);
        }

        public async Task DeleteEventHubAsync(EventHubModel eventHub)
        {
            await EnsureAuthenticatedAsync();
            Trace.WriteLine($"Deletes event hub: {eventHub.CreatedName}");

            await _client.EventHubs.DeleteAsync(_azureResourceIdentifier.ResourceGroupName,
                _azureResourceIdentifier.Name, eventHub.CreatedName);
        }

        public async Task<string> GetConnectionStringAsync()
        {
            await EnsureAuthenticatedAsync();
            AccessKeys keys = await _client.Namespaces
                .ListKeysAsync(_azureResourceIdentifier.ResourceGroupName,
                    _azureResourceIdentifier.Name,
                    "RootManageSharedAccessKey");
            return keys.PrimaryConnectionString;
        }

        public async Task<string> GetConnectionStringAsync(string eventHub)
        {
            await EnsureAuthenticatedAsync();

            var responseKeys = await _client.EventHubs.ListKeysWithHttpMessagesAsync(_azureResourceIdentifier.ResourceGroupName,
                _azureResourceIdentifier.Name, eventHub, "sender");

            var keys = responseKeys.Body;

            var eventHubNamespaceUri = new Uri($"sb://{_azureResourceIdentifier.Name}.servicebus.windows.net");
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(eventHubNamespaceUri, eventHub, keys.KeyName, keys.PrimaryKey);
            return connectionStringBuilder.ToString();
        }
    }
}
