using System.Collections.Generic;

namespace Squadron
{
    public class ServiceBusTopicModel
    {
        internal ServiceBusTopicModel()
        {

        }

        public ServiceBusTopicModel(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        internal string CreatedName { get; set; }

        public List<ServiceBusSubscriptionModel> Subscriptions { get; set; }
            = new List<ServiceBusSubscriptionModel>();
    }

    public class ServiceBusQueueModel
    {
        public ServiceBusQueueModel(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        internal string CreatedName { get; set; }

    }

    public class ServiceBusSubscriptionModel
    {
        public string Name { get; set; }

        public string SqlFilter { get; set; }
    }

}
