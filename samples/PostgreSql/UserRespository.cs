using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace PostgreSql
{
    public class UserRespository
    {
        private readonly IConfiguration _configuration;

        public UserRespository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task AddAsync(User user)
        {
            string connnectionString = _configuration.GetValue<string>("DBConnectionString");

            using (var con = new NpgsqlConnection(connnectionString))
            {
                NpgsqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO myUsers (id, name, email) " +
                                  "VALUES (@id, @name, @email)";
                cmd.Parameters.AddWithValue("id", user.Id);
                cmd.Parameters.AddWithValue("name", user.Name);
                cmd.Parameters.AddWithValue("email", user.Email);
                await con.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
        } 
    }
}
