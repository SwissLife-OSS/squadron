using System;

namespace Squadron
{
    /// <summary>
    /// Default MariaDB resource options
    /// </summary>
    public class MariaDBDefaultOptions
        : ContainerResourceOptions
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
                .Name("mariadb")
                .Image("mariadb:latest")
                .AddEnvironmentVariable($"MYSQL_ROOT_PASSWORD={Password}")
                .AddEnvironmentVariable($"MYSQL_USER={User}")
                .AddEnvironmentVariable($"MYSQL_PASSWORD={Password}")
                .WaitTimeout(60)
                .Username(User)
                .Password(Password)
                .InternalPort(3306);
        }
    }
}
