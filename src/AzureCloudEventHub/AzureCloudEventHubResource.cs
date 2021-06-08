using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Squadron.AzureCloud;
using Squadron.Model;
using Xunit;

namespace Squadron
{
    public class AzureCloudEventHubResource<TOptions>
        : AzureResource<TOptions>, IAsyncLifetime
        where TOptions : AzureCloudEventHubOptions,
        new()
    {
        private EventHubNamespaceModel _eventHubModel;
        private EventHubManager _eventHubManager;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BuildOptions();
            InitializeEventHubManager();
            await PrepareNamespaceAsync();
            await PrepareEventHubsAsync();
        }

        /// <summary>
        /// Get a EventHubClient
        /// </summary>
        /// <param name="name">EventHub name</param>
        /// <returns></returns>
        public async Task<EventHubClient> GetEventHubClientAsync(string name)
        {
            var eventHubConnectionString = await _eventHubManager.GetConnectionStringAsync(name);
            return EventHubClient.CreateFromConnectionString(eventHubConnectionString);
        }

        public async Task<PartitionReceiver> GetEventHubReceiverAsync(string name, string consumerGroup = "$default")
        {
            EventHubClient client = await GetEventHubClientAsync(name);
            EventHubRuntimeInformation runtimeInfo = await client.GetRuntimeInformationAsync();

            if (runtimeInfo.PartitionCount > 1)
                throw new InvalidOperationException("No support for more then 1 partition");

            var partitionId = runtimeInfo.PartitionIds[0];

            return client.CreateReceiver(consumerGroup, partitionId, EventPosition.FromStart());
        }

        private void InitializeEventHubManager()
        {
            _eventHubManager = new EventHubManager(
                AzureConfig.Credentials,
                new AzureResourceIdentifier
                {
                    SubscriptionId = AzureConfig.SubscriptionId,
                    ResourceGroupName = AzureConfig.ResourceGroup,
                    Name = _eventHubModel.Namespace
                });
        }

        private void BuildOptions()
        {
            var builder = EventHubOptionsBuilder.New();
            var options = new TOptions();
            options.Configure(builder);
            LoadResourceConfiguration(builder);

            _eventHubModel = builder.Build();
        }

        private async Task PrepareNamespaceAsync()
        {
            if (_eventHubModel.Namespace == null)
            {
                _eventHubModel.ProvisioningMode = EventHubProvisioningMode.CreateAndDelete;
                _eventHubModel.Namespace = await
                    _eventHubManager.CreateNamespaceAsync(AzureConfig.DefaultLocation);
            }
        }

        private async Task PrepareEventHubsAsync()
        {
            foreach (EventHubModel eventHub in _eventHubModel.GetEventHubs())
            {
                await CreateEventHubAsync(eventHub);
            }
        }

        private async Task CreateEventHubAsync(EventHubModel eventHub)
        {
            if (_eventHubModel.ProvisioningMode == EventHubProvisioningMode.UseExisting)
            {
                eventHub.CreatedName = $"{eventHub.Name}_{DateTime.UtcNow.Ticks}";
            }
            else
            {
                eventHub.CreatedName = eventHub.Name;
            }
            await _eventHubManager.CreateEventHubAsync(eventHub);
        }

        public async Task DisposeAsync()
        {
            try
            {
                if (_eventHubModel.ProvisioningMode == EventHubProvisioningMode.CreateAndDelete)
                {
                    await _eventHubManager.DeleteNamespaceAsync();
                }
                else
                {
                    foreach (EventHubModel eventHub in _eventHubModel.GetEventHubs())
                    {
                        await _eventHubManager.DeleteEventHubAsync(eventHub);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error cleaning up azure resources: {ex.Message}");
            }
        }
    }
}
