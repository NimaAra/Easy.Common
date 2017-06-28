namespace Easy.Common.Tests.Unit.KeyedCollectionExtensions
{
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class KeyedCollectionExtensionsTests
    {
        [Test]
        public void When_getting_or_adding_value()
        {
            var oldValue = new Person { Name = "Joe", Age = 1 };

            var someKeyedCollection = new KeyedCollectionEx<string, Person>(p => p.Name) { oldValue };

            var newValue = new Person { Name = "Jim", Age = 1 };

            oldValue.ShouldNotBe(newValue);

            someKeyedCollection.GetOrAdd("Joe", () => newValue).ShouldBe(oldValue);
            someKeyedCollection.GetOrAdd("Jim", () => newValue).ShouldBe(newValue);

            someKeyedCollection.Remove(newValue);

            someKeyedCollection.GetOrAdd("Joe", newValue).ShouldBe(oldValue);
            someKeyedCollection.GetOrAdd("Jim", newValue).ShouldBe(newValue);
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}