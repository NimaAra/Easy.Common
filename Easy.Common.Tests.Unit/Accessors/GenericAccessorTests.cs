namespace Easy.Common.Tests.Unit.Accessors;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Shouldly;

[TestFixture]
[SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
public sealed class GenericAccessorTests
{
    [Test]
    public void When_creating_generic_accessor_with_default_flags()
    {
        var parent = new Parent();
        GenericAccessor<Parent> parentAccessor = Accessor.Build<Parent>();
        parentAccessor.ShouldBeOfType<GenericAccessor<Parent>>();
        parentAccessor.Type.ShouldBe(typeof(Parent));
        parentAccessor.IgnoreCase.ShouldBe(false);
        parentAccessor.IncludesNonPublic.ShouldBe(false);

        parentAccessor.ShouldNotBeNull();
        parentAccessor.Properties.ShouldNotBeNull();
        parentAccessor.Properties.Length.ShouldBe(2);

        parentAccessor.Properties.Single(x => x.Name == "Name").PropertyType.ShouldBe(typeof(string));
        parentAccessor.Properties.Single(x => x.Name == "Age").PropertyType.ShouldBe(typeof(int));

        parentAccessor[parent, "Name"].ShouldBeNull();
        parentAccessor[parent, "Name"] = "Foo";
        parentAccessor[parent, "Name"].ShouldBe("Foo");

        parentAccessor[parent, "Age"].ShouldBe(0);
        parentAccessor[parent, "Age"] = 10;
        parentAccessor[parent, "Age"].ShouldBe(10);

        var child = new Child();
        GenericAccessor<Child> childAccessor = Accessor.Build<Child>();
        childAccessor.ShouldBeOfType<GenericAccessor<Child>>();
        childAccessor.Type.ShouldBe(typeof(Child));
        childAccessor.IgnoreCase.ShouldBe(false);
        childAccessor.IncludesNonPublic.ShouldBe(false);

        childAccessor.ShouldNotBeNull();
        childAccessor.Properties.ShouldNotBeNull();
        childAccessor.Properties.Length.ShouldBe(3);

        childAccessor.Properties.Single(x => x.Name == "ChildName").PropertyType.ShouldBe(typeof(string));
        childAccessor.Properties.Single(x => x.Name == "Name").PropertyType.ShouldBe(typeof(string));
        childAccessor.Properties.Single(x => x.Name == "Age").PropertyType.ShouldBe(typeof(int));

        childAccessor[child, "ChildName"].ShouldBe("Bar");
        childAccessor[child, "ChildName"] = "BarBar";
        childAccessor[child, "ChildName"].ShouldBe("BarBar");

        childAccessor[child, "Name"].ShouldBeNull();
        childAccessor[child, "Name"] = "Foo";
        childAccessor[child, "Name"].ShouldBe("Foo");
    }

    [Test]
    public void When_creating_generic_accessor_with_custom_flags()
    {
        var parent = new Parent();
        GenericAccessor<Parent> parentAccessor = Accessor.Build<Parent>(true, true);
        parentAccessor.ShouldBeOfType<GenericAccessor<Parent>>();
        parentAccessor.Type.ShouldBe(typeof(Parent));
        parentAccessor.IgnoreCase.ShouldBe(true);
        parentAccessor.IncludesNonPublic.ShouldBe(true);

        parentAccessor.ShouldNotBeNull();
        parentAccessor.Properties.ShouldNotBeNull();
        parentAccessor.Properties.Length.ShouldBe(3);

        parentAccessor.Properties.Single(x => x.Name == "Name").PropertyType.ShouldBe(typeof(string));
        parentAccessor.Properties.Single(x => x.Name == "Age").PropertyType.ShouldBe(typeof(int));
        parentAccessor.Properties.Single(x => x.Name == "Job").PropertyType.ShouldBe(typeof(string));

        parentAccessor[parent, "Name"].ShouldBeNull();
        parentAccessor[parent, "Name"] = "Foo";
        parentAccessor[parent, "Name"].ShouldBe("Foo");

        parentAccessor[parent, "Age"].ShouldBe(0);
        parentAccessor[parent, "Age"] = 10;
        parentAccessor[parent, "Age"].ShouldBe(10);

        parentAccessor[parent, "Job"].ShouldBeNull();
        parentAccessor[parent, "Job"] = "Clown";
        parentAccessor[parent, "Job"].ShouldBe("Clown");

        parentAccessor[parent, "NAME"].ShouldBe("Foo");
        parentAccessor[parent, "name"] = "Foo Foo";
        parentAccessor[parent, "naME"].ShouldBe("Foo Foo");

        var child = new Child();
        GenericAccessor<Child> childAccessor = Accessor.Build<Child>(true, true);
        childAccessor.ShouldBeOfType<GenericAccessor<Child>>();
        childAccessor.Type.ShouldBe(typeof(Child));
        childAccessor.IgnoreCase.ShouldBe(true);
        childAccessor.IncludesNonPublic.ShouldBe(true);

        childAccessor.ShouldNotBeNull();
        childAccessor.Properties.ShouldNotBeNull();
        childAccessor.Properties.Length.ShouldBe(3);

        childAccessor.Properties.Single(x => x.Name == "ChildName").PropertyType.ShouldBe(typeof(string));
        childAccessor.Properties.Single(x => x.Name == "Name").PropertyType.ShouldBe(typeof(string));
        childAccessor.Properties.Single(x => x.Name == "Age").PropertyType.ShouldBe(typeof(int));

        childAccessor[child, "ChildName"].ShouldBe("Bar");
        childAccessor[child, "ChIldNAme"] = "BarBar";
        childAccessor[child, "ChildName"].ShouldBe("BarBar");

        childAccessor[child, "Name"].ShouldBeNull();
        childAccessor[child, "Name"] = "Foo";
        childAccessor[child, "Name"].ShouldBe("Foo");
    }

    [Test]
    public void When_using_parent_accessor_to_access_child_properties()
    {
        GenericAccessor<Parent> parentAccessor = Accessor.Build<Parent>();
        parentAccessor.ShouldBeOfType<GenericAccessor<Parent>>();
        parentAccessor.Type.ShouldBe(typeof(Parent));
        parentAccessor.IgnoreCase.ShouldBe(false);
        parentAccessor.IncludesNonPublic.ShouldBe(false);

        var child = new Child();

        Should.Throw<ArgumentException>(() => { var _ = parentAccessor[child, "ChildName"]; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.GenericAccessorTests+Child` does not have a property named: `ChildName` that supports reading.");

        Should.Throw<ArgumentException>(() => { parentAccessor[child, "ChildName"] = "foo"; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.GenericAccessorTests+Child` does not have a property named: `ChildName` that supports writing.");
    }

    [Test]
    public void When_testing_public_members()
    {
        GenericAccessor<Parent> accessor = Accessor.Build<Parent>();
        accessor.ShouldBeOfType<GenericAccessor<Parent>>();
        accessor.Type.ShouldBe(typeof(Parent));
        accessor.IgnoreCase.ShouldBe(false);
        accessor.IncludesNonPublic.ShouldBe(false);

        accessor.Properties.ShouldNotBeNull();
        accessor.Properties.Length.ShouldBe(2);

        var instance = new Parent();

        accessor[instance, "Name"] = "John";
        instance.Name.ShouldBe("John");

        accessor.TryGet<string>(instance, "Name", out string result1).ShouldBeTrue();
        result1.ShouldBe("John");

        accessor.TrySet(instance, "Age", (object)10).ShouldBeFalse();
        accessor.TrySet(instance, "Age", 10).ShouldBeTrue();

        accessor.TryGet(instance, "Age", out int result2).ShouldBeTrue();
        result2.ShouldBe(10);

        accessor.TrySet(instance, "Name", "Bobby").ShouldBeTrue();
        accessor[instance, "Name"].ShouldBe("Bobby");
        accessor.TryGet(instance, "Name", out string result3).ShouldBeTrue();
        result3.ShouldBe("Bobby");

        accessor.TrySet(instance, "Name", "Joey").ShouldBeTrue();
        accessor[instance, "Name"].ShouldBe("Joey");
        accessor.TryGet(instance, "Name", out string result4).ShouldBeTrue();
        result4.ShouldBe("Joey");

        accessor.TryGet(instance, "Name", out string result5).ShouldBeTrue();
        result5.ShouldBe("Joey");
    }

    [Test]
    public void When_testing_special_cases()
    {
        GenericAccessor<SpecialCase> accessor = Accessor.Build<SpecialCase>();
        accessor.ShouldBeOfType<GenericAccessor<SpecialCase>>();
        accessor.Type.ShouldBe(typeof(SpecialCase));
        accessor.IgnoreCase.ShouldBe(false);
        accessor.Properties.Length.ShouldBe(2);
        accessor.Properties.ShouldContain(x => x.Name == "GetterOnly");
        accessor.Properties.ShouldContain(x => x.Name == "SetterOnly");

        var instance = new SpecialCase();

        Should.Throw<ArgumentException>(() => { var _ = accessor[instance, "SetterOnly"]; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.GenericAccessorTests+SpecialCase` does not have a property named: `SetterOnly` that supports reading.");

        Should.Throw<ArgumentException>(() => accessor[instance, "GetterOnly"] = "bar")
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.GenericAccessorTests+SpecialCase` does not have a property named: `GetterOnly` that supports writing.");

        accessor.TrySet(instance, "GetterOnly", (object) "Baz").ShouldBeFalse();

        accessor.TryGet(instance, "SetterOnly", out object tmpResult1).ShouldBeFalse();
        tmpResult1.ShouldBeNull();

        accessor.TryGet(instance, "SetterOnly", out string tmpResult2).ShouldBeFalse();
        tmpResult2.ShouldBeNull();

        accessor.TrySet(instance, "GetterOnly", "Boo").ShouldBeFalse();

        accessor[instance, "SetterOnly"] = "Foo";
        accessor[instance, "GetterOnly"].ShouldBe("Foo");

        accessor.TrySet(instance, "SetterOnly", "Baz").ShouldBeTrue();

        accessor.TryGet(instance, "GetterOnly", out string result1).ShouldBeTrue();
        result1.ShouldBe("Baz");

        accessor.TrySet(instance, "SetterOnly", "Boo");
            
        accessor.TryGet(instance, "GetterOnly", out string result2).ShouldBeTrue();
        result2.ShouldBe("Boo");
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private class Parent
    {
        public string Name { get; set; }
        public int Age { get; set; }
        private string Job { get; set; }

        public string GetJob() => Job;
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private sealed class Child : Parent
    {
        public string ChildName { get; set; } = "Bar";
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private sealed class SpecialCase
    {
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public string GetterOnly => _stuff;

        private string _stuff;
        public string SetterOnly { set => _stuff = value; }
    }
}