using System;

namespace Squadron
{
    /// <summary>
    /// Default Mongo resource options
    /// </summary>
    public class MongoDefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {

        public Type ResourceType => typeof(MongoResource);

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("mongodb")
                .Image("mongo:latest")
                .InternalPort(27017);
        }
    }
}

