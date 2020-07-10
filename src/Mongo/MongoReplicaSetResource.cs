using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron
{
    /// <summary>
    /// Represents a mongo database resplica set resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class MongoReplicaSetResource : MongoResource<MongoReplicaSetDefaultOptions>
    {
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            var client = new MongoClient(ExternalConnectionString + "/?connect=direct");
            BsonDocument rsConfig = CreateReplicaSetConfiguration();
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"replSetInitiate", rsConfig}
            });

            await client.GetDatabase("admin")
                .RunCommandAsync(command);

            await Initializer.WaitAsync(new MongoReplicaSetStatus(ExternalConnectionString));
        }

        public override IMongoClient Client => new MongoClient(ExternalConnectionString);

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

