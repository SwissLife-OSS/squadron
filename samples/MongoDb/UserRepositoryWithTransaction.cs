using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Squadron.Samples.Shared;

namespace Squadron.Samples.Mongo
{
    public class UserRepositoryWithTransaction
    {
        private readonly IMongoDatabase _mongoDatabase;

        public UserRepositoryWithTransaction(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async Task AddAsync(User user, IClientSessionHandle session)
        {
            IMongoCollection<User> col = _mongoDatabase.GetCollection<User>("users");
            await col.InsertOneAsync(session, user, options: null);
        }

        public async Task AddAuditAsync(UserAudit audit, IClientSessionHandle session)
        {
            IMongoCollection<UserAudit> col = _mongoDatabase.GetCollection<UserAudit>("audit");
            await col.InsertOneAsync(session, audit, options: null);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            IMongoCollection<User> col = _mongoDatabase.GetCollection<User>("users");
            return await col.AsQueryable().ToListAsync();
        }



    }
}
