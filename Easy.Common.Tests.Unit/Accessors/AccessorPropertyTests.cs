namespace Easy.Common.Tests.Unit.Accessors
{
    using System;
    using System.Reflection;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class AccessorPropertyTests
    {
        [Test]
        public void When_checking_invalid_input()
        {
            string nullStr = null;
            PropertyInfo nullPropInfo = null;

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildSetter<Person, string>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildSetter<Person, string>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildGetter<Person, string>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildGetter<Person, string>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildSetter<Person>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildSetter<Person>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildGetter<Person>(nullStr))
                .Message.ShouldBe("String must not be null, empty or whitespace.");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildGetter<Person>(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildSetter(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");

            Should.Throw<ArgumentException>(() => AccessorBuilder.BuildGetter(nullPropInfo))
                .Message.ShouldBe("Value cannot be null. (Parameter 'propertyInfo')");
        }

        [Test]
        public void When_getting_getters_class()
        {
            var nameGetterOne = AccessorBuilder.BuildGetter<Person, string>("Name");
            var nameGetterTwo = AccessorBuilder.BuildGetter<Person, string>("Name");

            nameGetterOne.ShouldNotBeSameAs(nameGetterTwo);

            var jobGetter = AccessorBuilder.BuildGetter<Person, string>("Job", true);

            jobGetter.ShouldNotBe(nameGetterOne);
            jobGetter.ShouldNotBe(nameGetterTwo);
        }

        [Test]
        public void When_getting_setters_class()
        {
            var nameSetterOne = AccessorBuilder.BuildSetter<Person, string>("Name");
            var nameSetterTwo = AccessorBuilder.BuildSetter<Person, string>("Name");

            nameSetterOne.ShouldNotBeSameAs(nameSetterTwo);

            var jobSetter = AccessorBuilder.BuildSetter<Person, string>("Job", true);

            jobSetter.ShouldNotBe(nameSetterOne);
            jobSetter.ShouldNotBe(nameSetterTwo);
        }

        [Test]
        public void When_setting_properties_of_known_instance_and_property_type_class()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            var nameSetter = AccessorBuilder.BuildSetter<Person, string>("Name");
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            var jobSetter = AccessorBuilder.BuildSetter<Person, string>("Job", true);
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

            var nameSetter = AccessorBuilder.BuildSetter(nameProp);
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            PropertyInfo jobProp;
            typeof(Person).TryGetInstanceProperty("Job", out jobProp).ShouldBeTrue();
            var jobSetter = AccessorBuilder.BuildSetter(jobProp, true);
            jobSetter(instance, "job");
            instance.GetJob().ShouldBe("job");
        }

        [Test]
        public void When_setting_properties_of_known_instance_but_unknown_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            var nameSetter = AccessorBuilder.BuildSetter<Person>("Name");
            nameSetter(instance, "A");
            instance.Name.ShouldBe("A");

            var jobSetter = AccessorBuilder.BuildSetter<Person>("Job", true);
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

            var nameGetter = AccessorBuilder.BuildGetter<Person, string>("Name");
            nameGetter(instance);

            instance.Name.ShouldBe("Foo");

            var jobSetter = AccessorBuilder.BuildSetter<Person, string>("Job", true);
            jobSetter(instance, "Baz");

            instance.GetJob().ShouldBe("Baz");

            var jobGetter = AccessorBuilder.BuildGetter<Person, string>("Job", true);
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

            var nameGetter = AccessorBuilder.BuildGetter(nameProp);
            nameGetter(instance).ShouldBe("Foo");

            instance.Name.ShouldBe("Foo");

            instance.GetJob().ShouldBeNull();

            PropertyInfo jobProp;
            typeof(Person).TryGetInstanceProperty("Job", out jobProp).ShouldBeTrue();
            var jobSetter = AccessorBuilder.BuildSetter(jobProp, true);

            jobSetter(instance, "Baz");
            instance.GetJob().ShouldBe("Baz");

            var jobGetter = AccessorBuilder.BuildGetter(jobProp, true);
            jobGetter(instance).ShouldBe("Baz");
        }

        [Test]
        public void When_getting_properties_of_known_instance_but_unknown_property_type()
        {
            var instance = new Person();
            instance.Name.ShouldBeNull();

            instance.Name = "Foo";

            instance.Name.ShouldBe("Foo");

            var nameGetter = AccessorBuilder.BuildGetter<Person>("Name");
            nameGetter(instance).ShouldBe("Foo");

            instance.Name.ShouldBe("Foo");

            instance.GetJob().ShouldBeNull();

            var jobSetter = AccessorBuilder.BuildSetter<Person>("Job", true);

            jobSetter(instance, "Baz");
            instance.GetJob().ShouldBe("Baz");

            var jobGetter = AccessorBuilder.BuildGetter<Person>("Job", true);
            jobGetter(instance).ShouldBe("Baz");
        }

        [Test]
        public void When_getting_properties_of_unknown_instance_and_property_type_of_a_struct()
        {
            var instance = new Struct();
            instance.SomeString.ShouldBeNull();

            instance.SomeString = "Foo";

            instance.SomeString.ShouldBe("Foo");

            PropertyInfo nameProp;
            typeof(Struct).TryGetInstanceProperty("SomeString", out nameProp).ShouldBeTrue();

            var nameGetter = AccessorBuilder.BuildGetter(nameProp);
            nameGetter(instance).ShouldBe("Foo");
            instance.SomeString.ShouldBe("Foo");
        }

        [Test]
        public void When_getting_properties_of_known_instance_but_unkonnown_property_type_of_a_struct()
        {
            var instance = new Struct();
            instance.SomeString.ShouldBeNull();

            instance.SomeString = "Foo";

            instance.SomeString.ShouldBe("Foo");

            var propGetter = AccessorBuilder.BuildGetter<Struct>("SomeString");
            propGetter(instance).ShouldBe("Foo");
            instance.SomeString.ShouldBe("Foo");
        }

        private sealed class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
            private string Job { get; set; }

            public string GetJob() => Job;
        }

        private struct Struct
        {
            public string SomeString { get; set; }
        }
    }
}