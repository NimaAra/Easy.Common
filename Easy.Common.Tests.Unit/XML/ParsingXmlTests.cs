namespace Easy.Common.Tests.Unit.XML;

using System;
using System.IO;
using System.Linq;
using System.Xml;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class ParsingXmlTests
{
    private readonly string _pathToXml = 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"XML\Sample.xml");

    [Test]
    public void When_parsing_xml_from_file_case_insensitive()
    {
        using XmlReader reader = XmlReader.Create(_pathToXml);
        reader.GetElements("book").Count().ShouldBe(12);
    }

    [Test]
    public void When_parsing_xml_from_file_case_sensitive()
    {
        using XmlReader reader = XmlReader.Create(_pathToXml);
        reader.GetElements("booK", false).Count().ShouldBe(0);
    }

    [Test]
    public void When_parsing_xml_from_text_fragment_case_insensitive()
    {
        string xml = File.ReadAllText(_pathToXml);
        xml.GetElements("book").Count().ShouldBe(12);
    }

    [Test]
    public void When_parsing_xml_from_text_fragment_case_sensitive()
    {
        string xml = File.ReadAllText(_pathToXml);
        xml.GetElements("booK", false).Count().ShouldBe(0);
    }

    [Test]
    public void When_parsing_xml_from_stream_case_insensitive()
    {
        FileStream stream = File.Open(_pathToXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.GetElements("book").Count().ShouldBe(12);
    }

    [Test]
    public void When_parsing_xml_from_stream_case_sensitive()
    {
        FileStream stream = File.Open(_pathToXml, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.GetElements("booK", false).Count().ShouldBe(0);
    }
}