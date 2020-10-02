using System;
using System.Collections.Generic;
using Squadron.AzureCloud;

namespace Squadron
{
    public class StorageAccountOptionsBuilder : AzureResourceOptionsBuilder
    {
        private List<BlobContainer> _blobContainers = new List<BlobContainer>();
        private AzureStorageModel _model = new AzureStorageModel();

        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static StorageAccountOptionsBuilder New()
            => new StorageAccountOptionsBuilder();

        public StorageAccountOptionsBuilder WithName(string name)
        {
            _model.Name = name;

            return this;
        }

        private StorageAccountOptionsBuilder()
            : base()
        { }

        public AzureStorageModel Build()
        {
            _model.BlobContainers = _blobContainers;

            return _model;
        }

        public StorageAccountOptionsBuilder AddBlobContainer(
            string name,
            bool isLegalHold = false)
        {
            _blobContainers.Add(new BlobContainer
            {
                Name = name,
                IsLegalHold = isLegalHold
            });

            return this;
        }
    }
}

