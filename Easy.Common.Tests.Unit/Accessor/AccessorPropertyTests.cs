namespace Easy.Common.Tests.Unit.Accessor
{
    using System;
    using System.Reflection;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using Accessor = Easy.Common.Accessor;

    [TestFixture]
    public class AccessorPropertyTests
    {
        [Test]
        public void When_checking_invalid_input()
        {
            string nullStr = null;
            PropertyInfo nullPropInfo = null;

            Should.Throw<ArgumentException>(() => Accessor.CreateSetter<Person, string>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => Accessor.CreateSetter<Person, string>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");

            Should.Throw<ArgumentException>(() => Accessor.CreateGetter<Person, string>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => Accessor.CreateGetter<Person, string>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");

            Should.Throw<ArgumentException>(() => Accessor.CreateSetter<Person>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => Accessor.CreateSetter<Person>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");

            Should.Throw<ArgumentException>(() => Accessor.CreateGetter<Person>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => Accessor.CreateGetter<Person>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");

            Should.Throw<ArgumentException>(() => Accessor.CreateSetter(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");

            Should.Throw<ArgumentException>(() => Accessor.CreateGetter(nullPropInfo))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: propertyInfo");
        }

        [Test]
        public void When_getting_getters_class()
        {
            var nameGetterOne = Accessor.CreateGetter<Person, string>("Name");
            var nameGetterTwo = Accessor.CreateGetter<Person, string>("Name");

            nameGetterOne.ShouldNotBeSameAs(nameGetterTwo);

            var jobGetter = Accessor.CreateGetter<Person, string>("Job", true);

            jobGetter.ShouldNotBe(nameGetterOne);
            jobGetter.ShouldNotBe(nameGetterTwo);
        }

        [Test]
        public void When_getting_setters_class()
        {
            var nameSetterOne = Accessor.CreateSetter<Person, string>("Name");
            var nameSetterTwo = Accessor.CreateSetter<Person, string>("Name");

            nameSetterOne.ShouldNotBeSameAs(nameSetterTwo);

            var jobSetter = Accessor.CreateSetter<Person, string>("Job", true);

            jobSetter.ShouldNotBe(nameSetterOne);
            jobSetter.ShouldNotBe(nameSetterTwo);
        }

        [Test]
        public void When_setting_properties_of_known_instance_and_property_type_class()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            var nameSetter = Accessor.CreateSetter<Person, string>("Name");
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            var jobSetter = Accessor.CreateSetter<Person, string>("Job", true);
            jobSetter(instance, "job");
            instance.GetJob().ShouldBe("job");
        }

        [Test]
        public void When_setting_properties_of_unknown_instance_and_property_type_class()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            PropertyInfo nameProp;
            typeof(Person).TryGetInstanceProperty("Name", out nameProp).ShouldBeTrue();

            var nameSetter = Accessor.CreateSetter(nameProp);
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            PropertyInfo jobProp;
            typeof(Person).TryGetInstanceProperty("Job", out jobProp).ShouldBeTrue();
            var jobSetter = Accessor.CreateSetter(jobProp, true);
            jobSetter(instance, "job");
            instance.GetJob().ShouldBe("job");
        }

        [Test]
        public void When_setting_properties_of_known_instance_but_unknown_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            var nameSetter = Accessor.CreateSetter<Person>("Name");
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            var jobSetter = Accessor.CreateSetter<Person>("Job", true);
            jobSetter(instance, "job");
            instance.GetJob().ShouldBe("job");
        }

        [Test]
        public void When_getting_properties_of_known_instance_and_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            instance.Name = "Foo";

            instance.Name.ShouldBe("Foo");

            var nameGetter = Accessor.CreateGetter<Person, string>("Name");
            nameGetter(instance);

            instance.Name.ShouldBe("Foo");

            var jobSetter = Accessor.CreateSetter<Person, string>("Job", true);
            jobSetter(instance, "Baz");

            instance.GetJob().ShouldBe("Baz");

            var jobGetter = Accessor.CreateGetter<Person, string>("Job", true);
            jobGetter(instance).ShouldBe("Baz");
        }

        [Test]
        public void When_getting_properties_of_unknown_instance_and_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            instance.Name = "Foo";

            instance.Name.ShouldBe("Foo");

            PropertyInfo nameProp;
            typeof(Person).TryGetInstanceProperty("Name", out nameProp).ShouldBeTrue();

            var nameGetter = Accessor.CreateGetter(nameProp);
            nameGetter(instance);

            instance.Name.ShouldBe("Foo");

            instance.GetJob().ShouldBeNull();

            PropertyInfo jobProp;
            typeof(Person).TryGetInstanceProperty("Job", out jobProp).ShouldBeTrue();
            var jobSetter = Accessor.CreateSetter(jobProp, true);

            jobSetter(instance, "Baz");
            instance.GetJob().ShouldBe("Baz");

            var jobGetter = Accessor.CreateGetter(jobProp, true);
            jobGetter(instance).ShouldBe("Baz");
        }

        [Test]
        public void When_getting_properties_of_known_instance_but_unknown_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            instance.Name = "Foo";

            instance.Name.ShouldBe("Foo");

            var nameGetter = Accessor.CreateGetter<Person>("Name");
            nameGetter(instance);

            instance.Name.ShouldBe("Foo");

            instance.GetJob().ShouldBeNull();

            var jobSetter = Accessor.CreateSetter<Person>("Job", true);

            jobSetter(instance, "Baz");
            instance.GetJob().ShouldBe("Baz");

            var jobGetter = Accessor.CreateGetter<Person>("Job", true);
            jobGetter(instance).ShouldBe("Baz");
        }

        private sealed class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            private string Job { get; set; }

            public string GetJob() => Job;
        }
    }

}