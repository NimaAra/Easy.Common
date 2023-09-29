namespace Easy.Common.Extensions;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// A set of extension methods for <see cref="Stream"/>.
/// </summary>
public static class StreamExtensions
{
    private const char CR = '\r';
    private const char LF = '\n';
    private const char NULL = (char)0;

    /// <summary>
    /// Returns the number of lines in the given <paramref name="stream"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static long CountLines(this Stream stream, Encoding? encoding = default)
    {
        long lineCount = 0L;
        byte[] byteBuffer = new byte[1024 * 1024];
        char detectedEOL = NULL;
        char currentChar = NULL;
        int bytesRead;

        if (encoding is null || Equals(encoding, Encoding.ASCII) || Equals(encoding, Encoding.UTF8))
        {
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    currentChar = (char)byteBuffer[i];

                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar is LF or CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }
        } 
        else
        {
            char[] charBuffer = new char[byteBuffer.Length];

            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                int charCount = encoding.GetChars(byteBuffer, 0, bytesRead, charBuffer, 0);

                for (var i = 0; i < charCount; i++)
                {
                    currentChar = charBuffer[i];

                    if (detectedEOL != NULL)
                    {
                        if (currentChar == detectedEOL)
                        {
                            lineCount++;
                        }
                    }
                    else if (currentChar is LF or CR)
                    {
                        detectedEOL = currentChar;
                        lineCount++;
                    }
                }
            }
        }

        if (currentChar != LF && currentChar != CR && currentChar != NULL)
        {
            lineCount++;
        }

        return lineCount;
    }

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="stream">The stream containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/></returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this Stream stream, string name, bool ignoreCase = true) =>
        stream.GetElements(name, new XmlReaderSettings(), ignoreCase);

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="stream">The stream containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="settings">The settings used by the <see cref="XmlReader"/></param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/></returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this Stream stream, string name, XmlReaderSettings settings, bool ignoreCase = true)
    {
        using XmlReader reader = XmlReader.Create(stream, settings);
        foreach (var xElement in reader.GetElements(name, ignoreCase))
        {
            yield return xElement;
        }
    }
}