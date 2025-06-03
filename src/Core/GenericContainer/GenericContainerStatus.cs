using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron;

/// <summary>
///  GenericContainer statu checker
/// </summary>
/// <seealso cref="Squadron.IResourceStatusProvider" />
public class GenericContainerStatus : IResourceStatusProvider
{
    private readonly Func<ContainerAddress, CancellationToken, Task<Status>> _statusChecker;
    private readonly ContainerAddress _address;

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericContainerStatus"/> class.
    /// </summary>
    /// <param name="statusChecker">The status checker.</param>
    /// <param name="address">The address.</param>
    public GenericContainerStatus(Func<ContainerAddress, CancellationToken, Task<Status>> statusChecker,
        ContainerAddress address)
    {
        _statusChecker = statusChecker;
        _address = address;
    }

    /// <inheritdoc/>
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        return await _statusChecker(_address, cancellationToken);
    }
}