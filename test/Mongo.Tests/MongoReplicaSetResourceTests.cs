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
    public async Task IndexBuildMinAvailableDiskSpace_DefaultsTo50Mb()
    {
        BsonDocument result = await mongoRsResource.Client
            .GetDatabase("admin")
            .RunCommandAsync<BsonDocument>(
                new BsonDocument
                {
                    { "getParameter", 1 },
                    { "indexBuildMinAvailableDiskSpaceMB", 1 }
                },
                readPreference: null,
                CancellationToken.None);

        Assert.Equal(50, result["indexBuildMinAvailableDiskSpaceMB"].AsInt64);
    }

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
