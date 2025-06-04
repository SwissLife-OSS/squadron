using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Squadron;

/// <summary>
/// Status checker for AzureStorage Blob
/// </summary>
/// <seealso cref="IResourceStatusProvider" />
public class AzureStorageBlobStatus : IResourceStatusProvider
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureStorageBlobStatus"/> class.
    /// </summary>
    public AzureStorageBlobStatus(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Determines whether Azure Blob is ready
    /// </summary>
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
        Azure.Response<BlobServiceProperties> serviceProperties =
            await blobServiceClient.GetPropertiesAsync(cancellationToken);
        return new Status
        {
            IsReady = serviceProperties != null,
            Message = $"Service version: {serviceProperties.Value.DefaultServiceVersion}."
        };
    }
}