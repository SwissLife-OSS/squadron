using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Squadron.Samples.Shared;
using Xunit;

namespace Squadron.Samples.PostgreSql
{
    public class UserRespositoryTests : IClassFixture<PostgreSqlResource>
    {
        private readonly PostgreSqlResource _resource;
        string _dbName = "sqadron_sample";

        public UserRespositoryTests(PostgreSqlResource resource)
        {
            _resource = resource;
        }

        [Fact]
        public async Task UserRepository_Add_AddedUser()
        {
            //arrange
            var initScript = File.ReadAllText("InitDatabase.sql");
            await _resource.CreateDatabaseAsync(_dbName);
            await _resource.RunSqlScriptAsync(initScript, _dbName);

            IConfiguration config = BuildInMemoryConfiguration();


            var repo = new UserRespository(config);
            var user = User.CreateSample();

            //act
            await repo.AddAsync(user);

            //assert
            User createdUser = await GetUserAsync(user.Id);
            createdUser.Should().BeEquivalentTo(user);
        }

        private IConfiguration BuildInMemoryConfiguration()
        {
            return new ConfigurationBuilder()
                        .AddInMemoryCollection(new Dictionary<string, string>
                        {
                            { "DBConnectionString", 
                               _resource.GetConnection(_dbName).ConnectionString }
                        }).Build();
        }

        private async Task<User> GetUserAsync(string id)
        {
            using (NpgsqlConnection con = _resource.GetConnection(_dbName))
            {
                NpgsqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from myusers where id = @id";
                cmd.Parameters.AddWithValue("id", id);
                await con.OpenAsync();
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new User
                        {
                            Id = (string)reader["id"],
                            Name = (string)reader["name"],
                            Email = (string)reader["email"]
                        };
                        return user;
                    }
                }
            }
            return null;
        }
    }
}
