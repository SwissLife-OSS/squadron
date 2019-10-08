using System;
using System.IO;

namespace Squadron
{
    /// <summary>
    /// Copy context has information about fileName and filePath
    /// </summary>
    public class CopyContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CopyContext"/> class.
        /// </summary>
        /// <param name="source">Source file path from Windows environment</param>
        /// <param name="destination">Destination file path to Linux environment</param>
        public CopyContext(string source, string destination)
        {
            Source = source;
            Destination = destination;
            DestinationFolder = Path.GetDirectoryName(destination);
        }

        /// <summary>
        /// Gets the source file path.
        /// </summary>
        public string Source { get; }

        /// <summary>
        /// Gets the destination file path.
        /// </summary>
        public string Destination { get; }

        internal string DestinationFolder { get; }

        public static CopyContext CreateFromFileContent(string content,
                                                        string fileExtension,
                                                        string destinationDir)
        {

            var filename = Guid.NewGuid().ToString("N") + $".{fileExtension}";
            var scriptFile = Path.Combine(
                Path.GetTempPath(), filename);
            File.WriteAllText(scriptFile, content);
            return new CopyContext(scriptFile, $"{destinationDir}/{filename}");
        }
    }
}
