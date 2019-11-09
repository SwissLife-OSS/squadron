using System.Text.Json;
using FluentAssertions;
using Squadron.Samples.Shared;
using StackExchange.Redis;
using Xunit;

namespace Squadron.Samples.Redis
{
    public class UserCacheTests : IClassFixture<RedisResource>
    {
        private readonly RedisResource _redisResource;

        public UserCacheTests(RedisResource redisResource)
        {
            _redisResource = redisResource;
        }

        [Fact]
        public void Add_AddedUserIsEquivalent()
        {
            //arrange
            var user = User.CreateSample();
            ConnectionMultiplexer connection = _redisResource.GetConnection();
            var repo = new UserCache(connection);

            //act
            repo.Add(user);

            //assert
            User cachedUser = GetUserFromCache(user.Id);
            cachedUser.Should().BeEquivalentTo(user);
        }

        private User GetUserFromCache(string id)
        {
            IDatabase db = _redisResource.GetConnection().GetDatabase();
            RedisValue json = db.StringGet($"_users_{id}");
            return JsonSerializer.Deserialize<User>(json.ToString());
        }
    }
}
