using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    /// <summary>
    /// Default S3 resource options
    /// </summary>
    public class S3DefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {
        public Type ResourceType => typeof(S3Resource);

        const string AccessKey = "minioadmin";
        const string SecretKey = "minioadmin";

        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("minio")
                .Image("minio/minio:latest")
                .AddCmd("server", "/data")
                .Username(AccessKey)
                .Password(SecretKey)
                .InternalPort(9000);
        }
    }
}
