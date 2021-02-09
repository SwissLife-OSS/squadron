using Microsoft.Azure.Storage;

namespace Squadron
{
    internal static class CloudStorageAccountBuilder
    {
        internal static string GetForBlob(ContainerInstance instance)
        {
            return BuildConnectionString("BlobEndpoint", instance);
        }

        internal static string GetForQueue(ContainerInstance instance)
        {
            return BuildConnectionString("QueueEndpoint", instance);
        }

        private static string BuildConnectionString(string endpoint, ContainerInstance instance)
        {
            CloudStorageAccount dev = CloudStorageAccount.DevelopmentStorageAccount;
            return 
                $"DefaultEndpointsProtocol=http;AccountName={dev.Credentials.AccountName};" +
                $"AccountKey={dev.Credentials.ExportBase64EncodedKey()};" +
                $"{endpoint}=http://{instance.IpAddress}:{instance.HostPort}/" +
                $"{dev.Credentials.AccountName};";
        }

    }
}
