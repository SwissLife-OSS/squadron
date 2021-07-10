using System.Collections.Generic;
using System.Text;
using Squadron.AzureCloud;

namespace Squadron
{
    /// <summary>
    /// ServiceBusOptions builder
    /// </summary>
    public class ServiceBusOptionsBuilder : AzureResourceOptionsBuilder
    {
        private ServiceBusModel _model = new ServiceBusModel();
        private List<ServiceBusTopicBuilder> _topics = new List<ServiceBusTopicBuilder>();
        private List<ServiceBusQueueBuilder> _queues = new List<ServiceBusQueueBuilder>();

        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static ServiceBusOptionsBuilder New() => new ServiceBusOptionsBuilder();


        private ServiceBusOptionsBuilder()
            : base()
        {

        }

        /// <summary>
        /// Namespace
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <returns></returns>
        public ServiceBusOptionsBuilder Namespace(string ns)
        {
            return Namespace(ns, createIfNotExists: false);
        }

        /// <summary>
        /// Namespace
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <param name="createIfNotExists">creates namespace if doesn't exist.</param>
        /// <returns></returns>
        public ServiceBusOptionsBuilder Namespace(string ns, bool createIfNotExists)
        {
            _model.Namespace = ns;
            _model.ProvisioningMode = createIfNotExists
                ? ServiceBusProvisioningMode.CreateIfNotExists
                : ServiceBusProvisioningMode.UseExisting;
            return this;
        }


        /// <summary>
        /// Adds a topic.
        /// </summary>
        /// <param name="name">The topic name.</param>
        /// <returns></returns>
        public ServiceBusTopicBuilder AddTopic(string name)
        {
            var topicBuilder = ServiceBusTopicBuilder.New(name);
            _topics.Add(topicBuilder);
            return topicBuilder;
        }


        /// <summary>
        /// Adds a queue.
        /// </summary>
        /// <param name="name">The queue name.</param>
        /// <returns></returns>
        public ServiceBusQueueBuilder AddQueue(string name)
        {
            var queueBuilder = new ServiceBusQueueBuilder(name);
            _queues.Add(queueBuilder);
            return queueBuilder;
        }


        /// <summary>
        /// Builds the options
        /// </summary>
        /// <returns></returns>
        public ServiceBusModel Build()
        {
            foreach (ServiceBusTopicBuilder tb in _topics)
            {
                _model.Topics.Add(tb.Build());
            }
            foreach ( ServiceBusQueueBuilder qb in _queues)
            {
                _model.Queues.Add(qb.Build());
            }
            return _model;
        }
    }
}
