using System.Threading.Tasks;
using Neo4j.Driver;
using Xunit;

namespace Squadron;

public class Neo4jResource : Neo4jResource<Neo4jDefaultOptions> { }

/// <summary>
/// Represents a neo4j database resource that can be used by unit tests.
/// </summary>
public class Neo4jResource<TOptions>
    : ContainerResource<TOptions>
        , IAsyncLifetime
    where TOptions : ContainerResourceOptions, new()
{
    /// <summary>
    /// Neo4j database driver
    /// </summary>
    public IDriver Driver;

    /// <summary>
    /// ConnectionString
    /// </summary>
    public string ConnectionString { get; private set; }

    public async override Task InitializeAsync()
    {
        await base.InitializeAsync().ConfigureAwait(false);

        ConnectionString = $"bolt://{Manager.Instance.Address}:{Manager.Instance.HostPort}";

        Driver = GraphDatabase.Driver(ConnectionString);
        await Initializer.WaitAsync(new Neo4jStatus(Driver)).ConfigureAwait(false);
    }

    public IAsyncSession GetAsyncSession(string databaseName = null)
    {
        return Driver.AsyncSession(o => o.WithDatabase(databaseName ?? "neo4j"));
    }

    public void Dispose()
    {
        Driver?.Dispose();
    }
}