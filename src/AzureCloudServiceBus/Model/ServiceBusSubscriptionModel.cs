namespace Squadron
{
    /// <summary>
    /// ServiceBusSubscription model
    /// </summary>
    public class ServiceBusSubscriptionModel
    {
        /// <summary>
        /// Gets or sets the subscription name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the SQL filter.
        /// </summary>
        /// <value>
        /// The SQL filter.
        /// </value>
        public string SqlFilter { get; set; }
    }

}
