using System;
using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;

namespace Squadron
{
    public class AzureStorageModel
    {
        public string Name { get; set; }

        public IEnumerable<BlobContainer> BlobContainers { get; set; }

        internal AzureResourceProvisioningMode ProvisioningMode { get; set; }
            = AzureResourceProvisioningMode.UseExisting;
    }

    public class BlobContainer
    {
        public string Name { get; set; }

        public string CreatedName { get; set; }

        public bool IsLegalHold { get; set; }
    }
}
