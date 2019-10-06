namespace Squadron
{
    public class RedisDefaultOptions : ContainerResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("redis")
                .Image("redis:latest")
                .InternalPort(6379);
        }
    }
}
