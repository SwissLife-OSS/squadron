using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.Management.ServiceBus.Models;
using Microsoft.Rest;
using Squadron.AzureCloud;

namespace Squadron
{
    internal sealed class ServiceBusManager
    {
        private IServiceBusManagementClient _client = null;
        private readonly AzureCredentials _azureCredentials;
        private readonly AzureResourceIdentifier _identifier;

        internal ServiceBusManager(AzureCredentials azureCredentials,
                                 AzureResourceIdentifier identifier)
        {
            _azureCredentials =
                azureCredentials ?? throw new ArgumentNullException(nameof(azureCredentials));
            _identifier =
                identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        private async Task EnsureAuthenticatedAsync()
        {
            if ( _client is null)
            {
                var tm = new AzureAdTokenManager();
                TokenCredentials token = await tm.RequestTokenAsync(_azureCredentials);
                _client = new ServiceBusManagementClient(token)
                {
                    SubscriptionId = _identifier.SubscriptionId,
                };
            }
        }

        internal async Task<string> CreateNamespaceAsync(string location)
        {
            await EnsureAuthenticatedAsync();

            var ns = $"squadron-{Guid.NewGuid().ToString("N").Substring(8)}";
            var pars = new SBNamespace
            {
                Sku = new SBSku(SkuName.Standard),
                Location = location
            };

            SBNamespace res = await _client.Namespaces
                    .CreateOrUpdateAsync(_identifier.ResourceGroupName, ns, pars);

            _identifier.Name = res.Name;
            return res.Name;
        }

        internal async Task<string> CreateNamespaceAsync(string location, string serviceBusNamespace)
        {
            await EnsureAuthenticatedAsync();

            var pars = new SBNamespace
            {
                Sku = new SBSku(SkuName.Standard),
                Location = location
            };

            SBNamespace res = await _client.Namespaces
                .CreateOrUpdateAsync(_identifier.ResourceGroupName, serviceBusNamespace, pars);

            _identifier.Name = res.Name;
            return res.Name;
        }

        internal async Task CreateTopic(ServiceBusTopicModel model)
        {
            await EnsureAuthenticatedAsync();
            Trace.WriteLine($"Create topic: {model.CreatedName}");
            var pars = new SBTopic()
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(30)
            };

            SBTopic topic = _client.Topics.CreateOrUpdate(
                                          _identifier.ResourceGroupName,
                                          _identifier.Name,
                                          model.CreatedName,
                                          pars);

            foreach (ServiceBusSubscriptionModel sub in model.Subscriptions)
            {
                await CreateSubscription(topic.Name,  sub);

            }
        }


        internal async Task CreateQueueAsync(string name)
        {
            var pars = new SBQueue()
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(30)
            };

             await _client.Queues.CreateOrUpdateAsync(
                                            _identifier.ResourceGroupName,
                                            _identifier.Name,
                                            name,
                                            pars);
        }

        internal async Task<string> GetConnectionString()
        {
            await EnsureAuthenticatedAsync();
            AccessKeys keys = await _client.Namespaces
                .ListKeysAsync(_identifier.ResourceGroupName,
                               _identifier.Name,
                               "RootManageSharedAccessKey");
            return keys.PrimaryConnectionString;
        }

        internal async Task DeleteTopic(string name)
        {
            await _client.Topics.DeleteAsync(
                _identifier.ResourceGroupName,
                _identifier.Name,
                name);
        }

        internal async Task DeleteQueue(string name)
        {
            await _client.Queues.DeleteAsync(
                _identifier.ResourceGroupName,
                _identifier.Name,
                name);
        }

        internal async Task DeleteNamespaceAsync()
        {
            await _client.Namespaces
                        .DeleteAsync(_identifier.ResourceGroupName, _identifier.Name);
        }



        private async Task CreateSubscription(string topic, ServiceBusSubscriptionModel model)
        {
            var pars = new SBSubscription
            {
                DefaultMessageTimeToLive = TimeSpan.FromDays(10),
                LockDuration = TimeSpan.FromMinutes(5),
                MaxDeliveryCount = 10,
            };

            SBSubscription createdSubscription = await _client.Subscriptions
                .CreateOrUpdateAsync(_identifier.ResourceGroupName,
                                     _identifier.Name,
                                     topic,
                                     model.Name,
                                     pars);

            if ( model.SqlFilter != null)
            {
                var filter = new SqlFilter(model.SqlFilter);
                var rule = new Rule { FilterType = FilterType.SqlFilter, SqlFilter = filter };

                await _client.Rules
                        .CreateOrUpdateAsync(_identifier.ResourceGroupName,
                                             _identifier.Name,
                                             topic,
                                             createdSubscription.Name,
                                             "test",
                                             rule);
            }
        }
    }
}
