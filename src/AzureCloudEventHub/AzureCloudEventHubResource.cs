using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        private EventHubModel _eventHubModel;
        private EventHubManager _eventHubManager;

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BuildOptions();
            InitializeEventHubManager();
            await PrepareNamespaceAsync();
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
            var builder = new EventHubOptionsBuilder();
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
                    throw new System.NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error cleaning up azure resources: {ex.Message}");
            }
        }
    }
}
