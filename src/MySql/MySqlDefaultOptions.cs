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
                .Image("mysql:latest")
                .AddEnvironmentVariable($"MYSQL_ROOT_PASSWORD={Password}")
                .AddEnvironmentVariable($"MYSQL_USER={User}")
                .AddEnvironmentVariable($"MYSQL_PASSWORD={Password}")
                .WaitTimeout(60)
                .Username(User)
                .Password(Password)
                .InternalPort(3306)
                .PreferLocalImage();
        }
    }
}
