using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Squadron
{
    public class MongoReplicaSetResourceTests
        : ISquadronResourceFixture<MongoReplicaSetResource>
    {
        private readonly MongoReplicaSetResource _mongoRsResource;

        public MongoReplicaSetResourceTests(SquadronResource<MongoReplicaSetResource> mongoRsResource)
        {
            _mongoRsResource = mongoRsResource.Resource;
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
