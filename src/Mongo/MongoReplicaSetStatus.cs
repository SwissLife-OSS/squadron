using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron
{
    public sealed class MongoReplicaSetStatus : IResourceStatusProvider
    {
        private readonly string _connectionString;

        public MongoReplicaSetStatus(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            var client = new MongoClient(_connectionString);
            IMongoDatabase adminDb = client.GetDatabase("admin");
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"isMaster", 1}
            });

            BsonDocument res = await adminDb.RunCommandAsync(command);
            bool isMaster = res.GetValue("ismaster").AsBoolean;

            if (isMaster)
            {
                IClientSessionHandle session = await client.StartSessionAsync();
                session.StartTransaction();
                await session.CommitTransactionAsync();
            }

            return new Status
            {
                IsReady = isMaster,
                Message = res.ToString()
            };
        }
    }
}


