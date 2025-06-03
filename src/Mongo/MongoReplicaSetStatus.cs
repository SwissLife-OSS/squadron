using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron;

public sealed class MongoReplicaSetStatus(string connectionString) : IResourceStatusProvider
{
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        var client = new MongoClient(connectionString);
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