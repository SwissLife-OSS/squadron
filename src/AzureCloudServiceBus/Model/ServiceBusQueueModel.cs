namespace Squadron
{
    /// <summary>
    ///  ServiceBusQueue model
    /// </summary>
    public class ServiceBusQueueModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusQueueModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ServiceBusQueueModel(string name)
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
        /// Gets or sets the  generated name
        /// </summary>
        /// <value>
        /// The name of the created.
        /// </value>
        internal string CreatedName { get; set; }
    }

}
