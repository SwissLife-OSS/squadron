using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
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
        public async Task UserRepository_Add_AddedUser()
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
