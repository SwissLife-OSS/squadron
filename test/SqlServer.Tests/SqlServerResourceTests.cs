#if NET46
#endif
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Xunit;
using Xunit.Abstractions;

namespace Squadron;

public class SqlServerResourceTests(
    SqlServerResource resource,
    ITestOutputHelper logger) : IClassFixture<SqlServerResource>
{
#if NET46
        [Fact(Skip = "How container will be handled in Test Agent? TODO: Pull image from Azure Registry")]
        public void SqlServerResource_FromDatabase_HasContent()
        {
        }

        [Fact]
        public void SqlServerResource_FromDacPackage_HasContent()
        {
            // arrange
            string dacpacFile = Path.Combine("Resources", "SampleModel.dacpac");
            const string databaseName = "Sales_7DB7B310";

            // act
            var connectionString = _resource.DeployDatabase(dacpacFile, databaseName);

            // assert
            string retrievedDatabaseName = FindDatabase(connectionString, databaseName);
            Assert.Equal(databaseName, retrievedDatabaseName);
        }
#else
    [Fact]
    public async Task SqlServerResource_FromSqlScript_HasContent()
    {
        // arrange
        string sqlFile = Path.Combine("Resources", "SampleModel.sql");
        const string databaseName = "Sales_DEABB979";

        // act
        var connectionString = await resource.CreateDatabaseAsync(
            File.ReadAllText(sqlFile), databaseName);

        // assert
        string retrievedDatabaseName = FindDatabase(connectionString, databaseName);
        Assert.Equal(databaseName, retrievedDatabaseName);
    }

    [Fact]
    public async Task SqlServerResource_Empty_Created()
    {
        // arrange
        const string databaseName = "Sales_DEABB980";

        // act
        var connectionString = await resource.CreateDatabaseAsync(databaseName);

        // assert
        string retrievedDatabaseName = FindDatabase(connectionString, databaseName);
        Assert.Equal(databaseName, retrievedDatabaseName);
    }
#endif

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