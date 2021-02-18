using System;
using System.Threading.Tasks;
using Squadron.AzureCloud;
using Squadron.Model;
using Xunit;

namespace Squadron
{
    public class AzureKeyVaultResource<TOptions>
            : AzureResource<TOptions>, IAsyncLifetime
        where TOptions : AzureKeyVaultOptions, new()
    {
        private KeyVaultModel _keyVaultModel;
        private KeyVaultManager _keyVaultManager;

        public Uri VaultUri { get; private set; }

        /// <summary>
        /// Initialize the resource
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BuildOptions();
            InitializeKeyVaultManager();
            await PrepareKeyVaultAsync();
        }

        private void InitializeKeyVaultManager()
        {
            _keyVaultManager = new KeyVaultManager(
                    AzureConfig.Credentials,
                    new AzureResourceIdentifier
                    {
                        SubscriptionId = AzureConfig.SubscriptionId,
                        ResourceGroupName = AzureConfig.ResourceGroup,
                        Name = _keyVaultModel.Name
                    });
        }

        private async Task PrepareKeyVaultAsync()
        {
            if (_keyVaultModel.Name == null)
            {
                _keyVaultModel.ProvisioningMode = KeyVaultProvisioningMode.CreateAndDelete;
                _keyVaultModel.Name = await
                    _keyVaultManager.CreateAsync(AzureConfig.DefaultLocation);
            }

            VaultUri = await _keyVaultManager.GetUriAsync();
        }

        public async Task DisposeAsync()
        {
            if (_keyVaultModel.ProvisioningMode == KeyVaultProvisioningMode.CreateAndDelete)
            {
                await _keyVaultManager.DeleteAsync(_keyVaultModel.Name);
            }
        }

        private void BuildOptions()
        {
            var builder = KeyVaultOptionsBuilder.New();
            var options = new TOptions();
            options.Configure(builder);
            LoadResourceConfiguration(builder);
            _keyVaultModel = builder.Build();
        }
    }
}
