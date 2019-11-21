using System.Text.Json;
using Squadron.Samples.Shared;
using StackExchange.Redis;

namespace Squadron.Samples.Redis
{
    public class UserCache
    {
        private readonly ConnectionMultiplexer _connection;

        public UserCache(ConnectionMultiplexer connection)
        {
            _connection = connection;
        }

        public void Add(User user)
        {
            IDatabase db = _connection.GetDatabase();
            db.StringSet($"_users_{user.Id}", JsonSerializer.Serialize(user));
        }
    }
}
