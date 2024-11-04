using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Squadron.Samples.Shared;

namespace Squadron.Samples.Mongo
{
    public class UserRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public UserRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task AddAsync(User user)
        {
            IMongoCollection<User> col = _mongoDatabase.GetCollection<User>("users");
            await col.InsertOneAsync(user, options: null);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            IMongoCollection<User> col = _mongoDatabase.GetCollection<User>("users");
            return await col.AsQueryable().ToListAsync();
        }
    }
}
