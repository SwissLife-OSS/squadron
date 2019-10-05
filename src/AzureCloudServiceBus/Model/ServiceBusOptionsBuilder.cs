using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class ServiceBusOptionsBuilder
    {
        private ServiceBusModel _options = new ServiceBusModel();
        private List<ServiceBusTopicBuilder> _topics = new List<ServiceBusTopicBuilder>();
        private List<ServiceBusQueueBuilder> _queues = new List<ServiceBusQueueBuilder>();

        public static ServiceBusOptionsBuilder New() => new ServiceBusOptionsBuilder();

        public ServiceBusOptionsBuilder Namespace(string ns)
        {
            _options.Namespace = ns;
            return this;
        }

        public ServiceBusTopicBuilder AddTopic(string name)
        {
            var topicBuilder = ServiceBusTopicBuilder.New(name);
            _topics.Add(topicBuilder);
            return topicBuilder;
        }

        public ServiceBusQueueBuilder AddQueue(string name)
        {
            var queueBuilder = new ServiceBusQueueBuilder(name);
            _queues.Add(queueBuilder);
            return queueBuilder;
        }

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

    public class ServiceBusQueueBuilder
    {

        ServiceBusQueueModel _queue = null;

        public ServiceBusQueueBuilder(string name)
        {
            _queue = new ServiceBusQueueModel(name);
        }

        internal ServiceBusQueueModel Build()
        {
            return _queue;
        }

    }

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

        public static ServiceBusTopicBuilder New()
            => new ServiceBusTopicBuilder();


        public static ServiceBusTopicBuilder New(string name)
            => new ServiceBusTopicBuilder(name);


        public ServiceBusTopicBuilder Name(string name)
        {
            _topic.Name = name;
            return this;
        }

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
