using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron
{
    public sealed class MongoReplicaSetStatus : IResourceStatusProvider
    {
        private readonly MongoClient _mongoClient;

        public MongoReplicaSetStatus(MongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            IMongoDatabase adminDb = _mongoClient.GetDatabase("admin");
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument
            {
                {"isMaster", 1}
            });

            BsonDocument res = await adminDb.RunCommandAsync(command);
            bool isMaster = res.GetValue("ismaster").AsBoolean;

            return new Status
            {
                IsReady = isMaster,
                Message = res.ToString()
            };
        }
    }
}


