using System;

namespace Squadron
{
    /// <summary>
    /// Default Redis resource options
    /// </summary>
    public class RedisDefaultOptions
        : ContainerResourceOptions,
        IComposableResourceOption
    {
        public Type ResourceType => typeof(RedisResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("redis")
                .Image("public.ecr.aws/docker/library/redis:latest")
                .InternalPort(6379);
        }
    }
}
