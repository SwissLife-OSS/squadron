using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace Squadron
{
    public class PostgreSqlResourceTests : ISquadronResourceFixture<PostgreSqlResource>
    {
        private readonly PostgreSqlResource _resource;

        public PostgreSqlResourceTests(SquadronResource<PostgreSqlResource> resource)
        {
            _resource = resource.Resource;
        }

        [Fact]
        public void PrepareResource_NoError()
        {
            //Act
            Action action = () =>
            {
                _resource.GetConnection();
            };

            //Assert
            action.Should().NotThrow();
        }

        [Fact]
        public async Task CreateDatabaseAndRunSqlScript()
        {
            //Arrange
            var script = File.ReadAllText(Path.Combine("Resources", "Init1.sql"));
            var newDb = "squadron";

            //Act
            await _resource.CreateDatabaseAsync(newDb);
            await _resource.RunSqlScriptAsync(script, newDb);

            //Assert
            var features = new List<string>();

            using (NpgsqlConnection con = _resource.GetConnection(newDb))
            {
                NpgsqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from features";
                await con.OpenAsync();
                using (NpgsqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while ( await reader.ReadAsync())
                    {
                        features.Add((string) reader["name"]);
                    }
                }
            }
            features.Should().HaveCount(6);
        }
    }
}
