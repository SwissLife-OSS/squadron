using System;

namespace Squadron
{
    /// <summary>
    /// Default OPA resource options
    /// </summary>
    public class OpaDefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {
        public Type ResourceType => typeof(OpaResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("OPA")
                .Image("openpolicyagent/opa:latest")
                .InternalPort(8181);
        }
    }
}

