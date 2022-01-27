using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Xunit;

namespace Squadron
{
    /// <inheritdoc/>
    public class AzureStorageQueueResource
        : AzureStorageQueueResource<AzureStorageQueueDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a AzureStorage queue resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class AzureStorageQueueResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        string _internalConnectionString = null;
        /// <summary>
        /// Connection string to access to queue
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            ConnectionString = CloudStorageAccountBuilder.GetForQueue(Manager.Instance);
            _internalConnectionString
                = CloudStorageAccountBuilder.GetForBlobInternal(Settings);

            await Initializer.WaitAsync(
                new AzureStorageQueueStatus(ConnectionString));
        }


        /// <summary>
        /// Creates a Queue client
        /// </summary>
        /// <returns></returns>
        public QueueServiceClient CreateQueueServiceClient()
        {
            return new QueueServiceClient(ConnectionString);
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
