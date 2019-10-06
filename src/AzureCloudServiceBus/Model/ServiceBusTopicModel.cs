using System.Collections.Generic;

namespace Squadron
{
    /// <summary>
    /// ServiceBusTopic
    /// </summary>
    public class ServiceBusTopicModel
    {
        internal ServiceBusTopicModel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusTopicModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceBusTopicModel(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the generated name
        /// </summary>
        /// <value>
        /// The name of the created.
        /// </value>
        internal string CreatedName { get; set; }

        /// <summary>
        /// Gets or sets the subscriptions.
        /// </summary>
        /// <value>
        /// The subscriptions.
        /// </value>
        public List<ServiceBusSubscriptionModel> Subscriptions { get; set; }
            = new List<ServiceBusSubscriptionModel>();
    }
}
