using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using Xunit;

namespace Squadron
{

    /// <inheritdoc/>
    public class ElasticsearchResource
        : ElasticsearchResource<ElasticsearchDefaultOptions>
    {
    }

    /// <summary>
    /// Represents a elasticsearch database resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class ElasticsearchResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            Uri uri = new Uri($"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}");
            var connectionSettings = new ConnectionSettings(uri);
            connectionSettings.EnableDebugMode();
            connectionSettings.DisableDirectStreaming();

            Client = new ElasticClient(connectionSettings);

            await Initializer.WaitAsync(
                new ElasticsearchStatus(Client));
        }

        /// <summary>
        /// Gets the elasticsearch client connected
        /// to elasticsearch local container.
        /// </summary>
        public IElasticClient Client { get; private set; }

        /// <summary>
        /// Creates the index asynchronous.
        /// </summary>
        public async Task<string> CreateIndexAsync<TIndexType>()
            where TIndexType : class
        {
            string indexName = Guid.NewGuid().ToString();
            ICreateIndexResponse createIndexResponse = await Client.CreateIndexAsync(
                index: indexName,
                selector: descriptor => descriptor
                    .Mappings(mappingsDescriptor => mappingsDescriptor
                        .Map<TIndexType>(mappingDescriptor => mappingDescriptor
                            .AutoMap<TIndexType>())));

            if (!createIndexResponse.IsValid)
            {
                throw new InvalidOperationException(
                    $"Could not create the index: {createIndexResponse.DebugInformation}",
                    createIndexResponse.OriginalException);
            }

            return indexName;
        }

        /// <summary>
        /// Creates documents on to the given index
        /// </summary>
        public async Task CreateDocumentsAsync<TDocument>(string index,
            params CreateDocumentRequest<TDocument>[] requests)
            where TDocument : class, new()
        {
            if (string.IsNullOrEmpty(index))
            {
                throw new ArgumentNullException(nameof(index));
            }

            if (requests.Length < 1)
            {
                throw new ArgumentException(
                    "At least one document request is needed to create something");
            }

            IEnumerable<BulkCreateDescriptor<TDocument>> createDescriptors = requests
                .Select(request => new BulkCreateDescriptor<TDocument>()
                    .Document(request.Document)
                    .Id(request.Id));

            BulkDescriptor bulkDescriptor = new BulkDescriptor()
                .Index(index);

            foreach (BulkCreateDescriptor<TDocument> bulkCreateDescriptor in createDescriptors)
            {
                bulkDescriptor.AddOperation(bulkCreateDescriptor);
            }

            BulkResponse bulkResponse = await Client.LowLevel.BulkPutAsync<BulkResponse>(
                index, typeof(TDocument).Name.ToLowerInvariant(), PostData.Serializable(bulkDescriptor));
            if (!bulkResponse.IsValid)
            {
                throw new InvalidOperationException(
                    $"Could not create the documents: {bulkResponse.DebugInformation}",
                    bulkResponse.OriginalException);
            }

            await Client.RefreshAsync(Indices.All);
        }

        /// <summary>
        /// Create a alias for the given indices.
        /// </summary>
        public async Task<string> MergeIndicesAsync(params string[] indices)
        {
            if (indices.Length < 2)
            {
                throw new ArgumentException(
                    "At least two indices are needed to create a merged index");
            }

            string alias = Guid.NewGuid().ToString();

            IEnumerable<AliasAddDescriptor> createDescriptors = indices
                .Select(index => new AliasAddDescriptor()
                    .Alias(alias)
                    .Index(index));

            BulkAliasDescriptor aliasDescriptor = new BulkAliasDescriptor();

            foreach (AliasAddDescriptor aliasAddDescriptor in createDescriptors)
            {
                aliasDescriptor.Add(aliasAddDescriptor);
            }

            IBulkAliasResponse bulkAliasResponse = await Client.AliasAsync(aliasDescriptor);
            if (!bulkAliasResponse.IsValid)
            {
                throw new InvalidOperationException(
                    $"Could not create alias for the indices \"{string.Join(",", indices)}\": {bulkAliasResponse.DebugInformation}",
                    bulkAliasResponse.OriginalException);
            }

            await Client.RefreshAsync(Indices.All);
            return alias;
        }
    }
}
