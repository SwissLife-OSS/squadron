using System;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Squadron
{
    public class MongoReplicaSetResourceTests
        : IClassFixture<MongoReplicaSetResource>
    {
        private readonly MongoReplicaSetResource _mongoRsResource;

        public MongoReplicaSetResourceTests(MongoReplicaSetResource mongoRsResource)
        {
            _mongoRsResource = mongoRsResource;
        }

        [Fact]
        public void CommitTransaction_NoError()
        {
            //Act
            Action action = () =>
            {
                using (IClientSessionHandle session = _mongoRsResource.Client.StartSession())
                {
                    IMongoCollection<BsonDocument> collection = _mongoRsResource.CreateCollection<BsonDocument>("bar");
                    session.StartTransaction();
                    collection.InsertOne(session, new BsonDocument("name", "test"));
                    session.CommitTransaction();
                }
            };

            //Assert
            action.Should().NotThrow();
        }
    }
}
