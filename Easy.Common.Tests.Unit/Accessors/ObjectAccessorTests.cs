namespace Easy.Common.Tests.Unit.Accessors;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class ObjectAccessorTests
{
    [Test]
    public void When_creating_object_accessor_with_default_flags()
    {
        Parent parent = new();
        ObjectAccessor parentAccessor = Accessor.Build(parent);
        parentAccessor.ShouldBeOfType<ObjectAccessor>();
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

        Child child = new();
        ObjectAccessor childAccessor = Accessor.Build(child);
        childAccessor.ShouldBeOfType<ObjectAccessor>();
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
    public void When_creating_object_accessor_with_custom_flags()
    {
        Parent parent = new();
        ObjectAccessor parentAccessor = Accessor.Build(parent, true, true);
        parentAccessor.ShouldBeOfType<ObjectAccessor>();
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

        Child child = new();
        ObjectAccessor childAccessor = Accessor.Build(typeof(Child), true, true);
        childAccessor.ShouldBeOfType<ObjectAccessor>();
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
        Parent parent = new();
        ObjectAccessor parentAccessor = Accessor.Build(parent);
        parentAccessor.Type.ShouldBe(typeof(Parent));
        parentAccessor.IgnoreCase.ShouldBe(false);
        parentAccessor.IncludesNonPublic.ShouldBe(false);

        Child child = new();

        Should.Throw<ArgumentException>(() => { var _ = parentAccessor[child, "ChildName"]; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.ObjectAccessorTests+Child` does not have a property named: `ChildName` that supports reading.");

        Should.Throw<ArgumentException>(() => { parentAccessor[child, "ChildName"] = "foo"; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.ObjectAccessorTests+Child` does not have a property named: `ChildName` that supports writing.");
    }

    [Test]
    public void When_using_child_accessor_to_access_parent_properties()
    {
        Child child = new();
        ObjectAccessor childAccessor = Accessor.Build(child);
        childAccessor.Type.ShouldBe(typeof(Child));
        childAccessor.IgnoreCase.ShouldBe(false);
        childAccessor.IncludesNonPublic.ShouldBe(false);

        Parent parent = new();

        Should.Throw<NullReferenceException>(() => { var _ = childAccessor[parent, "Name"]; });
    }

    [Test]
    public void When_setting_invalid_values()
    {
        Parent instance = new();

        ObjectAccessor accessor = Accessor.Build(instance);
        accessor.Type.ShouldBe(typeof(Parent));
        accessor.IgnoreCase.ShouldBe(false);
        accessor.IncludesNonPublic.ShouldBe(false);
            
        accessor[instance, "Name"] = 10;
        instance.Name.ShouldBeNull();

        Should.Throw<InvalidCastException>(() => accessor[instance, "Age"] = "10");
    }

    [Test]
    public void When_testing_special_cases()
    {
        SpecialCase instance = new();

        ObjectAccessor accessor = Accessor.Build(instance);
        accessor.Type.ShouldBe(typeof(SpecialCase));
        accessor.IgnoreCase.ShouldBe(false);
        accessor.Properties.Length.ShouldBe(2);
        accessor.Properties.ShouldContain(x => x.Name == "GetterOnly");
        accessor.Properties.ShouldContain(x => x.Name == "SetterOnly");
            
        Should.Throw<ArgumentException>(() => { var _ = accessor[instance, "SetterOnly"]; })
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.ObjectAccessorTests+SpecialCase` does not have a property named: `SetterOnly` that supports reading.");

        Should.Throw<ArgumentException>(() => accessor[instance, "GetterOnly"] = "bar")
            .Message.ShouldBe("Type: `Easy.Common.Tests.Unit.Accessors.ObjectAccessorTests+SpecialCase` does not have a property named: `GetterOnly` that supports writing.");

        accessor[instance, "SetterOnly"] = "Foo";
        accessor[instance, "GetterOnly"].ShouldBe("Foo");
    }

    [Test]
    public void When_reusing_object_accessor_on_different_instances_of_the_same_type()
    {
        Parent instanceOne = new();
        Parent instanceTwo = new();

        ObjectAccessor accessor = Accessor.Build(instanceOne);
        instanceOne.Name.ShouldBeNull();
        instanceOne.Age.ShouldBe(0);

        accessor[instanceOne, "Name"] = "Foo1";
        accessor[instanceOne, "Age"] = 1;

        accessor[instanceTwo, "Name"] = "Foo2";
        accessor[instanceTwo, "Age"] = 2;

        instanceOne.Name.ShouldBe("Foo1");
        instanceOne.Age.ShouldBe(1);

        instanceTwo.Name.ShouldBe("Foo2");
        instanceTwo.Age.ShouldBe(2);
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