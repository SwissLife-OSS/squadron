using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class KafkaAndZookeeperDefaultOptions : ComposeResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("zookeeper")
                .Image("zookeeper:latest")
                .InternalPort(2181)
                .ExternalPort(2181)
        }

        public override void Configure(ComposeResourceBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
