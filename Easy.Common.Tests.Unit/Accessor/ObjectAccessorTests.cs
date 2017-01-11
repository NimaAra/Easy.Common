namespace Easy.Common.Tests.Unit.Accessor
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using Accessor = Easy.Common.Accessor;

    [TestFixture]
    public sealed class ObjectAccessorTests
    {
        [Test]
        public void When_creating_object_accessor_with_default_flags()
        {
            var parent = new Parent();
            var parentAccessor = Accessor.CreateAccessor(parent.GetType());
            parentAccessor.ShouldBeOfType<ObjectAccessor>();

            parentAccessor.ShouldNotBeNull();
            parentAccessor.Properties.ShouldNotBeNull();
            parentAccessor.Properties.Length.ShouldBe(2);

            parentAccessor.Properties[0].Name.ShouldBe("Name");
            parentAccessor.Properties[0].PropertyType.ShouldBe(typeof(string));

            parentAccessor.Properties[1].Name.ShouldBe("Age");
            parentAccessor.Properties[1].PropertyType.ShouldBe(typeof(int));

            parentAccessor[parent, "Name"].ShouldBeNull();
            parentAccessor[parent, "Name"] = "Foo";
            parentAccessor[parent, "Name"].ShouldBe("Foo");

            parentAccessor[parent, "Age"].ShouldBe(0);
            parentAccessor[parent, "Age"] = 10;
            parentAccessor[parent, "Age"].ShouldBe(10);

            var child = new Child();
            var childAccessor = Accessor.CreateAccessor(child.GetType());
            childAccessor.ShouldBeOfType<ObjectAccessor>();

            childAccessor.ShouldNotBeNull();
            childAccessor.Properties.ShouldNotBeNull();
            childAccessor.Properties.Length.ShouldBe(3);

            childAccessor.Properties[0].Name.ShouldBe("ChildName");
            childAccessor.Properties[0].PropertyType.ShouldBe(typeof(string));

            childAccessor.Properties[1].Name.ShouldBe("Name");
            childAccessor.Properties[1].PropertyType.ShouldBe(typeof(string));

            childAccessor.Properties[2].Name.ShouldBe("Age");
            childAccessor.Properties[2].PropertyType.ShouldBe(typeof(int));

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
            var parent = new Parent();
            var parentAccessor = Accessor.CreateAccessor(parent.GetType(), true, true);
            parentAccessor.ShouldBeOfType<ObjectAccessor>();

            parentAccessor.ShouldNotBeNull();
            parentAccessor.Properties.ShouldNotBeNull();
            parentAccessor.Properties.Length.ShouldBe(3);

            parentAccessor.Properties[0].Name.ShouldBe("Name");
            parentAccessor.Properties[0].PropertyType.ShouldBe(typeof(string));

            parentAccessor.Properties[1].Name.ShouldBe("Age");
            parentAccessor.Properties[1].PropertyType.ShouldBe(typeof(int));

            parentAccessor.Properties[2].Name.ShouldBe("Job");
            parentAccessor.Properties[2].PropertyType.ShouldBe(typeof(string));

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
            var childAccessor = Accessor.CreateAccessor(typeof(Child), true, true);
            childAccessor.ShouldBeOfType<ObjectAccessor>();

            childAccessor.ShouldNotBeNull();
            childAccessor.Properties.ShouldNotBeNull();
            childAccessor.Properties.Length.ShouldBe(3);

            childAccessor.Properties[0].Name.ShouldBe("ChildName");
            childAccessor.Properties[0].PropertyType.ShouldBe(typeof(string));

            childAccessor.Properties[1].Name.ShouldBe("Name");
            childAccessor.Properties[1].PropertyType.ShouldBe(typeof(string));

            childAccessor.Properties[2].Name.ShouldBe("Age");
            childAccessor.Properties[2].PropertyType.ShouldBe(typeof(int));

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
            var parent = new Parent();
            var parentAccessor = Accessor.CreateAccessor(parent.GetType());

            var child = new Child();

            Should.Throw<InvalidOperationException>(() => { var ignore = parentAccessor[child, "ChildName"]; })
                .Message.ShouldBe("Unable to find property: ChildName.");
        }

        [Test]
        public void When_using_child_accessor_to_access_parent_properties()
        {
            var child = new Child();
            var childAccessor = Accessor.CreateAccessor(child.GetType());

            var parent = new Parent();

            Should.Throw<NullReferenceException>(() => { var ignore = childAccessor[parent, "Name"]; });
        }

        private class Parent
        {
            public string Name { get; set; }
            public int Age { get; set; }
            private string Job { get; set; }

            public string GetJob() => Job;
        }

        private sealed class Child : Parent
        {
            public string ChildName { get; set; } = "Bar";
        }
    }
}