using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class KafkaZookeeperDefaultOptions : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder
                .AddContainer<KafkaDefaultOptions>("kafka")
                .AddLink("zookeeper");

            builder.AddContainer<ZookeeperDefaultOptions>("zookeeper")
                .AddLink("kafka");
        }
    }
}
