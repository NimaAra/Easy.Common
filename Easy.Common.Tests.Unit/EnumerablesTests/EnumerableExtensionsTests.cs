// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Tests.Unit.EnumerablesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class EnumerableExtensionsTests
    {
        [Test]
        public void When_converting_an_enumerable_to_a_read_only_collection()
        {
            var intArray = new[] {1, 2, 3, 4, 5};
            var readOnlyIntSequence = intArray.ToReadOnlyCollection();

            readOnlyIntSequence.Count().ShouldBe(intArray.Length);
            readOnlyIntSequence.ShouldBe(intArray);

            var localCopy = readOnlyIntSequence;
            Action convertBackToOriginalArray = () =>
            {
                // ReSharper disable once UnusedVariable
                var originalIntArray = (int[]) localCopy;
            };

            convertBackToOriginalArray.ShouldThrow<InvalidCastException>();

            var intList = new List<int> {1, 2, 3, 4, 5};
            readOnlyIntSequence = intList.ToReadOnlyCollection();

            readOnlyIntSequence.Count().ShouldBe(intList.Count);
            readOnlyIntSequence.ShouldBe(intList);

            Action convertBackToOriginalList = () =>
            {
                // ReSharper disable once UnusedVariable
                var originalIntArray = (int[]) readOnlyIntSequence;
            };

            convertBackToOriginalList.ShouldThrow<InvalidCastException>();

            readOnlyIntSequence.ShouldNotContain(-1);
            intList.Add(-1);
            readOnlyIntSequence.ShouldContain(-1);
        }

        [Test]
        public void When_randomizing_a_collection()
        {
            var sequence = Enumerable.Range(1, 25).ToList();
            sequence.ShouldBe(new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25});
            Assert.That(sequence, Is.Ordered);
            
            var randomized = sequence.Randomize().ToList();
            Assert.That(randomized, Is.Not.Ordered);
        }

        [Test]
        public void When_getting_a_collection_as_comma_separated_values()
        {
            var sequence = Enumerable.Range(1, 5);
            sequence.ToCommaSeparated().ShouldBe("1,2,3,4,5");

            sequence = Enumerable.Empty<int>();
            sequence.ToCommaSeparated().ShouldBe(string.Empty);

            Action actionOnNullString = () =>
            {
                ((string) null).ToCommaSeparated();
            };

            actionOnNullString.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void When_getting_a_collection_as_char_separated_values()
        {
            var sequence = Enumerable.Range(1, 5);
            sequence.ToCharSeparated('|').ShouldBe("1|2|3|4|5");

            sequence = Enumerable.Empty<int>();
            sequence.ToCharSeparated('|').ShouldBe(string.Empty);

            Action actionOnNullString = () =>
            {
                ((string) null).ToCharSeparated('|');
            };

            actionOnNullString.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void When_getting_a_collection_as_string_separated_values()
        {
            var sequence = Enumerable.Range(1, 5);
            sequence.ToStringSeparated("@|@").ShouldBe("1@|@2@|@3@|@4@|@5");

            sequence = Enumerable.Empty<int>();
            sequence.ToStringSeparated("@|@").ShouldBe(string.Empty);

            Action actionOnNullString = () =>
            {
                ((string) null).ToStringSeparated("@|@");
            };

            actionOnNullString.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void When_checking_a_collection_that_it_is_not_null_or_empty()
        {
            var sequence = Enumerable.Range(1, 5);
            sequence.IsNotNullOrEmpty().ShouldBeTrue();

            Enumerable.Empty<int>().IsNotNullOrEmpty().ShouldBeFalse();

            ((IEnumerable<int>) null).IsNotNullOrEmpty().ShouldBeFalse();
        }
    }
}