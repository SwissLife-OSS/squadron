using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Snapshooter.Xunit;
using Squadron.Samples.Shared;
using Xunit;

namespace Squadron.Samples.Mongo
{
    public class UserRepositoryTests : IClassFixture<MongoResource> 
    {
        private readonly MongoResource _mongoResource;

        public UserRepositoryTests(MongoResource mongoResource)
        {
            _mongoResource = mongoResource;
        }

        [Fact]
        public async Task Add_AddedUserIsEquivalent()
        {
            //arrange
            var user = User.CreateSample();
            IMongoDatabase db = _mongoResource.CreateDatabase();
            var repo = new UserRepository(db);

            //act
            await repo.AddAsync(user);

            //assert
            User createdUser = await GetUserAsync(db, user.Id);
            createdUser.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetAll_MatchSnapshot()
        {
            //arrange
            IMongoDatabase db = _mongoResource.CreateDatabase();
            var options = new CreateCollectionFromFileOptions
            {
                CollectionOptions = new CreateCollectionOptions
                {
                    CollectionName = "users"
                },
                File = new FileInfo("users.json")
            };

            IMongoCollection<User> col = await _mongoResource  
                .CreateCollectionFromFileAsync<User>(db, options);

            var repo = new UserRepository(db);

            //act
            IEnumerable<User> allUsers = await repo.GetAllAsync();

            //assert
            allUsers.MatchSnapshot();
        }

        private Task<User> GetUserAsync(IMongoDatabase db, string id)
        {
            IMongoCollection<User> col = db.GetCollection<User>("users");
            Task<User> user = col.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return user;
        }
    }
}
