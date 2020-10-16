using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class KafkaDefaultOptions : ContainerResourceOptions, IComposableResourceOption
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("kafka")
                .Image("bitnami/kafka:latest")
                .InternalPort(9092)
                .AddEnvironmentVariable("KAFKA_BROKER_ID=1")
                .AddEnvironmentVariable("KAFKA_LISTENERS=PLAINTEXT://:9092")
                .AddEnvironmentVariable("KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://127.0.0.1:9092")
                .AddEnvironmentVariable("KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181")
                .AddEnvironmentVariable("ALLOW_PLAINTEXT_LISTENER=yes");
        }

        public Type ResourceType => typeof(KafkaResource);
    }
}
