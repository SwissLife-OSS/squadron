using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MySql.Data.MySqlClient;
using Xunit;

namespace Squadron
{
    public class MySqlResourceTests : IClassFixture<MySqlResource>
    {
        private readonly MySqlResource _resource;

        public MySqlResourceTests(MySqlResource resource)
        {
            _resource = resource;
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
            //await _resource.RunSqlScriptAsync(script, newDb);

            //Assert
            var features = new List<string>();

            using (MySqlConnection con = _resource.GetConnection(newDb))
            {
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from features";
                await con.OpenAsync();
                using (DbDataReader reader = await cmd.ExecuteReaderAsync())
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
