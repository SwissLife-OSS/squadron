using System;
using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;
using Xunit;

namespace Squadron
{

    public class StorageAccountOptionsBuilder : AzureResourceOptionsBuilder
    {
        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static StorageAccountOptionsBuilder New()
            => new StorageAccountOptionsBuilder();

        private StorageAccountOptionsBuilder()
            : base()
        {

        }
    }

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
        public abstract void Configure(ServiceBusOptionsBuilder builder);
    }

    public class AzureCloudStorageAccountResource<TOptions>
        : AzureResource<TOptions>, IAsyncLifetime
            where TOptions : AzureCloudStorageAccountOptions,
                     new()
    {

    }

}

