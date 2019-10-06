namespace Squadron
{
    public class RabbitMQDefaultOptions : ContainerResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("rabbit")
                .Image("rabbitmq:3")
                .InternalPort(5672)
                .Username("guest")
                .Password("guest");
        }
    }
}
