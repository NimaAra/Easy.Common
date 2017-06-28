namespace Easy.Common.Tests.Unit.KeyedCollectionEx
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class KeyedCollectionExTests
    {
        [Test]
        public void When_creating_a_new_keyedCollectionEx()
        {
            var personA = new Person { Name = "A", Age = 1 };
            var personB = new Person { Name = "B", Age = 2 };
            var personC = new Person { Name = "C", Age = 3 };

            var collection = new KeyedCollectionEx<string, Person>(p => p.Name)
            {
                personA,
                personB
            };

            collection.ShouldNotBeNull();
            collection.Count.ShouldBe(2);
            collection.Contains("A").ShouldBeTrue();
            collection.Contains("a").ShouldBeFalse();
            collection.Contains("B").ShouldBeTrue();
            collection.Contains("b").ShouldBeFalse();
            collection.Contains("C").ShouldBeFalse();
            collection.Contains("c").ShouldBeFalse();

            collection.Contains(personA).ShouldBeTrue();
            collection.Contains(personB).ShouldBeTrue();
            collection.Contains(personC).ShouldBeFalse();

            collection.Add(personC);
            collection.Contains("C").ShouldBeTrue();
            collection.Contains("c").ShouldBeFalse();
            collection.Contains(personC).ShouldBeTrue();

            var dodgyPerson = new Person { Name = "A", Age = 666 };
            Should.Throw<ArgumentException>(() => collection.Add(dodgyPerson)).Message
                .ShouldBe("An item with the same key has already been added.");

            collection["A"].Age.ShouldBe(1);
            collection["B"].Age.ShouldBe(2);
            collection["C"].Age.ShouldBe(3);

            Person somePerson;
            collection.TryGet("A", out somePerson).ShouldBeTrue();
            somePerson.ShouldNotBeNull();
            somePerson.Age.ShouldBe(1);

            collection.TryGet("D", out somePerson).ShouldBeFalse();
            somePerson.ShouldBeNull();
        }

        [Test]
        public void When_creating_a_new_keyedCollectionEx_with_non_default_comparer()
        {
            var personA = new Person { Name = "A", Age = 1 };
            var personB = new Person { Name = "B", Age = 2 };
            var personC = new Person { Name = "C", Age = 3 };

            var collection = new KeyedCollectionEx<string, Person>(p => p.Name, StringComparer.OrdinalIgnoreCase)
            {
                personA,
                personB
            };

            collection.ShouldNotBeNull();
            collection.Count.ShouldBe(2);
            collection.Contains("A").ShouldBeTrue();
            collection.Contains("a").ShouldBeTrue();
            collection.Contains("B").ShouldBeTrue();
            collection.Contains("b").ShouldBeTrue();
            collection.Contains("C").ShouldBeFalse();
            collection.Contains("c").ShouldBeFalse();

            collection.Contains(personA).ShouldBeTrue();
            collection.Contains(personB).ShouldBeTrue();
            collection.Contains(personC).ShouldBeFalse();

            collection.Add(personC);
            collection.Contains("C").ShouldBeTrue();
            collection.Contains("c").ShouldBeTrue();
            collection.Contains(personC).ShouldBeTrue();

            var dodgyPerson = new Person { Name = "A", Age = 666 };
            Should.Throw<ArgumentException>(() => collection.Add(dodgyPerson)).Message
                .ShouldBe("An item with the same key has already been added.");

            collection["A"].Age.ShouldBe(1);
            collection["B"].Age.ShouldBe(2);
            collection["C"].Age.ShouldBe(3);

            Person somePerson;
            collection.TryGet("A", out somePerson).ShouldBeTrue();
            somePerson.ShouldNotBeNull();
            somePerson.Age.ShouldBe(1);

            collection.TryGet("D", out somePerson).ShouldBeFalse();
            somePerson.ShouldBeNull();
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}