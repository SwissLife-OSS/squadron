// Copyright 2020 Energinet DataHub A/S
//
// Licensed under the Apache License, Version 2.0 (the "License2");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Squadron.Resources;
using Xunit;
using Xunit.Abstractions;

namespace Squadron
{
    public class SqlServer2019Tests
        : IClassFixture<SqlServerResource<SqlServer2019Options>>
    {
        private readonly SqlServerResource<SqlServer2019Options> _resource;
        private readonly ITestOutputHelper _logger;

        public SqlServer2019Tests(SqlServerResource<SqlServer2019Options> resource, ITestOutputHelper logger)
        {
            _resource = resource;
            _logger = logger;
        }

        [Fact]
        public async Task SqlServer2019Resource_Empty_Created()
        {
            // arrange
            const string databaseName = "Sales_DEABB989";

            // act
            var connectionString = await _resource.CreateDatabaseAsync(databaseName);

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
                    _logger.WriteLine("Connection is open.");

                    using (var command = new SqlCommand($"SELECT * FROM sys.databases WHERE name = '{databaseName}'",
                        connection))
                    {
                        value = (string) command.ExecuteScalar();
                        _logger.WriteLine("Database name retrieved.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine("FindDatabase failed.");
                _logger.WriteLine(ex.Message);
            }

            return value;
        }
    }
}
