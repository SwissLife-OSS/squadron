using System;

namespace Squadron
{
    /// <summary>
    /// Default mySql resource options
    /// </summary>
    public class MySqlDefaultOptions : ContainerResourceOptions
    {
        const string Password = "mypassword";
        const string User = "user";

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("mysql")
                .Image("mysql:5.7")
                .AddEnvironmentVariable($"MYSQL_ROOT_PASSWORD={Password}")
                .AddEnvironmentVariable($"MYSQL_DATABASE=squadron_{Guid.NewGuid()}")
                .AddEnvironmentVariable($"MYSQL_USER={User}")
                .AddEnvironmentVariable($"MYSQL_PASSWORD={Password}")
                //.AddEnvironmentVariable("MYSQL_ROOT_HOST=172.17.0.1")
                .Username(User)
                .Password(Password)
                .InternalPort(3306);
        }
    }
}
