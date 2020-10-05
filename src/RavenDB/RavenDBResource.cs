using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Squadron
{
    /// <inheritdoc/>
    public class RavenDBResource : RavenDBResource<RavenDBDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a RavenDB resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class RavenDBResource<TOptions>
        : ContainerResource<TOptions>,
          ISquadronAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    { 
        /// <summary>
        /// Connection string to access to queue
        /// </summary>
    public string ConnectionString { get; private set; }

        /// <inheritdoc cref="ISquadronAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            ConnectionString = $"http://{Manager.Instance.Address}:{Manager.Instance.HostPort}";

            await Initializer.WaitAsync(
                new RavenDBStatus(ConnectionString));
        }

        public IDocumentStore CreateDatabase(string name)
        {
            using (IDocumentStore store = GetDocumentStore())
            {
                store.Maintenance.Server.Send(
                    new CreateDatabaseOperation(new DatabaseRecord(name)));
            }
            return GetDocumentStore(name);
        }


        /// <summary>
        /// Gets a RavenDB DocumentStore
        /// </summary>
        /// <returns></returns>
        public IDocumentStore GetDocumentStore(string databaseName=null)
        {
            var store = new DocumentStore()
            {
                Urls = new[] { ConnectionString },
                Database = databaseName
            };
            store.Initialize();
            return store;
        }
    }
}
