using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;

namespace Squadron
{
    public class MongoResourceTests
        : IClassFixture<MongoResource>
    {
        public MongoResourceTests(MongoResource mongoResource)
        {
            MongoResource = mongoResource;
        }

        private MongoResource MongoResource { get; }

        [Fact]
        public void CreateAndUseCollection()
        {
            // arrange
            IMongoCollection<BsonDocument> collection = MongoResource.CreateCollection<BsonDocument>();

            BsonDocument document = new BsonDocument {{"a", "b"}};

            // act
            collection.InsertOne(document);

            // assert
            BsonDocument retrieved = collection.Find(new BsonDocument()).First();
            Assert.True(retrieved.Contains("a"));
            Assert.Equal(document["a"], retrieved["a"]);
        }

        [Fact]
        public void ReUseDatabase()
        {
            // arrange
            BsonDocument document = new BsonDocument {{"a", "b"}};

            // act
            IMongoDatabase database = MongoResource.CreateDatabase();
            IMongoCollection<BsonDocument> collectiona = MongoResource.CreateCollection<BsonDocument>(database);
            IMongoCollection<BsonDocument> collectionb = MongoResource.CreateCollection<BsonDocument>(database);

            collectiona.InsertOne(document);
            collectionb.InsertOne(document);

            // assert
            Assert.NotEqual(collectiona, collectionb);
            Assert.Equal(database, collectiona.Database);
            Assert.Equal(database, collectionb.Database);

            BsonDocument retrieveda = collectiona.Find(new BsonDocument()).First();
            BsonDocument retrievedb = collectionb.Find(new BsonDocument()).First();

            Assert.True(retrieveda.Contains("a"));
            Assert.Equal(document["a"], retrieveda["a"]);
            Assert.True(retrievedb.Contains("a"));
            Assert.Equal(document["a"], retrievedb["a"]);
        }

        [Fact]
        public async Task CreateCollectionFromFile_DefaultOptions()
        {
            // act
            IMongoCollection<BsonDocument> collection = await MongoResource.CreateCollectionFromFileAsync<BsonDocument>(
                null,
                new CreateCollectionFromFileOptions
                {
                    File = new FileInfo(Path.Combine("Resources", "news.json")),
                });

            // assert
            BsonDocument imported = collection.Find(new BsonDocument()).FirstOrDefault();
            imported.Should().NotBeNull();
            imported.Contains("headline").Should().BeTrue();
            imported.Contains("date").Should().BeTrue();
            imported.Contains("views").Should().BeTrue();
            imported.Contains("author").Should().BeTrue();
            imported.Contains("published").Should().BeTrue();
            imported.Contains("tags").Should().BeTrue();
        }

        [Fact]
        public async Task CreateCollectionFromFile_CustomOptions()
        {
            // act
            IMongoCollection<BsonDocument> collection = await MongoResource.CreateCollectionFromFileAsync<BsonDocument>(
                null,
                new CreateCollectionFromFileOptions
                {
                    File = new FileInfo(Path.Combine("Resources", "news.tsv")),
                    CollectionOptions = new CreateCollectionOptions
                    {
                        CollectionName = "myCollection",
                        DatabaseOptions = new CreateDatabaseOptions
                        {
                            DatabaseName = "myDatabase"
                        }
                    },
                    CustomImportArgs = new []
                    {
                        "--type=tsv",
                        "--headerline"
                    }
                });

            // assert
            BsonDocument imported = collection.Find(new BsonDocument()).FirstOrDefault();
            imported.Should().NotBeNull();
            imported.Contains("headline").Should().BeTrue();
            imported.Contains("date").Should().BeTrue();
            imported.Contains("views").Should().BeTrue();
            imported.Contains("author").Should().BeTrue();
            imported.Contains("published").Should().BeTrue();
            imported.Contains("tags").Should().BeTrue();
        }
    }
}
