using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace Squadron;

public class PostgreSqlResourceTests(PostgreSqlResource resource) : IClassFixture<PostgreSqlResource>
{
    [Fact]
    public void PrepareResource_NoError()
    {
        //Act
        Action action = () =>
        {
            resource.GetConnection();
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
        await resource.CreateDatabaseAsync(newDb);
        await resource.RunSqlScriptAsync(script, newDb);

        //Assert
        var features = new List<string>();

        using (NpgsqlConnection con = resource.GetConnection(newDb))
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