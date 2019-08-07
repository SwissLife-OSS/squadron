namespace Squadron
{
    /// <summary>
    /// Options when creating a mongo collection
    /// </summary>
    public class CreateCollectionOptions
    {
        /// <summary>
        /// Database options for the collection
        /// </summary>
        public CreateDatabaseOptions DatabaseOptions { get; set; } =
            new CreateDatabaseOptions();

        /// <summary>
        /// The collection name.
        /// Default name is generated if is not set.
        /// </summary>
        public string CollectionName { get; set; } =
            MongoUtils.CreateName("col");
    }
}
