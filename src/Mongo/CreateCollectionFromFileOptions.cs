using System.IO;

namespace Squadron;

/// <summary>
/// Options when creating a mongo collection from given file
/// </summary>
public class CreateCollectionFromFileOptions
{
    /// <summary>
    /// Source file information
    /// </summary>
    public FileInfo File { get; set; }

    /// <summary>
    /// Destination path (unix).
    /// Default destination is "/tmp"
    /// </summary>
    internal string Destination { get; set; } =
        "/tmp";

    /// <summary>
    /// Collection options for the imported file content.
    /// </summary>
    public CreateCollectionOptions CollectionOptions { get; set; } =
        new CreateCollectionOptions();

    /// <summary>
    /// Custom arguments for mongoimport command.
    /// Args pattern should look like {parameter}={value}
    /// (e.g. --type=tsv)
    /// </summary>
    public string[] CustomImportArgs { get; set; } =
        new string[0];
}