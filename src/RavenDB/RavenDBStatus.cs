using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Operations;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Squadron;

/// <summary>
/// Status checker for RavenDB
/// </summary>
/// <seealso cref="IResourceStatusProvider" />
public class RavenDBStatus : IResourceStatusProvider
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDBStatus"/> class.
    /// </summary>
    /// <param name="connectionString">The ConnectionString</param>
    public RavenDBStatus(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <inheritdoc/>
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        var store = new DocumentStore()
        {
            Urls = new[] { _connectionString },
        };
        store.Initialize();
        await store.Maintenance.Server.SendAsync(
            new CreateDatabaseOperation(new DatabaseRecord("health")));

        return new Status
        {
            IsReady = true
        };
    }
}