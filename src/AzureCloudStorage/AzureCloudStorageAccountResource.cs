using System;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Squadron.AzureCloud;
using Xunit;

namespace Squadron
{
    /// <summary>
    /// StorageAccount resources options
    /// </summary>
    /// <seealso cref="Squadron.AzureCloud.AzureResourceOptions" />
    public abstract class AzureCloudStorageAccountOptions : AzureResourceOptions
    {
        /// <summary>
        /// Configures the ServiceBus
        /// </summary>
        /// <param name="builder">The builder.</param>
        public abstract void Configure(StorageAccountOptionsBuilder builder);
    }

    public class AzureCloudStorageAccountResource<TOptions>
        : AzureResource<TOptions>, IAsyncLifetime
            where TOptions : AzureCloudStorageAccountOptions,
                     new()
    {
        private StorageAccountManager _storageAccountManager;
        private AzureStorageModel _storageModel;

        public string ConnectionString { get; private set; }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BuildOptions();
            InitializeStorageAccountManager();

            await PrepareAccountAsync();
            await PrepareBlobContainersAsync();

            ConnectionString = await _storageAccountManager.GetConnectionString();
        }

        public BlobServiceClient CreateBlobClient()
        {
            return new BlobServiceClient(ConnectionString);
        }

        public BlobContainerClient CreateBlobContainerClient(string name)
        {
            var createdName = GetBlobContainerName(name);

            return CreateBlobClient().GetBlobContainerClient(createdName);
        }

        public string GetBlobContainerName(string name)
        {
            BlobContainer container = _storageModel.BlobContainers
                .FirstOrDefault(x => x.CreatedName == name);

            if (container == null)
                throw new InvalidOperationException($"No container with name: {name} exists");

            return container.CreatedName;
        }

        public async Task DisposeAsync()
        {
            await DeleteBlobContainersAsync();

            if (_storageModel.ProvisioningMode == AzureResourceProvisioningMode.CreateAndDelete)
            {
                await _storageAccountManager.DeleteAccountAsync();
            }
        }

        private async Task DeleteBlobContainersAsync()
        {
            if (_storageModel.BlobContainers != null)
            {
                foreach (BlobContainer container in _storageModel.BlobContainers)
                {
                    await _storageAccountManager.DeleteBlobContainerAsync(container);
                }
            }
        }

        private void BuildOptions()
        {
            var builder = StorageAccountOptionsBuilder.New();
            var options = new TOptions();
            options.Configure(builder);
            LoadResourceConfiguration(builder);
            _storageModel = builder.Build();
        }

        private void InitializeStorageAccountManager()
        {
            _storageAccountManager = new StorageAccountManager(
                    AzureConfig.Credentials,
                    new AzureResourceIdentifier
                    {
                        SubscriptionId = AzureConfig.SubscriptionId,
                        ResourceGroupName = AzureConfig.ResourceGroup,
                        Name = _storageModel.Name
                    });
        }

        private async Task PrepareAccountAsync()
        {
            if (_storageModel.Name == null)
            {
                _storageModel.ProvisioningMode = AzureResourceProvisioningMode.CreateAndDelete;
                _storageModel.Name = await _storageAccountManager.CreateAccountAsync(
                    AzureConfig.DefaultLocation);
            }
        }

        private async Task PrepareBlobContainersAsync()
        {
            if (_storageModel.BlobContainers != null)
            {
                foreach (BlobContainer container in _storageModel.BlobContainers)
                {
                    if (_storageModel.ProvisioningMode == AzureResourceProvisioningMode.UseExisting)
                    {
                        container.CreatedName = $"{container.Name}{DateTime.UtcNow.Ticks}";
                    }
                    else
                    {
                        container.CreatedName = container.Name;
                    }

                    await _storageAccountManager.CreateContainerAsync(
                        container);
                }
            }
        }
    }
}

