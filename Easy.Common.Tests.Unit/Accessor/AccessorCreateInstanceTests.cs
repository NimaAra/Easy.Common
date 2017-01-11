namespace Easy.Common.Tests.Unit.Accessor
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using NUnit.Framework;
    using Shouldly;
    using Accessor = Easy.Common.Accessor;

    [TestFixture]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal sealed class AccessorCreateInstanceTests
    {
        [Test]
        public void When_creating_an_instance_of_object_with_default_constructor()
        {
            var publicCtor = typeof(Zero).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = Accessor.CreateInstanceBuilder<Zero>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Zero" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("Zero");
        }

        [Test]
        public void When_creating_an_instance_of_object_with_one_parameter_public_constructor()
        {
            var publicCtor = typeof(One).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(2);

            var instanceBuilder = Accessor.CreateInstanceBuilder<One>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "One" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("One");
            instance.Age.ShouldBe(0);
        }

        [Test]
        public void When_creating_an_instance_of_object_with_multiple_parameters_public_constructor()
        {
            var publicCtor = typeof(One).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(2);

            var instanceBuilder = Accessor.CreateInstanceBuilder<One>(publicCtor[1]);
            var instance = instanceBuilder(new object[] { "One", 1 });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("One");
            instance.Age.ShouldBe(1);
        }

        [Test]
        public void When_creating_an_instance_of_object_with_one_parameter_private_constructor()
        {
            var publicCtor = typeof(Two).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = Accessor.CreateInstanceBuilder<Two>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Two" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("Two");
        }

        [Test]
        public void When_creating_an_instance_of_object_with_one_parameter_internal_constructor()
        {
            var publicCtor = typeof(Three).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = Accessor.CreateInstanceBuilder<Three>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Two" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("Two");
        }

        [Test]
        public void When_creating_an_instance_of_a_child_object_with_two_parameters_constructor()
        {
            var publicCtor = typeof(Four).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = Accessor.CreateInstanceBuilder<Four>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Child", "Base" });

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<Four>();
            instance.ShouldBeAssignableTo<Three>();
            instance.NameChild.ShouldBe("Child");
            instance.Name.ShouldBe("Base");
        }

        [Test]
        public void When_creating_an_instance_of_a_struct()
        {
            var publicCtor = typeof(ValueType).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = Accessor.CreateInstanceBuilder<ValueType>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Child", 10 });

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<ValueType>();
            instance.ShouldBeAssignableTo<ValueType>();
            instance.Name.ShouldBe("Child");
            instance.Age.ShouldBe(10);
            instance.GetJob().ShouldBe("SomeJob");
        }

        private sealed class Zero
        {
            public string Name => "Zero";
        }

        private sealed class One
        {
            public string Name { get; }
            public int Age { get; }

            public One(string name)
            {
                Name = name;
            }

            public One(string name, int age)
            {
                Name = name;
                Age = age;
            }
        }

        private sealed class Two
        {
            public string Name { get; }
            private Two(string name)
            {
                Name = name;
            }
        }

        private class Three
        {
            public string Name { get; }
            internal Three(string name)
            {
                Name = name;
            }
        }

        private sealed class Four : Three
        {
            public string NameChild { get; }
            internal Four(string childName, string baseName) : base(baseName)
            {
                NameChild = childName;
            }
        }

        private struct ValueType
        {
            public ValueType(string name, int age)
            {
                Name = name;
                Age = age;
                Job = "SomeJob";
            }

            public string Name { get; set; }
            public int Age { get; set; }
            private string Job { get; set; }

            public string GetJob() => Job;
        }
    }
}