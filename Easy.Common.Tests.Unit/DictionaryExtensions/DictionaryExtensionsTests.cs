namespace Easy.Common.Tests.Unit.DictionaryExtensions
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class DictionaryExtensionsTests
    {
        [Test]
        public void When_getting_value_or_default()
        {
            var someDic = new Dictionary<int, string> {{1, "A"}};

            string value;
            someDic.TryGetValue(1, out value).ShouldBeTrue();
            value.ShouldBe("A");

            someDic.GetOrDefault(1).ShouldBe("A");

            someDic.GetOrDefault(4).ShouldBeNull();

            someDic.GetOrDefault(4, "not-there").ShouldBe("not-there");
            someDic.GetOrDefault(1, "not-there").ShouldBe("A");
        }

        [Test]
        public void When_converting_nameValueCollection_to_dictionary()
        {
            var someNameValueCollection = new NameValueCollection {{"1", "A"}, {"2", "B"}};

            var dictionary = someNameValueCollection.ToDictionary();

            dictionary.ShouldNotBeNull();
            dictionary.Count.ShouldBe(someNameValueCollection.Count);
            dictionary["1"].ShouldBe("A");
            dictionary["2"].ShouldBe("B");
        }

        [Test]
        public void When_converting_dictionary_to_concurrentDictionary()
        {
            var someDic = new Dictionary<int, string> { { 1, "A" }, {2, "B"} };
            var concurrentDic = someDic.ToConcurrentDictionary();

            concurrentDic.ShouldNotBeNull();
            concurrentDic.ShouldBeOfType<ConcurrentDictionary<int, string>>();
            concurrentDic.Count.ShouldBe(someDic.Count);
            concurrentDic[1].ShouldBe("A");
            concurrentDic[2].ShouldBe("B");
        }
    }
}