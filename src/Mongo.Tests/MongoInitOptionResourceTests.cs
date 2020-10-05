using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace Squadron
{
    public class MongoInitOptionResourceTests
        : IResourceFixture<MongoResource<FileInitOptions>>
    {
        private readonly MongoResource<FileInitOptions> _mongoResource;

        public MongoInitOptionResourceTests(XUnitResource<MongoResource<FileInitOptions>> mongoResource)
        {
            _mongoResource = mongoResource.Resource;
        }

        [Fact]
        public async Task Database_IsInitialized()
        {
            IMongoCollection<BsonDocument> collection = _mongoResource.Client
                .GetDatabase("fileImport")
                .GetCollection<BsonDocument>("news");

            List<BsonDocument> items = await collection
                .Find(Builders<BsonDocument>.Filter.Empty)
                .ToListAsync();

            items.Should().HaveCount(1);
        }
    }

    public class FileInitOptions : MongoInitOptions
    {
        public override CreateDatabaseFromFilesOptions GetOptions()
        {
            return new CreateDatabaseFromFilesOptions
            {
                DatabaseOptions = new CreateDatabaseOptions
                {
                    DatabaseName = "fileImport"
                },
                Files = new[]
                {
                    new FileInfo(Path.Combine("Resources", "news.json"))
                }
            };
        }
    }
}
