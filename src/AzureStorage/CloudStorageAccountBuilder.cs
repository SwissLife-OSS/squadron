using Microsoft.Azure.Storage;

namespace Squadron
{
    internal static class CloudStorageAccountBuilder
    {
        internal static string GetForBlob(ContainerInstance instance)
        {
            return BuildConnectionString("BlobEndpoint", instance);
        }

        internal static string GetForBlobInternal(
            ContainerInstance instance,
            ContainerResourceSettings settings)
        {
            return BuildInternalConnectionString("BlobEndpoint", instance, settings);
        }

        internal static string GetForQueue(ContainerInstance instance)
        {
            return BuildConnectionString("QueueEndpoint", instance);
        }

        internal static string GetForQueueInternal(
            ContainerInstance instance,
            ContainerResourceSettings settings)
        {
            return BuildInternalConnectionString("QueueEndpoint", instance, settings);
        }

        private static string BuildConnectionString(string endpoint, ContainerInstance instance)
        {
            CloudStorageAccount dev = CloudStorageAccount.DevelopmentStorageAccount;
            return
                $"DefaultEndpointsProtocol=http;AccountName={dev.Credentials.AccountName};" +
                $"AccountKey={dev.Credentials.ExportBase64EncodedKey()};" +
                $"{endpoint}=http://{instance.Address}:{instance.HostPort}/" +
                $"{dev.Credentials.AccountName};";
        }

        private static string BuildInternalConnectionString(
            string endpoint,
            ContainerInstance instance,
            ContainerResourceSettings settings)
        {
            CloudStorageAccount dev = CloudStorageAccount.DevelopmentStorageAccount;
            return
                $"DefaultEndpointsProtocol=http;AccountName={dev.Credentials.AccountName};" +
                $"AccountKey={dev.Credentials.ExportBase64EncodedKey()};" +
                $"{endpoint}=http://{instance.Address}:{settings.InternalPort}/" +
                $"{dev.Credentials.AccountName};";
        }

    }
}
