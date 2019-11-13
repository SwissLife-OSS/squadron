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
    public class MongoReplicateSetResource : MongoResource<MongoReplicateSetDefaultOptions>
    {
        public async override Task InitializeAsync()
        {
            await base.InitializeAsync();
            var client = new MongoClient(ConnectionString + "/?connect=direct");
            BsonDocument rsConfig = CreateReplicaSetConfiguration();
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"replSetInitiate", rsConfig}
            });

            await client.GetDatabase("admin")
                .RunCommandAsync(command);

            SetClient();
            await WaitForMaster();
        }

        private async Task WaitForMaster()
        {
            IMongoDatabase adminDb = Client.GetDatabase("admin");
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"isMaster", 1}
            });

            int retryCount = 0;

            while (true )
            {
                BsonDocument res = await adminDb.RunCommandAsync(command);
                bool isMaster = res.GetValue("ismaster").AsBoolean;
                if (isMaster)
                    break;
                await Task.Delay(1000);
                retryCount++;

                if (retryCount > 3)
                    throw new ApplicationException("Timeout expired while waiting for master");
            }
        }

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

