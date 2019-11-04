namespace Squadron
{
    /// <summary>
    /// ServiceBusQueue builder
    /// </summary>
    public class ServiceBusQueueBuilder
    {
        ServiceBusQueueModel _queue = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusQueueBuilder"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceBusQueueBuilder(string name)
        {
            _queue = new ServiceBusQueueModel(name);
        }

        internal ServiceBusQueueModel Build()
        {
            return _queue;
        }

    }
}
