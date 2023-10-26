namespace Easy.Common.Tests.Unit.XML;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public class XmlToDynamicTests
{
    [Test]
    public void When_converting_empty_xml_to_dynamic_dictionary()
    {
        const string XmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>";

        using (var stringReader = new StringReader(XmlContent))
        using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings()))
        {
            Should.Throw<XmlException>(() => xmlReader.ToDynamic())
                .Message.ShouldBe("Root element is missing.");
        }
    }

    [Test]
    public void When_converting_simple_xml_to_dynamic_dictionary()
    {
        const string XmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SampleModel>
  <Id>123</Id>
  <Category>Some category</Category>
</SampleModel>";

        using (var stringReader = new StringReader(XmlContent))
        using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings()))
        {
            var result = xmlReader.ToDynamic();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result["Elements"].ShouldNotBeNull();
            result["elements"].ShouldNotBeNull();
            result["Elements"].ShouldBeOfType<List<XElement>>();

            var elements = result["Elements"] as List<XElement>;
            elements.ShouldNotBeNull();
            elements.Count.ShouldBe(1);
            elements[0].Name.ShouldBe("SampleModel");
            elements[0].HasElements.ShouldBeTrue();
            elements[0].Element("Id").ShouldNotBeNull();
            elements[0].Element("Id").Value.ShouldBe("123");
            elements[0].Element("Id").HasAttributes.ShouldBeFalse();

            elements[0].Element("Category").ShouldNotBeNull();
            elements[0].Element("Category").Value.ShouldBe("Some category");
            elements[0].Element("Category").HasAttributes.ShouldBeFalse();
        }
    }

    [Test]
    public void When_converting_xml_with_attributes_to_dynamic_dictionary()
    {
        const string XmlContent = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SampleModel xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <Id attr1=""foo"" attr2=""bar"">123</Id>
  <Category>Some category</Category>
</SampleModel>";

        using (var stringReader = new StringReader(XmlContent))
        using (var xmlReader = XmlReader.Create(stringReader, new XmlReaderSettings()))
        {
            var result = xmlReader.ToDynamic();

            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result["Elements"].ShouldNotBeNull();
            result["elements"].ShouldNotBeNull();
            result["Elements"].ShouldBeOfType<List<XElement>>();

            var elements = result["Elements"] as List<XElement>;
            elements.ShouldNotBeNull();
            elements.Count.ShouldBe(1);
            elements[0].Name.ShouldBe("SampleModel");
            elements[0].HasElements.ShouldBeTrue();
            elements[0].Element("Id").ShouldNotBeNull();
            elements[0].Element("Id").Value.ShouldBe("123");
            elements[0].Element("Id").HasAttributes.ShouldBeTrue();
            elements[0].Element("Id").Attributes().Count().ShouldBe(2);
            elements[0].Element("Id").Attribute("attr1").Value.ShouldBe("foo");
            elements[0].Element("Id").Attribute("attr2").Value.ShouldBe("bar");

            elements[0].Element("Category").ShouldNotBeNull();
            elements[0].Element("Category").Value.ShouldBe("Some category");
            elements[0].Element("Category").HasAttributes.ShouldBeFalse();
        }
    }
}