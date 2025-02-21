using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron
{
    /// <inheritdoc/>
    public class MongoReplicaSetResource : MongoReplicaSetResource<MongoReplicaSetDefaultOptions> { }

    /// <summary>
    /// Represents a mongo database replica set resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MongoReplicaSetResource<TOptions> :
        MongoResource<TOptions>
        where TOptions : MongoReplicaSetDefaultOptions, new()
    {
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            var client = new MongoClient(ConnectionString);
            BsonDocument rsConfig = CreateReplicaSetConfiguration();
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"replSetInitiate", rsConfig}
            });

            await client.GetDatabase("admin")
                .RunCommandAsync(command);

            await Initializer.WaitAsync(new MongoReplicaSetStatus(ConnectionString));
        }

        public override IMongoClient Client => new MongoClient(ConnectionString);

        private BsonDocument CreateReplicaSetConfiguration()
        {
            var membersDocument = new BsonArray();
            {
                membersDocument.Add(new BsonDocument {
                    {"_id", 0},
                    {"host", $"{Manager.Instance.Address}:{Settings.InternalPort}"}
                });
            }
            var cfg = new BsonDocument {
                {"_id", ResourceOptions.ReplicaSetName},
                {"members", membersDocument}
            };
            return cfg;
        }
    }
}

