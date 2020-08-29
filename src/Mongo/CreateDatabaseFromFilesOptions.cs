using System.IO;

namespace Squadron
{
    /// <summary>
    /// Options when creating a mongo database from files (collections)
    /// </summary>
    public class CreateDatabaseFromFilesOptions
    {
        /// <summary>
        /// Source file information
        /// </summary>
        public FileInfo[] Files { get; set; }

        /// <summary>
        /// Destination path (unix).
        /// Default destination is "/tmp"
        /// </summary>
        internal string Destination { get; set; } =
            "/tmp";

        /// <summary>
        /// Collection options for the imported file content.
        /// </summary>
        public CreateDatabaseOptions DatabaseOptions { get; set; } =
            new CreateDatabaseOptions();

        /// <summary>
        /// Custom arguments for mongoimport command.
        /// Args pattern should look like {parameter}={value}
        /// (e.g. --type=tsv)
        /// </summary>
        public string[] CustomImportArgs { get; set; } =
            new string[0];
    }
}
