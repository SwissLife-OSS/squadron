using System;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Squadron;

public class MongoReplicaSetResourceTests(MongoReplicaSetResource mongoRsResource)
    : IClassFixture<MongoReplicaSetResource>
{
    [Fact]
    public void CommitTransaction_NoError()
    {
        //Act
        Action action = () =>
        {
            using (IClientSessionHandle session = mongoRsResource.Client.StartSession())
            {
                IMongoCollection<BsonDocument> collection = mongoRsResource.CreateCollection<BsonDocument>("bar");
                session.StartTransaction();
                collection.InsertOne(session, new BsonDocument("name", "test"));
                session.CommitTransaction();
            }
        };

        //Assert
        action.Should().NotThrow();
    }
}