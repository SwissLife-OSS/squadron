using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Squadron
{
    /// <summary>
    /// Status checker for MongoDB
    /// </summary>
    /// <seealso cref="IResourceStatusProvider" />
    public sealed class MongoStatus : IResourceStatusProvider
    {
        private readonly BsonValue _okResult = BsonValue.Create(1.0);
        private readonly string _systemDatabase = "admin";
        private readonly string _pingCommand = "{ping:1}";
        private readonly IMongoClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="MongoStatus"/> class.
        /// </summary>
        public MongoStatus(IMongoClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Determines whether MongoDB is ready.
        /// </summary>
        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            BsonDocument result = await _client
                .GetDatabase(_systemDatabase)
                .RunCommandAsync<BsonDocument>(_pingCommand,
                                               readPreference: null,
                                               cancellationToken);

            return new Status
            {
                IsReady = result["ok"].AsBsonValue == _okResult,
                Message = result.ToString()
            };
        }
    }
}
