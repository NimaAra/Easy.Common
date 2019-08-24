// ReSharper disable PossibleMultipleEnumeration
namespace Easy.Common.Tests.Unit.EnumerablesTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class EnumerableExtensionsTests
    {
        [Test]
        public void When_getting_pages_from_a_sequence()
        {
            var collection = Enumerable.Range(1, 25).ToList();
            collection.ShouldBe(new[]
                {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25});

            const uint PageSize = 3;

            var firstPage = collection.GetPage(0, PageSize).ToArray();

            firstPage.ShouldNotBeNull();
            firstPage.ShouldNotBeEmpty();
            firstPage.Length.ShouldBe(3);
            firstPage[0].ShouldBe(1);
            firstPage[1].ShouldBe(2);
            firstPage[2].ShouldBe(3);

            var secondPage = collection.GetPage(1, PageSize).ToArray();

            secondPage.ShouldNotBeNull();
            secondPage.ShouldNotBeEmpty();
            secondPage.Length.ShouldBe(3);
            secondPage[0].ShouldBe(4);
            secondPage[1].ShouldBe(5);
            secondPage[2].ShouldBe(6);

            var emptyPage = collection.GetPage(1, 0).ToArray();

            emptyPage.ShouldNotBeNull();
            emptyPage.ShouldBeEmpty();

            var lastPage = collection.GetPage(4, 5).ToArray();

            lastPage.ShouldNotBeNull();
            lastPage.ShouldNotBeEmpty();
            lastPage.Length.ShouldBe(5);
            lastPage[0].ShouldBe(21);
            lastPage[1].ShouldBe(22);
            lastPage[2].ShouldBe(23);
            lastPage[3].ShouldBe(24);
            lastPage[4].ShouldBe(25);

            var nonExistingPage = collection.GetPage(5, 5).ToArray();

            nonExistingPage.ShouldNotBeNull();
            nonExistingPage.ShouldBeEmpty();
        }

        [Test]
        public void When_converting_an_enumerable_to_a_read_only_collection()
        {
            var intArray = new[] { 1, 2, 3, 4, 5 };
            var readOnlyIntSequence = intArray.ToReadOnlyCollection();

            readOnlyIntSequence.Count().ShouldBe(intArray.Length);
            readOnlyIntSequence.ShouldBe(intArray);

            var localCopy = readOnlyIntSequence;
            Action convertBackToOriginalArray = () =>
            {
                // ReSharper disable once UnusedVariable
                var originalIntArray = (int[])localCopy;
            };

            convertBackToOriginalArray.ShouldThrow<InvalidCastException>();

            var intList = new List<int> { 1, 2, 3, 4, 5 };
            readOnlyIntSequence = intList.ToReadOnlyCollection();

            readOnlyIntSequence.Count().ShouldBe(intList.Count);
            readOnlyIntSequence.ShouldBe(intList);

            Action convertBackToOriginalList = () =>
            {
                // ReSharper disable once UnusedVariable
                var originalIntArray = (int[])readOnlyIntSequence;
            };

            convertBackToOriginalList.ShouldThrow<InvalidCastException>();

            readOnlyIntSequence.ShouldNotContain(-1);
            intList.Add(-1);
            readOnlyIntSequence.ShouldContain(-1);
        }

        [Test]
        public void When_selecting_a_random_element_from_a_sequence()
        {
            var collection = Enumerable.Range(1, 50).ToList();
            collection.ShouldBe(new[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50
            });
            Assert.That(collection, Is.Ordered);

            var firstElement = collection.First();
            var secondElement = collection.First();

            firstElement.ShouldBe(secondElement);

            var firstRandomElement = collection.SelectRandom();
            var secondRandomElement = collection.SelectRandom();

            firstRandomElement.ShouldNotBe(secondRandomElement);

            var sequence = collection.Skip(0);
            Assert.That(sequence, Is.Ordered);

            firstRandomElement = sequence.SelectRandom();
            secondRandomElement = sequence.SelectRandom();

            firstRandomElement.ShouldNotBe(secondRandomElement);
        }

        [Test]
        public void When_randomizing_a_collection()
        {
            var collection = Enumerable.Range(1, 50).ToList();
            collection.ShouldBe(new[]
            {
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28,
                29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50
            });
            Assert.That(collection, Is.Ordered);

            var randomized = collection.Randomize().ToList();
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
                ((string)null).ToCommaSeparated();
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
                ((string)null).ToCharSeparated('|');
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
                ((string)null).ToStringSeparated("@|@");
            };

            actionOnNullString.ShouldThrow<ArgumentNullException>();
        }

        [Test]
        public void When_checking_a_collection_that_it_is_not_null_or_empty()
        {
            var sequence = Enumerable.Range(1, 5);
            sequence.IsNotNullOrEmpty().ShouldBeTrue();

            Enumerable.Empty<int>().IsNotNullOrEmpty().ShouldBeFalse();

            ((IEnumerable<int>)null).IsNotNullOrEmpty().ShouldBeFalse();
        }

        [Test]
        public void When_getting_distinct_elements_by_given_key_default_comparer()
        {
            string[] source = { "one", "two", "three", "four", "five" };
            var distinct = source.DistinctBy(word => word.Length);
            distinct.ShouldBe(new[] { "one", "three", "four" });
        }

        [Test]
        public void When_getting_distinct_elements_by_given_key_with_comparer()
        {
            string[] source = { "one", "two", "three", "four", "five" };
            var distinct = source.DistinctBy(word => word.Length, EqualityComparer<int>.Default);
            distinct.ShouldBe(new[] { "one", "three", "four" });
        }

        [Test]
        public void When_converting_sequence_to_hashset_with_default_comparer()
        {
            var sequence = new[] { "A", "B", "C", "B", "C", "C", "D" };

            sequence.Count(x => x == "A").ShouldBe(1);
            sequence.Count(x => x == "B").ShouldBe(2);
            sequence.Count(x => x == "C").ShouldBe(3);
            sequence.Count(x => x == "D").ShouldBe(1);

            var hashSet = sequence.ToHashSet();

            hashSet.Count.ShouldBe(4);

            hashSet.Count(x => x == "A").ShouldBe(1);
            hashSet.Count(x => x == "B").ShouldBe(1);
            hashSet.Count(x => x == "C").ShouldBe(1);
            hashSet.Count(x => x == "D").ShouldBe(1);

            hashSet.Contains("A").ShouldBeTrue();
            hashSet.Contains("a").ShouldBeFalse();

            hashSet.Contains("B").ShouldBeTrue();
            hashSet.Contains("b").ShouldBeFalse();

            hashSet.Contains("C").ShouldBeTrue();
            hashSet.Contains("c").ShouldBeFalse();

            hashSet.Contains("D").ShouldBeTrue();
            hashSet.Contains("d").ShouldBeFalse();

            hashSet.Contains("E").ShouldBeFalse();
            hashSet.Contains("e").ShouldBeFalse();
        }

        [Test]
        public void When_converting_sequence_to_hashset_with_non_default_comparer()
        {
            var sequence = new[] { "A", "B", "C", "B", "C", "C", "D" };

            sequence.Count(x => x == "A").ShouldBe(1);
            sequence.Count(x => x == "B").ShouldBe(2);
            sequence.Count(x => x == "C").ShouldBe(3);
            sequence.Count(x => x == "D").ShouldBe(1);

            var hashSet = sequence.ToHashSet(StringComparer.OrdinalIgnoreCase);

            hashSet.Count.ShouldBe(4);

            hashSet.Count(x => x == "A").ShouldBe(1);
            hashSet.Count(x => x == "B").ShouldBe(1);
            hashSet.Count(x => x == "C").ShouldBe(1);
            hashSet.Count(x => x == "D").ShouldBe(1);

            hashSet.Contains("A").ShouldBeTrue();
            hashSet.Contains("a").ShouldBeTrue();

            hashSet.Contains("B").ShouldBeTrue();
            hashSet.Contains("b").ShouldBeTrue();

            hashSet.Contains("C").ShouldBeTrue();
            hashSet.Contains("c").ShouldBeTrue();

            hashSet.Contains("D").ShouldBeTrue();
            hashSet.Contains("d").ShouldBeTrue();

            hashSet.Contains("E").ShouldBeFalse();
            hashSet.Contains("e").ShouldBeFalse();
        }

        [Test]
        public void When_converting_sequence_to_easy_dictionary_with_default_comparer()
        {
            var sequence = Enumerable.Range(1, 10).Select(n => new { Age = n, Name = "Name-" + n.ToString() });
            var keyedCollection = sequence.ToEasyDictionary(item => item.Name);
            keyedCollection.Count.ShouldBe(10);
            keyedCollection["Name-1"].Age.ShouldBe(1);
            keyedCollection["Name-10"].Age.ShouldBe(10);

            Should.Throw<KeyNotFoundException>(() =>
            {
                var _ = keyedCollection["name-10"];
            }).Message.ShouldBe("The given key was not present in the dictionary.");
        }

        [Test]
        public void When_converting_sequence_to_easy_dictionary_with_non_default_comparer()
        {
            var sequence = Enumerable.Range(1, 10).Select(n => new { Age = n, Name = "Name-" + n.ToString() });
            var keyedCollection = sequence.ToEasyDictionary(item => item.Name, StringComparer.OrdinalIgnoreCase);
            keyedCollection.Count.ShouldBe(10);
            keyedCollection["Name-1"].Age.ShouldBe(1);

            keyedCollection["Name-10"].Age.ShouldBe(10);
            keyedCollection["name-10"].Age.ShouldBe(10);
        }

        [Test]
        public void When_creating_batch_from_list_with_buckets_of_size_zero() =>
            Should.Throw<ArgumentOutOfRangeException>(() => new List<int>().Batch(0).ToArray())
                .Message.ShouldBe("Specified argument was out of the range of valid values.\r\nParameter name: size");

        [Test]
        public void When_creating_batch_from_empty_list()
        {
            var batches = new List<int>().Batch(2);

            batches.ShouldBeEmpty();
        }

        [Test]
        public void When_creating_batch_from_list_with_odd_number_of_items()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 6 };

            var batches = list.Batch(2).ToArray();

            batches.Length.ShouldBe(4);

            batches[0].ShouldBe(new[] { 1, 2 });
            batches[1].ShouldBe(new[] { 3, 4 });
            batches[2].ShouldBe(new[] { 5, 6 });
            batches[3].ShouldBe(new[] { 6 });
        }

        [Test]
        public void When_creating_batch_from_list_with_even_number_of_items()
        {
            var list = new List<int> { 1, 2, 3, 4, 5, 6, 6, 7 };

            var batches = list.Batch(2).ToArray();

            batches.Length.ShouldBe(4);

            batches[0].ShouldBe(new[] { 1, 2 });
            batches[1].ShouldBe(new[] { 3, 4 });
            batches[2].ShouldBe(new[] { 5, 6 });
            batches[3].ShouldBe(new[] { 6, 7 });
        }
    }
}