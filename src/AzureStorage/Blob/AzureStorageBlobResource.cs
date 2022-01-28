using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class AzureStorageBlobResource
        : AzureStorageBlobResource<AzureStorageBlobDefaultOptions>
    {
    }

    /// <summary>
    /// Represents a AzureStorage blob resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageBlobResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime,
          IComposableResource
        where TOptions : ContainerResourceOptions, new()
    {
        string _internalConnectionString = null;
        /// <summary>
        /// Connection string to the blob
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = CloudStorageAccountBuilder.GetForBlob(Manager.Instance);
            _internalConnectionString
                = CloudStorageAccountBuilder.GetForBlobInternal(Settings);

            await Initializer.WaitAsync(
                new AzureStorageBlobStatus(ConnectionString));
        }

        /// <summary>
        /// Creates a Blob client
        /// </summary>
        /// <returns></returns>
        public BlobServiceClient CreateBlobServiceClient()
        {
            return new BlobServiceClient(ConnectionString);
        }

        public override Dictionary<string, string> GetComposeExports()
        {
            Dictionary<string, string> exports = base.GetComposeExports();
            exports.Add("CONNECTIONSTRING", ConnectionString);
            exports.Add("CONNECTIONSTRING_INTERNAL", _internalConnectionString);

            return exports;
        }
    }
}
