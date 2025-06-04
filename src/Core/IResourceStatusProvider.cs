using System.Threading;
using System.Threading.Tasks;

namespace Squadron;

/// <summary>
/// Provide the current status of a given resource 
/// </summary>
public interface IResourceStatusProvider
{
    /// <summary>
    /// Determines whether a resource is ready.
    /// </summary>
    Task<Status> IsReadyAsync(CancellationToken cancellationToken);
}

/// <summary>
/// Describe current status
/// </summary>
public class Status
{
    /// <summary>
    /// Not ready status
    /// </summary>
    public static Status NotReady { get; } =
        new Status
        {
            IsReady = false,
            Message = "Not ready."
        };

    /// <summary>
    /// Ready status
    /// </summary>
    public bool IsReady { get; set; }

    /// <summary>
    /// Additional status information
    /// </summary>
    public string Message { get; set; }
}