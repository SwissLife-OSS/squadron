namespace Squadron;

/// <summary>
/// Request for creating a document into the elasticsearch cluster
/// </summary>
/// <typeparam name="TDocument">The type of the document.</typeparam>
public class CreateDocumentRequest<TDocument>
{
    /// <summary>
    /// Gets or sets the id of the document.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    public TDocument Document { get; set; }
}