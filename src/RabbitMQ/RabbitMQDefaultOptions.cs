namespace Squadron
{
    /// <summary>
    /// Default RabbitMQ resource options
    /// </summary>
    public class RabbitMQDefaultOptions : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder
                .Name("rabbit")
                .Image("rabbitmq:3")
                .WaitTimemout(60)
                .InternalPort(5672)
                .Username("guest")
                .Password("guest");
        }
    }
}
