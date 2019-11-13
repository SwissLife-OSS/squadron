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
        : IClassFixture<MongoReplicateSetResource>
    {
        private readonly MongoReplicateSetResource _mongoRsResource;

        public MongoReplicaSetResourceTests(MongoReplicateSetResource mongoRsResource)
        {
            _mongoRsResource = mongoRsResource;
        }

        [Fact]
        public void CommitTransaction_NoError()
        {
            //Act
            Action action = () =>
            {
                IMongoClient client = new MongoClient(_mongoRsResource.ConnectionString);
                using (IClientSessionHandle session = client.StartSession())
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
