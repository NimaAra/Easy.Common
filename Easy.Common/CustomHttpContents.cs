namespace Easy.Common;

using System.Net.Http;
using System.Text;

/// <summary>
/// Provides HTTP content based on a <c>JSON</c> string.
/// </summary>
public sealed class JSONContent : StringContent
{
    private const string JSONMime = "application/json";

    /// <summary>
    /// Creates an instance of the <see cref="JSONContent"/> with the default encoding of <see cref="Encoding.UTF8"/>.
    /// </summary>
    public JSONContent(string jsonContent) : base(jsonContent, Encoding.UTF8, JSONMime) {}

    /// <summary>
    /// Creates an instance of the <see cref="JSONContent"/> with the given <paramref name="encoding"/>.
    /// </summary>
    public JSONContent(string jsonContent, Encoding encoding) : base(jsonContent, encoding, JSONMime) {}
}

/// <summary>
/// Provides HTTP content based on an <c>XML</c> string.
/// </summary>
public sealed class XMLContent : StringContent
{
    private const string XMLMime = "application/xml";

    /// <summary>
    /// Creates an instance of the <see cref="XMLContent"/> with the default encoding of <see cref="Encoding.UTF8"/>.
    /// </summary>
    public XMLContent(string xmlContent) : base(xmlContent, Encoding.UTF8, XMLMime) { }
        
    /// <summary>
    /// Creates an instance of the <see cref="XMLContent"/> with the given <paramref name="encoding"/>.
    /// </summary>
    public XMLContent(string xmlContent, Encoding encoding) : base(xmlContent, encoding, XMLMime) {}
}