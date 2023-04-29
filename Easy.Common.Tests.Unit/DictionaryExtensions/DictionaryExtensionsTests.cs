namespace Easy.Common.Tests.Unit.DictionaryExtensions;

using Easy.Common.Extensions;
using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using HashHelper = Easy.Common.HashHelper;

[TestFixture]
internal sealed class DictionaryExtensionsTests
{
    [Test]
    public void When_getting_or_adding_value()
    {
        Stopwatch oldValue = new();

        Dictionary<int, Stopwatch> someDic = new() { { 1, oldValue } };

        Stopwatch newValue = new();

        oldValue.ShouldNotBe(newValue);

        someDic.GetOrAdd(1, () => newValue).ShouldBe(oldValue);
        someDic.GetOrAdd(2, () => newValue).ShouldBe(newValue);

        someDic.Remove(2);

        someDic.GetOrAdd(1, newValue).ShouldBe(oldValue);
        someDic.GetOrAdd(2, newValue).ShouldBe(newValue);
    }

    [Test]
    public void When_getting_value_or_default()
    {
        Dictionary<int, string> someDic = new() { { 1, "A" } };

        someDic.TryGetValue(1, out string? value).ShouldBeTrue();
        value.ShouldBe("A");

        someDic.GetOrDefault(1).ShouldBe("A");

        someDic.GetOrDefault(4).ShouldBeNull();

        someDic.GetOrDefault(4, "not-there").ShouldBe("not-there");
        someDic.GetOrDefault(1, "not-there").ShouldBe("A");
    }

    [Test]
    public void When_converting_nameValueCollection_to_dictionary()
    {
        NameValueCollection someNameValueCollection = new() { { "1", "A" }, { "2", "B" } };

        Dictionary<string, string> dictionary = someNameValueCollection.ToDictionary();

        dictionary.ShouldNotBeNull();
        dictionary.Count.ShouldBe(someNameValueCollection.Count);
        dictionary["1"].ShouldBe("A");
        dictionary["2"].ShouldBe("B");
    }

    [Test]
    public void When_converting_dictionary_to_concurrentDictionary()
    {
        Dictionary<string, int> someDic = new() { { "A", 1 }, { "B", 2 } };
        ConcurrentDictionary<string, int> concurrentDic = someDic.ToConcurrentDictionary();

        concurrentDic.ShouldNotBeNull();
        concurrentDic.ShouldBeOfType<ConcurrentDictionary<string, int>>();
        concurrentDic.Count.ShouldBe(someDic.Count);
        concurrentDic["A"].ShouldBe(1);
        concurrentDic["B"].ShouldBe(2);

        concurrentDic.ContainsKey("A").ShouldBeTrue();
        concurrentDic.ContainsKey("a").ShouldBeFalse();
    }

    [Test]
    public void When_converting_dictionary_to_concurrentDictionary_with_comparer()
    {
        Dictionary<string, int> someDic = new() { { "A", 1 }, { "B", 2 } };
        ConcurrentDictionary<string, int> concurrentDic = someDic.ToConcurrentDictionary(StringComparer.OrdinalIgnoreCase);

        concurrentDic.ShouldNotBeNull();
        concurrentDic.ShouldBeOfType<ConcurrentDictionary<string, int>>();
        concurrentDic.Count.ShouldBe(someDic.Count);
        concurrentDic["A"].ShouldBe(1);
        concurrentDic["B"].ShouldBe(2);

        concurrentDic.ContainsKey("A").ShouldBeTrue();
        concurrentDic.ContainsKey("a").ShouldBeTrue();
    }

    [Test]
    public void When_comparing_dictionaries()
    {
        IDictionary<string, int> left = new Dictionary<string, int>();
        IDictionary<string, int> right = new Dictionary<string, int>();

        left.EqualsAnother(right).ShouldBeTrue();

        left.Add("A", 1);
        
        left.EqualsAnother(right).ShouldBeFalse();

        right.Add("A", 1);
        left.EqualsAnother(right).ShouldBeTrue();

        left["A"] = 2;
        left.EqualsAnother(right).ShouldBeFalse();

        right["A"] = 2;
        right["B"] = 3;
        left.EqualsAnother(right).ShouldBeFalse();

        left["B"] = 3;
        left.EqualsAnother(right).ShouldBeTrue();
    }

    [Test]
    public void When_comparing_dictionaries_with_comparer()
    {
        IDictionary<int, string> left = new Dictionary<int, string>();
        IDictionary<int, string> right = new Dictionary<int, string>();

        var comparer = StringComparer.OrdinalIgnoreCase;

        left.EqualsAnother(right, comparer).ShouldBeTrue();

        left.Add(1, "A");
        left.EqualsAnother(right, comparer).ShouldBeFalse();

        right.Add(1, "A");
        left.EqualsAnother(right, comparer).ShouldBeTrue();

        left[2] = "A";
        left.EqualsAnother(right, comparer).ShouldBeFalse();

        right[2] = "A";
        right[3] = "B";
        left.EqualsAnother(right, comparer).ShouldBeFalse();

        left[3] = "B";
        left.EqualsAnother(right, comparer).ShouldBeTrue();

        left[3] = "b";
        left.EqualsAnother(right, comparer).ShouldBeTrue();
    }

    [Test]
    public void When_comparing_easy_dictionaries()
    {
        IReadOnlyDictionary<string, Person> left = new EasyDictionary<string, Person>(p => p.Id);
        IReadOnlyDictionary<string, Person> right = new EasyDictionary<string, Person>(p => p.Id);

        left.EqualsAnother(right).ShouldBeTrue();

        ((IEasyDictionary<string, Person>)left).Add(new Person("A", 1));
        left.EqualsAnother(right).ShouldBeFalse();

        ((IEasyDictionary<string, Person>)right).Add(new Person("A", 1));
        left.EqualsAnother(right).ShouldBeTrue();
    }

    [Test]
    public void When_initializing_a_dictionary_with_multiple_items()
    {
        Dictionary<int, string> itemsToAdd = new()
        {
            [2] = "B",
            [3] = "C",
            [4] = "D",
        };

        Dictionary<int, string> dic = new()
        {
            {1, "A"},
            itemsToAdd, 
            {5, "E"}
        };

        dic.Count.ShouldBe(5);
        dic[1].ShouldBe("A");
        dic[2].ShouldBe("B");
        dic[3].ShouldBe("C");
        dic[4].ShouldBe("D");
        dic[5].ShouldBe("E");
    }

    private sealed class Person : Equatable<Person>
    {
        public Person(string id, int age)
        {
            Id = id;
            Age = age;
        }

        public string Id { get; }
        public int Age { get; }

        public override int GetHashCode() => HashHelper.GetHashCode(Id, Age);
    }
}