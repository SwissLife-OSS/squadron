using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using MySqlConnector;
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
            var dbName = "squadron_45233aaf";

            //Act
            await _resource.CreateDatabaseAsync(dbName);
            await _resource.RunSqlScriptAsync(script, dbName);

            //Assert
            IList<string> features = await ReadFeatures(dbName);

            features.Should().HaveCount(6);
        }

        [Fact]
        public async Task CreateTwoDatabasesWithDifferentNameAndRunSqlScript()
        {
            //Arrange
            var script1 = File.ReadAllText(Path.Combine("Resources", "Init1.sql"));
            var script2 = File.ReadAllText(Path.Combine("Resources", "Init2.sql"));
            var newDb1 = "squadron_8474fb97";
            var newDb2 = "squadron_b8d58ce0";

            //Act
            await _resource.CreateDatabaseAsync(newDb1);
            await _resource.CreateDatabaseAsync(newDb2);

            await _resource.RunSqlScriptAsync(script1, newDb1);
            await _resource.RunSqlScriptAsync(script2, newDb2);

            //Assert
            IList<string> resultDb1 = await ReadFeatures(newDb1);
            IList<string> resultDb2 = await ReadFeatures(newDb2);

            resultDb1.Should().HaveCount(6);
            resultDb2.Should().HaveCount(1);
        }

        [Fact]
        public void TryCreateDatabaseWithSameNameThrows()
        {
            //Arrange
            var newDb1 = "squadron_c87f0a89";

            //Act
            Func<Task> resultAction = async () =>
            {
                await _resource.CreateDatabaseAsync(newDb1);
                await _resource.CreateDatabaseAsync(newDb1);
            };

            //Assert
            resultAction.Should().ThrowAsync<ContainerException>().WithMessage("*database exists*");
        }

        private async Task<IList<string>> ReadFeatures(string dbName)
        {
            var features = new List<string>();

            using (MySqlConnection con = _resource.GetConnection(dbName))
            {
                MySqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "select * from features";
                await con.OpenAsync();
                using (DbDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        features.Add((string)reader["name"]);
                    }
                }
            }

            return features;
        }
    }
}
