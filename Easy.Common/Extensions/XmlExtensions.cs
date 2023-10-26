﻿namespace Easy.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// Extension methods for classes in the <see cref="System.Xml"/> namespace.
/// </summary>
public static class XmlExtensions
{
    /// <summary>
    /// Sets the default XML namespace of every element in the given XML element
    /// </summary>
    public static void SetDefaultXmlNamespace(this XElement element, XNamespace xmlns)
    {
        Ensure.NotNull(element, nameof(element));
        Ensure.NotNull(xmlns, nameof(xmlns));

        if (element.Name.NamespaceName == string.Empty)
        {
            element.Name = xmlns + element.Name.LocalName;
        }

        foreach (var e in element.Elements())
        {
            e.SetDefaultXmlNamespace(xmlns);
        }
    }

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="reader">The <see cref="XmlReader"/> used to read the XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/></returns>
    public static IEnumerable<XElement> GetElements(this XmlReader reader, string name, bool ignoreCase = true)
    {
        Ensure.NotNull(reader, nameof(reader));
        Ensure.NotNull(name, nameof(name));

        var compPolicy = ignoreCase
            ? StringComparison.InvariantCultureIgnoreCase
            : StringComparison.InvariantCulture;

        reader.MoveToElement();
        while (reader.Read())
        {
            while (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(name, compPolicy))
            {
                yield return (XElement)XNode.ReadFrom(reader);
            }
        }
    }

    /// <summary>
    /// Converts the content of the given <paramref name="reader"/> to <see cref="DynamicDictionary"/>.
    /// </summary>
    public static DynamicDictionary ToDynamic(this XmlReader reader, bool ignoreCase = true)
    {
        Ensure.NotNull(reader, nameof(reader));

        var result = new DynamicDictionary(ignoreCase);
        var elements = new List<XElement>();
        result["Elements"] = elements;

        reader.MoveToElement();
        while (reader.Read())
        {
            while (reader.NodeType == XmlNodeType.Element)
            {
                var element = (XElement)XNode.ReadFrom(reader);
                elements.Add(element);
            }
        }

        return result;
    }
}