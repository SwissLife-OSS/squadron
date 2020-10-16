namespace Squadron
{
    /// <summary>
    /// Default mySql resource options
    /// </summary>
    public class MySqlDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("mysql")
                .Image("mysql/mysql-server:latest")
                .AddEnvironmentVariable("MYSQL_ROOT_PASSWORD=MyPassword")
                .InternalPort(3306);
        }
    }
}
