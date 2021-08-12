using System;

namespace Squadron
{
    /// <summary>
    /// Default RavenDB resource options
    /// </summary>
    public class RavenDBDefaultOptions
        : ContainerResourceOptions,
        IComposableResourceOption
    {
        public Type ResourceType => typeof(RavenDBResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("ravendb")
                .Image("ravendb/ravendb:ubuntu-latest")
                .InternalPort(8080);
        }
    }
}
