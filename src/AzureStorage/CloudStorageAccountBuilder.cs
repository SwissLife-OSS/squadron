using Microsoft.Azure.Storage;

namespace Squadron
{
    internal static class CloudStorageAccountBuilder
    {
        internal static string GetForBlob(IImageSettings settings)
        {
            return BuildConnectionString("BlobEndpoint", settings);
        }

        internal static string GetForQueue(IImageSettings settings)
        {
            return BuildConnectionString("QueueEndpoint", settings);
        }


        private static string BuildConnectionString(string endpoint, IImageSettings settings)
        {
            CloudStorageAccount dev = CloudStorageAccount.DevelopmentStorageAccount;
            return 
                $"DefaultEndpointsProtocol=http;AccountName={dev.Credentials.AccountName};" +
                $"AccountKey={dev.Credentials.ExportBase64EncodedKey()};" +
                $"{endpoint}=http://{settings.ContainerAddress}:{settings.HostPort}/" +
                $"{dev.Credentials.AccountName};";
        }

    }
}
