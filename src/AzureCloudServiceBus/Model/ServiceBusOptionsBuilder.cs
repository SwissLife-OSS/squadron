using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    /// <summary>
    /// ServiceBusOptions builder
    /// </summary>
    public class ServiceBusOptionsBuilder
    {
        private ServiceBusModel _options = new ServiceBusModel();
        private List<ServiceBusTopicBuilder> _topics = new List<ServiceBusTopicBuilder>();
        private List<ServiceBusQueueBuilder> _queues = new List<ServiceBusQueueBuilder>();


        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static ServiceBusOptionsBuilder New() => new ServiceBusOptionsBuilder();

        /// <summary>
        /// Namespace
        /// </summary>
        /// <param name="ns">The namespace.</param>
        /// <returns></returns>
        public ServiceBusOptionsBuilder Namespace(string ns)
        {
            _options.Namespace = ns;
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
        /// Builds the model
        /// </summary>
        /// <returns></returns>
        public ServiceBusModel Build()
        {
            foreach (ServiceBusTopicBuilder tb in _topics)
            {
                _options.Topics.Add(tb.Build());
            }
            foreach ( ServiceBusQueueBuilder qb in _queues)
            {
                _options.Queues.Add(qb.Build());
            }
            return _options;
        }
    }


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


    /// <summary>
    /// ServiceBusTopic builder
    /// </summary>
    public class ServiceBusTopicBuilder
    {
        ServiceBusTopicModel _topic = null;

        private ServiceBusTopicBuilder()
        {
            _topic = new ServiceBusTopicModel();
        }

        private ServiceBusTopicBuilder(string topicName)
        {
            _topic = new ServiceBusTopicModel(topicName);
        }

        /// <summary>
        /// Creates a new empty builder
        /// </summary>
        /// <returns></returns>
        public static ServiceBusTopicBuilder New()
            => new ServiceBusTopicBuilder();


        /// <summary>
        /// Creates a new builder with a topic name
        /// </summary>
        /// <param name="name">The topic name.</param>
        /// <returns></returns>
        public static ServiceBusTopicBuilder New(string name)
            => new ServiceBusTopicBuilder(name);


        /// <summary>
        /// Topic name
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ServiceBusTopicBuilder Name(string name)
        {
            _topic.Name = name;
            return this;
        }


        /// <summary>
        /// Adds a subscription to the topic
        /// </summary>
        /// <param name="name">The subscription name.</param>
        /// <param name="sqlFilter">The SQL filter.</param>
        /// <returns></returns>
        public ServiceBusTopicBuilder AddSubscription(string name, string sqlFilter = null)
        {
            _topic.Subscriptions.Add(new ServiceBusSubscriptionModel
            {
                Name = name,
                SqlFilter = sqlFilter
            });
            return this;
        }

        internal ServiceBusTopicModel Build()
        {
            return _topic;
        }
    }
}
