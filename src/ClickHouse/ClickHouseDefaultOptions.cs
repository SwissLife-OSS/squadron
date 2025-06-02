namespace Squadron
{
    /// <summary>
    /// Default ClickHouse resource options
    /// </summary>
    public class ClickHouseDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("clickhouse-server")
                .Image("clickhouse/clickhouse-server")
                .Username("default")
                .Password("") // ClickHouse default user has empty password by default
                .AddEnvironmentVariable("CLICKHOUSE_USER=default")
                .AddEnvironmentVariable("CLICKHOUSE_PASSWORD=")
                .AddEnvironmentVariable("CLICKHOUSE_DEFAULT_ACCESS_MANAGEMENT=1")
                .InternalPort(8123)
                .PreferLocalImage();
        }
    }
}
