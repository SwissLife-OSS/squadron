namespace Squadron
{
    /// <summary>
    /// Default Redis resource options
    /// </summary>
    public class RedisDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("redis")
                .Image("redis:latest")
                .InternalPort(6379);
        }
    }
}
