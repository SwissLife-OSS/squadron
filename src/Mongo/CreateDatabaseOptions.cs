namespace Squadron
{
    /// <summary>
    /// Options when creating a mongo database
    /// </summary>
    public class CreateDatabaseOptions
    {
        /// <summary>
        /// The database name.
        /// Default name is generated if is not set.
        /// </summary>
        public string DatabaseName { get; set; } =
            UniqueNameGenerator.Create("db");
    }
}
