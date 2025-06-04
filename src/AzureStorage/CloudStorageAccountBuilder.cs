namespace Squadron;

internal static class CloudStorageAccountBuilder
{

    private const string DevStorageAccountName = "devstoreaccount1";
    private const string DevKey=
        "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    internal static string GetForBlob(ContainerInstance instance)
    {
        return BuildConnectionString("BlobEndpoint", instance);
    }

    internal static string GetForBlobInternal(
        ContainerResourceSettings settings)
    {
        return BuildInternalConnectionString("BlobEndpoint", settings);
    }

    internal static string GetForQueue(ContainerInstance instance)
    {
        return BuildConnectionString("QueueEndpoint", instance);
    }

    internal static string GetForQueueInternal(
        ContainerResourceSettings settings)
    {
        return BuildInternalConnectionString("QueueEndpoint", settings);
    }

    private static string BuildConnectionString(string endpoint, ContainerInstance instance)
    {
        return
            $"DefaultEndpointsProtocol=http;AccountName={DevStorageAccountName};" +
            $"AccountKey={DevKey};" +
            $"{endpoint}=http://127.0.0.1:{instance.HostPort}/" +
            $"{DevStorageAccountName};";
    }

    private static string BuildInternalConnectionString(
        string endpoint,
        ContainerResourceSettings settings)
    {
        return
            $"DefaultEndpointsProtocol=http;AccountName={DevStorageAccountName};" +
            $"AccountKey={DevKey};" +
            $"{endpoint}=http://127.0.0.1:{settings.InternalPort}/" +
            $"{DevStorageAccountName};";
    }

}