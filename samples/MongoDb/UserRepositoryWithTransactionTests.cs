using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Squadron.Samples.Shared;
using Xunit;

namespace Squadron.Samples.Mongo
{
    public class UserRepositoryWithTransactionTests
        : IClassFixture<MongoReplicaSetResource>
    {
        private readonly MongoReplicaSetResource _mongoRsResource;

        public UserRepositoryWithTransactionTests(MongoReplicaSetResource mongoRsResource)
        {
            _mongoRsResource = mongoRsResource;
        }

        [Fact]
        public async Task AddUserAndAddAudit_Commit_RecordsExsists()
        {
            //arrange
            var user = User.CreateSample();
            var audit = UserAudit.CreateSample(user.Id);

            IMongoDatabase db = _mongoRsResource.CreateDatabase();
            await db.CreateCollectionAsync("users");
            await db.CreateCollectionAsync("audit");

            using (IClientSessionHandle session =
                await _mongoRsResource.Client.StartSessionAsync() )
            {
                var repo = new UserRepositoryWithTransaction(db);
                session.StartTransaction();

                //act
                await repo.AddAsync(user, session);
                await repo.AddAuditAsync(audit, session);

                await session.CommitTransactionAsync();
            }
 

            //assert
            User createdUser = await GetUserAsync(db, user.Id);
            createdUser.Should().BeEquivalentTo(user);

            UserAudit createdAudit = await GetUserAuditAsync(db, audit.Id);
            createdAudit.Should().BeEquivalentTo(audit);
        }


        [Fact]
        public async Task AddUserAndAddAudit_Abort_UserNull()
        {
            //arrange
            var user = User.CreateSample();

            IMongoDatabase db = _mongoRsResource.CreateDatabase();
            await db.CreateCollectionAsync("users");

            using (IClientSessionHandle session =
                await _mongoRsResource.Client.StartSessionAsync())
            {
                var repo = new UserRepositoryWithTransaction(db);
                session.StartTransaction();

                //act
                await repo.AddAsync(user, session);
                await session.AbortTransactionAsync();
            }

            //assert
            User createdUser = await GetUserAsync(db, user.Id);
            createdUser.Should().BeNull();
        }


        private async Task<User> GetUserAsync(IMongoDatabase db, string id)
        {
            IMongoCollection<User> col = db.GetCollection<User>("users");
            User user = await col.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return user;
        }


        private async Task<UserAudit> GetUserAuditAsync(IMongoDatabase db, string id)
        {
            IMongoCollection<UserAudit> col = db.GetCollection<UserAudit>("audit");
            UserAudit audit = await col.AsQueryable()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return audit;
        }

    }
}
