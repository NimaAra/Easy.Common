namespace Easy.Common.Extensions
{
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
        private const char NL = '\n';
        private const char NULL = (char)0;
        
        /// <summary>
        /// Detects the text encoding for the given <paramref name="stream"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static Encoding DetectEncoding(this Stream stream, Encoding defaultEncodingIfNoBOM)
        {
            Ensure.NotNull(stream, nameof(stream));
            
            var startPos = stream.Position;

            try
            {
                using (var reader = new StreamReader(stream, defaultEncodingIfNoBOM, true, 1, true))
                {
                    var _ = reader.Peek();
                    return reader.CurrentEncoding;
                }
            } finally
            {
                if (stream.CanSeek) { stream.Position = startPos; }
            }
        }
        
        /// <summary>
        /// Returns the number of lines in the given <paramref name="stream"/>.
        /// </summary>
        [DebuggerStepThrough]
        public static long CountLines(this Stream stream)
        {
            Ensure.NotNull(stream, nameof(stream));

            var lineCount = 0L;

            var byteBuffer = new byte[1024 * 1024];
            var prevChar = NULL;
            var pendingTermination = false;

            int bytesRead;
            while ((bytesRead = stream.Read(byteBuffer, 0, byteBuffer.Length)) > 0)
            {
                for (var i = 0; i < bytesRead; i++)
                {
                    var currentChar = (char)byteBuffer[i];
                    switch (currentChar)
                    {
                        case NULL:
                        case NL when prevChar == CR:
                            continue;
                        case CR:
                        case NL when prevChar != CR:
                            lineCount++;
                            pendingTermination = false;
                            break;
                        default:
                            if (!pendingTermination)
                            {
                                pendingTermination = true;
                            }
                            break;
                    }
                    prevChar = currentChar;
                }
            }

            if (pendingTermination) { lineCount++; }
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
        public static IEnumerable<XElement> GetElements(
            this Stream stream, XName name, bool ignoreCase = true)
        {
            Ensure.NotNull(stream, nameof(stream));
            return stream.GetElements(name, new XmlReaderSettings(), ignoreCase);
        }

        /// <summary>
        /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
        /// </summary>
        /// <param name="stream">The stream containing XML</param>
        /// <param name="name">The name of the elements to return</param>
        /// <param name="settings">The settings used by the <see cref="XmlReader"/></param>
        /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
        /// <returns>The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/></returns>
        [DebuggerStepThrough]
        public static IEnumerable<XElement> GetElements(
            this Stream stream, XName name, XmlReaderSettings settings, bool ignoreCase = true)
        {
            Ensure.NotNull(stream, nameof(stream));
            Ensure.NotNull(name, nameof(name));
            Ensure.NotNull(settings, nameof(settings));

            using (var reader = XmlReader.Create(stream, settings))
            {
                foreach (var xElement in reader.GetEelements(name, ignoreCase))
                {
                    yield return xElement;
                }
            }
        }
    }
}