namespace Easy.Common.Tests.Unit.KeyedCollectionEx
{
    using System;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class KeyedCollectionExTests
    {
        [Test]
        public void When_checking_keys_of_an_empty_collection()
        {
            var collection = new KeyedCollectionEx<string, Person>(p => p.Name);

            collection.ShouldBeEmpty();
            collection.Count.ShouldBe(0);

            collection.Keys.ShouldNotBeNull();
            collection.Keys.Count.ShouldBe(0);
        }
        
        [Test]
        public void When_checking_keys()
        {
            var personA = new Person { Name = "A", Age = 1 };
            var personB = new Person { Name = "B", Age = 2 };
            var personC = new Person { Name = "C", Age = 3 };

            var collection = new KeyedCollectionEx<string, Person>(p => p.Name)
            {
                personA,
                personB,
                personC
            };

            collection.ShouldNotBeNull();
            collection.Count.ShouldBe(3);
            
            collection.Keys.ShouldNotBeNull();
            collection.Keys.ShouldNotBeEmpty();
            collection.Keys.Count.ShouldBe(3);
            collection.Keys.ShouldBe(new[] {"A", "B", "C"});

            collection.RemoveAt(1);

            collection.Count.ShouldBe(2);

            collection.Keys.Count.ShouldBe(2);
            collection.Keys.ShouldBe(new[] { "A", "C" });
        }

        [Test]
        public void When_checking_values_of_an_empty_collection()
        {
            var collection = new KeyedCollectionEx<string, Person>(p => p.Name);

            collection.ShouldBeEmpty();
            collection.Count.ShouldBe(0);

            collection.Values.ShouldNotBeNull();
            collection.Values.Count.ShouldBe(0);
        }

        [Test]
        public void When_checking_values()
        {
            var personA = new Person { Name = "A", Age = 1 };
            var personB = new Person { Name = "B", Age = 2 };
            var personC = new Person { Name = "C", Age = 3 };

            var collection = new KeyedCollectionEx<string, Person>(p => p.Name)
            {
                personA,
                personB,
                personC
            };

            collection.ShouldNotBeNull();
            collection.Count.ShouldBe(3);

            collection.Values.ShouldNotBeNull();
            collection.Values.ShouldNotBeEmpty();
            collection.Values.Count.ShouldBe(3);
            collection.Values.ShouldBe(new[] { personA, personB, personC });

            collection.RemoveAt(1);

            collection.Count.ShouldBe(2);

            collection.Values.Count.ShouldBe(2);
            collection.Values.ShouldBe(new[] { personA, personC });
        }

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

        [Test]
        public void When_getting_or_adding_value()
        {
            var oldValue = new Person { Name = "Joe", Age = 1 };

            var someKeyedCollection = new KeyedCollectionEx<string, Person>(p => p.Name) { oldValue };

            var newValue = new Person { Name = "Jim", Age = 1 };

            oldValue.ShouldNotBe(newValue);

            someKeyedCollection.Remove(newValue);

            someKeyedCollection.GetOrAdd("Joe", newValue).ShouldBe(oldValue);
            someKeyedCollection.GetOrAdd("Jim", newValue).ShouldBe(newValue);
        }

        [Test]
        public void When_getting_or_adding_value_using_value_creator()
        {
            var oldValue = new Person { Name = "Joe", Age = 1 };

            var someKeyedCollection = new KeyedCollectionEx<string, Person>(p => p.Name) { oldValue };

            var newValue = new Person { Name = "Jim", Age = 1 };

            oldValue.ShouldNotBe(newValue);

            someKeyedCollection.GetOrAdd("Joe", () => newValue).ShouldBe(oldValue);
            someKeyedCollection.GetOrAdd("Jim", () => newValue).ShouldBe(newValue);
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}