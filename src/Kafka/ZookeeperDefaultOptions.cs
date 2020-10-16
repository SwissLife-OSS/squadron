using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class ZookeeperDefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("zookeeper")
                .Image("bitnami/zookeeper:latest")
                .InternalPort(2181)
                .AddEnvironmentVariable("ALLOW_ANONYMOUS_LOGIN=yes");
        }

        public Type ResourceType => typeof(ZookeeperResource);
    }
}
