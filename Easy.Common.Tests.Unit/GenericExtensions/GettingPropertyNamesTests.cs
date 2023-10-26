namespace Easy.Common.Tests.Unit.GenericExtensions;

using System.Dynamic;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class GettingPropertyNamesTests
{
    [Test]
    public void When_getting_property_names_of_expando_object()
    {
        dynamic expando = new ExpandoObject();
        expando.Name = "Foo";
        expando.Age = 1;

        var propertyNames = ((object)expando).GetPropertyNames(true, false);
        propertyNames.ShouldNotBeNull();
        propertyNames.Length.ShouldBe(2);
        propertyNames[0].ShouldBe("Name");
        propertyNames[1].ShouldBe("Age");
    }

    [Test]
    public void When_getting_property_names_of_a_class()
    {
        var model = new SomeClass();

        var propertyNames = ((object)model).GetPropertyNames(true, false);
        propertyNames.ShouldNotBeNull();
        propertyNames.Length.ShouldBe(2);
        propertyNames[0].ShouldBe("Name");
        propertyNames[1].ShouldBe("Age");
    }

    [Test]
    public void When_getting_property_names_of_a_struct()
    {
        var model = new SomeStruct();

        var propertyNames = ((object)model).GetPropertyNames(true, false);
        propertyNames.ShouldNotBeNull();
        propertyNames.Length.ShouldBe(2);
        propertyNames[0].ShouldBe("Name");
        propertyNames[1].ShouldBe("Age");
    }

    private sealed class SomeClass
    {
        public string Name { get; set; }
        public int Age { get; set; }
        private string AnotherName { get; set; }
        public static int AnotherAge { get; set; }
    }

    private struct SomeStruct
    {
        public string Name { get; set; }
        public int Age { get; set; }
        private string AnotherName { get; set; }
        public static int AnotherAge { get; set; }
    }
}