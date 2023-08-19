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
                .InternalPort(8123);
        }
    }
}
