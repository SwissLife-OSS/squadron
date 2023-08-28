using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    public class ClickHouseResourceTests : IClassFixture<ClickHouseResource>
    {
        private readonly ClickHouseResource _resource;

        public ClickHouseResourceTests(ClickHouseResource resource)
        {
            _resource = resource;
        }

        [Fact]
        public async Task CreateDatabaseAndRunSqlScript()
        {
            //Arrange

            //Act
            await _resource.RunSqlScriptAsync("CREATE DATABASE test;");

            //Assert
            var res = await _resource.SendCommand("SELECT name FROM system.databases;");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("test", await res.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task ShouldCreateTableInDatabase()
        {
            //Arrange
            await _resource.RunSqlScriptAsync("CREATE DATABASE test2;");

            //Act
            await _resource.RunSqlScriptAsync(
                "CREATE TABLE test_table (id Int32) ENGINE = MergeTree ORDER BY (id);",
                "test2");

            //Assert
            var res = await _resource.SendCommand(
                "SELECT name FROM system.tables WHERE database = 'test2';");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            Assert.Contains("test_table", await res.Content.ReadAsStringAsync());
        }
    }
}
