using System;

namespace Squadron
{
    /// <summary>
    /// Default PostgreSQL resource options
    /// </summary>
    public class PostgreSqlDefaultOptions
        : ContainerResourceOptions,
        IComposableResourceOption
    {
        public Type ResourceType => typeof(PostgreSqlResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("postgres")
                .Image("postgres:latest")
                .Username("postgres")
                .Password(Guid.NewGuid().ToString("N").Substring(12))
                .InternalPort(5432);
        }
    }
}
