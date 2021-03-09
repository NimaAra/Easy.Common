namespace Easy.Common.Tests.Unit.Accessors
{
    using System.Reflection;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class AccessorCreateInstanceTests
    {
        [Test]
        public void When_creating_an_instance_of_object_with_default_constructor()
        {
            var publicCtor = typeof(Zero).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilderOne = AccessorBuilder.BuildInstanceCreator<Zero>(publicCtor[0]);
            var instanceOne = instanceBuilderOne(new object[] { "Zero" });

            instanceOne.ShouldNotBeNull();
            instanceOne.Name.ShouldBe("Zero");

            var instanceBuilderTwo = AccessorBuilder.BuildInstanceCreator<Zero>();
            var instanceTwo = instanceBuilderTwo();

            instanceTwo.ShouldNotBeNull();
            instanceTwo.Name.ShouldBe("Zero");
        }

        [Test]
        public void When_creating_an_instance_of_object_with_one_parameter_public_constructor()
        {
            var publicCtor = typeof(One).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(2);

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<One>(publicCtor[0]);
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

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<One>(publicCtor[1]);
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

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<Two>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Two" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("Two");
        }

        [Test]
        public void When_creating_an_instance_of_object_with_one_parameter_internal_constructor()
        {
            var publicCtor = typeof(Three).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<Three>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Two" });

            instance.ShouldNotBeNull();
            instance.Name.ShouldBe("Two");
        }

        [Test]
        public void When_creating_an_instance_of_a_child_object_with_two_parameters_constructor()
        {
            var publicCtor = typeof(Four).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<Four>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Child", "Base" });

            instance.ShouldNotBeNull();
            instance.ShouldBeOfType<Four>();
            instance.ShouldBeAssignableTo<Three>();
            instance.NameChild.ShouldBe("Child");
            instance.Name.ShouldBe("Base");
        }

        [Test]
        public void When_creating_an_instance_of_a_struct_with_constructor()
        {
            var publicCtor = typeof(ValueTypeWithCtor).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            publicCtor.Length.ShouldBe(1);

            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<ValueTypeWithCtor>(publicCtor[0]);
            var instance = instanceBuilder(new object[] { "Child", 10 });

            instance.ShouldBeOfType<ValueTypeWithCtor>();
            instance.ShouldBeAssignableTo<ValueTypeWithCtor>();
            instance.Name.ShouldBe("Child");
            instance.Age.ShouldBe(10);
            instance.GetJob().ShouldBe("SomeJob");
        }

        [Test]
        public void When_creating_an_instance_of_a_struct_with_default_constructor()
        {
            var instanceBuilder = AccessorBuilder.BuildInstanceCreator<ValueTypeDefaultCtor>();
            var instance = instanceBuilder();

            instance.ShouldBeOfType<ValueTypeDefaultCtor>();
            instance.ShouldBeAssignableTo<ValueTypeDefaultCtor>();
            instance.Name.ShouldBe("Foo");
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

        private struct ValueTypeWithCtor
        {
            public ValueTypeWithCtor(string name, int age)
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

        private struct ValueTypeDefaultCtor
        {
            public string Name => "Foo";
        }
    }
}