using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Snapshooter.Xunit3;
using Xunit;

namespace Squadron;

public class MongoResourceTests(MongoResource mongoResource) : IClassFixture<MongoResource>
{
    private MongoResource MongoResource { get; } = mongoResource;

    [Fact]
    public async Task IndexBuildMinAvailableDiskSpace_DefaultsTo50Mb()
    {
        int result = await GetIndexBuildMinAvailableDiskSpaceAsync();

        Assert.Equal(50, result);
    }

    [Fact]
    public async Task SetIndexBuildMinAvailableDiskSpace_UpdatesServerParameter()
    {
        try
        {
            await MongoResource.Client.SetIndexBuildMinAvailableDiskSpaceAsync(
                200,
                TestContext.Current.CancellationToken);

            int result = await GetIndexBuildMinAvailableDiskSpaceAsync();

            Assert.Equal(200, result);
        }
        finally
        {
            await MongoResource.Client.SetIndexBuildMinAvailableDiskSpaceAsync(
                50,
                TestContext.Current.CancellationToken);
        }
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(8_388_609)]
    public async Task SetIndexBuildMinAvailableDiskSpace_RejectsUnsupportedValue(int megabytes)
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
            MongoResource.Client.SetIndexBuildMinAvailableDiskSpaceAsync(
                megabytes,
                TestContext.Current.CancellationToken));
    }

    private async Task<int> GetIndexBuildMinAvailableDiskSpaceAsync()
    {
        BsonDocument result = await MongoResource.Client
            .GetDatabase("admin")
            .RunCommandAsync<BsonDocument>(
                new BsonDocument
                {
                    { "getParameter", 1 },
                    { "indexBuildMinAvailableDiskSpaceMB", 1 }
                },
                readPreference: null,
                TestContext.Current.CancellationToken);

        return checked((int)result["indexBuildMinAvailableDiskSpaceMB"].AsInt64);
    }

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
        imported.MatchSnapshot(o => o.IgnoreField("[0].Value"));
    }

    [Fact]
    public async Task CreateDatabaseFromFiles()
    {
        // act
        IMongoDatabase db = await MongoResource.CreateDatabase(
            new FileInfo(Path.Combine("Resources", "news.json")));

        // assert
        IMongoCollection<BsonDocument> imported = db.GetCollection<BsonDocument>("news");
        List<BsonDocument> items = await imported
            .Find(Builders<BsonDocument>.Filter.Empty)
            .ToListAsync();
        items.MatchSnapshot(o => o.IgnoreField("[0].[0].Value"));
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
        imported.MatchSnapshot(o => o.IgnoreField("[0].Value"));
    }
}
