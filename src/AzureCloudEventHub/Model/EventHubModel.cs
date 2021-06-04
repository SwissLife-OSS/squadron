namespace Squadron
{
    /// <summary>
    ///  ServiceBusQueue model
    /// </summary>
    public class EventHubModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHubModel"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EventHubModel(string name)
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
        /// Gets or sets the created name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        internal string CreatedName { get; set; }
    }

}
