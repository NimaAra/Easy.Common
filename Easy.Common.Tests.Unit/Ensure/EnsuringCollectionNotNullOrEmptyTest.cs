namespace Easy.Common.Tests.Unit.Ensure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Easy.Common;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class EnsuringCollectionNotNullOrEmptyTest
    {
        [Test]
        public void When_checking_a_null_string_collection()
        {
            List<string> collection = null;

            Action action = () => Ensure.NotNullOrEmpty(collection);

            action.ShouldThrow<ArgumentNullException>("Because collection is null.")
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: collection");
        }

        [Test]
        public void When_checking_an_empty_string_collection()
        {
            var collection = Enumerable.Empty<string>().ToList();

            Action action = () => Ensure.NotNullOrEmpty(collection);

            action.ShouldThrow<ArgumentException>("Because collection is empty.")
                .Message.ShouldBe("Collection must not be null or empty.");
        }

        [Test]
        public void When_checking_a_non_empty_collection()
        {
            var collection = new List<string> { "Item One" };

            ICollection<string> returnedValue = new Collection<string>();
            Action action = () => returnedValue = Ensure.NotNullOrEmpty(collection);
            
            action.ShouldNotThrow("Because collection is not empty.");
            returnedValue.ShouldBeSameAs(collection);
        }
    }
}