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

        public Task DisposeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
