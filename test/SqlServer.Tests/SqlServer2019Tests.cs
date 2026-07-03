using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Squadron.Resources;
using Xunit;
using Xunit.Abstractions;

namespace Squadron;

public class SqlServer2019Tests(SqlServerResource<SqlServer2019Options> resource, ITestOutputHelper logger)
    : IClassFixture<SqlServerResource<SqlServer2019Options>>
{
    [Fact]
    public async Task SqlServer2019Resource_Empty_Created()
    {
        // arrange
        const string databaseName = "Sales_DEABB989";

        // act
        var connectionString = await resource.CreateDatabaseAsync(databaseName);

        // assert
        string retrievedDatabaseName = FindDatabase(connectionString, databaseName);
        Assert.Equal(databaseName, retrievedDatabaseName);
    }

    private string FindDatabase(string databaseConnection, string databaseName)
    {
        string value = null;
        try
        {
            using (var connection = new SqlConnection(databaseConnection))
            {
                connection.Open();
                logger.WriteLine("Connection is open.");

                using (var command = new SqlCommand($"SELECT * FROM sys.databases WHERE name = '{databaseName}'",
                           connection))
                {
                    value = (string) command.ExecuteScalar();
                    logger.WriteLine("Database name retrieved.");
                }
            }
        }
        catch (Exception ex)
        {
            logger.WriteLine("FindDatabase failed.");
            logger.WriteLine(ex.Message);
        }

        return value;
    }
}