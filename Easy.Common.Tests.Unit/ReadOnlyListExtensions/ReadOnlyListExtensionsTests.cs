namespace Easy.Common.Tests.Unit.ReadOnlyListExtensions
{
    using System.Collections.Generic;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class ReadOnlyListExtensionsTests
    {
        [Test]
        public void When_searching_for_an_int()
        {
            IReadOnlyList<int> items = new[] {1, 2, 3};
            items.IndexOf(1).ShouldBe(0);
            items.IndexOf(3).ShouldBe(2);

            items.IndexOf(42).ShouldBe(-1);
        }
        
        [Test]
        public void When_searching_for_a_string()
        {
            IReadOnlyList<string> items = new[] {"A", "B"};
            items.IndexOf("A").ShouldBe(0);
            items.IndexOf("B").ShouldBe(1);

            items.IndexOf("b").ShouldBe(-1);
        }

        [Test]
        public void When_searching_for_an_object()
        {
            var jo = new Person { Name = "Jo", Age = 42 };
            var bob = new Person { Name = "Bob", Age = 16 };

            IReadOnlyList<Person> items = new[] { jo, bob };

            items.IndexOf(jo).ShouldBe(0);
            items.IndexOf(bob).ShouldBe(1);

            items.IndexOf(new Person { Name = "Jil", Age = 81 }).ShouldBe(-1);
        }

        private sealed class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}