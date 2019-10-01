using Microsoft.Azure.Storage;

namespace Squadron
{
    internal static class CloudStorageAccountBuilder
    {
        internal static CloudStorageAccount GetForBlob(IImageSettings settings)
        {
            return GetAccountByEndpoint("BlobEndpoint", settings);
        }

        internal static CloudStorageAccount GetForQueue(IImageSettings settings)
        {
            return GetAccountByEndpoint("QueueEndpoint", settings);
        }

        private static CloudStorageAccount GetAccountByEndpoint(
                string endpoint,
                IImageSettings settings)
        {
            CloudStorageAccount dev = CloudStorageAccount.DevelopmentStorageAccount;
            return CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=http;AccountName={dev.Credentials.AccountName};" +
                $"AccountKey={dev.Credentials.ExportBase64EncodedKey()};" +
                $"{endpoint}=http://{settings.ContainerAddress}:{settings.HostPort}/" +
                $"{dev.Credentials.AccountName};");
        }
    }
}
