namespace Easy.Common.Tests.Unit.EasyDictionary
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Shouldly;
    using HashHelper = Easy.Common.HashHelper;

    [TestFixture]
    internal sealed class EasyDictionaryTests
    {
        [Test]
        public void When_creating_easy_dictionary_with_key_selector()
        {
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(0);
            dic.Keys.ShouldBeEmpty();
            dic.Values.ShouldBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldBe(EqualityComparer<string>.Default);
        }        

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_and_capacity()
        {
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector, 10);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(0);
            dic.Keys.ShouldBeEmpty();
            dic.Values.ShouldBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldBe(EqualityComparer<string>.Default);
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_capacity_and_comparer()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, 10, comparer);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(0);
            dic.Keys.ShouldBeEmpty();
            dic.Values.ShouldBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldNotBe(EqualityComparer<string>.Default);
            dic.Comparer.ShouldBe(comparer);
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_and_dictionary()
        {
            var somePerson = new Person("A", 1);
            var someDic = new Dictionary<string, Person> {["B"] = somePerson};
            
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector, someDic);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(1);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldBe(EqualityComparer<string>.Default);
            
            dic.Keys.ShouldBe(new [] {"A"});
            dic.Values.ShouldBe(new[] { somePerson });
            
            dic.ContainsKey("A").ShouldBeTrue();
            dic.Contains(somePerson).ShouldBeTrue();

            dic.ContainsKey("B").ShouldBeFalse();
            
            dic["A"].Age.ShouldBe(1);
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_comparer_and_dictionary()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            var somePerson = new Person("A", 1);
            var someDic = new Dictionary<string, Person> { ["B"] = somePerson };

            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, someDic, comparer);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(1);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldNotBe(EqualityComparer<string>.Default);
            dic.Comparer.ShouldBe(comparer);

            dic.Keys.ShouldBe(new[] { "A" });
            dic.Values.ShouldBe(new[] { somePerson });

            dic.ContainsKey("A").ShouldBeTrue();
            dic.Contains(somePerson).ShouldBeTrue();

            dic.ContainsKey("B").ShouldBeFalse();

            dic["A"].Age.ShouldBe(1);
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_and_sequence()
        {
            var seq = Enumerable.Range(1, 5).Select(n => new Person(n.ToString(), n));
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector, seq);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(5);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldBe(EqualityComparer<string>.Default);

            dic.Keys.ShouldBe(new[] { "1", "2", "3", "4", "5" });
            dic.Values.ShouldBe(seq);

            dic.ContainsKey("1").ShouldBeTrue();
            dic.Contains(seq.First()).ShouldBeTrue();

            dic["1"].Age.ShouldBe(1);

            dic.ContainsKey("0").ShouldBeFalse();
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_comparer_and_sequence()
        {
            var seq = Enumerable.Range(1, 5).Select(n => new Person(n.ToString(), n));
            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, seq, comparer);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(5);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldNotBe(EqualityComparer<string>.Default);
            dic.Comparer.ShouldBe(comparer);

            dic.Keys.ShouldBe(new[] { "1", "2", "3", "4", "5" });
            dic.Values.ShouldBe(seq);

            dic.ContainsKey("1").ShouldBeTrue();
            dic.Contains(seq.First()).ShouldBeTrue();

            dic["1"].Age.ShouldBe(1);

            dic.ContainsKey("0").ShouldBeFalse();
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_and_collection()
        {
            var collection = Enumerable.Range(1, 5).Select(n => new Person(n.ToString(), n)).ToList();
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector, collection);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(5);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldBe(EqualityComparer<string>.Default);

            dic.Keys.ShouldBe(new[] { "1", "2", "3", "4", "5" });
            dic.Values.ShouldBe(collection);

            dic.ContainsKey("1").ShouldBeTrue();
            dic.Contains(collection[0]).ShouldBeTrue();

            dic["1"].Age.ShouldBe(1);

            dic.ContainsKey("0").ShouldBeFalse();
        }

        [Test]
        public void When_creating_easy_dictionary_with_key_selector_comparer_and_collection()
        {
            var collection = Enumerable.Range(1, 5).Select(n => new Person(n.ToString(), n)).ToList();
            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, collection, comparer);

            dic.ShouldNotBeNull();
            dic.KeySelector.ShouldBe(selector);
            dic.Count.ShouldBe(5);
            dic.Keys.ShouldNotBeEmpty();
            dic.Values.ShouldNotBeEmpty();
            dic.IsReadOnly.ShouldBeFalse();
            dic.Comparer.ShouldNotBe(EqualityComparer<string>.Default);
            dic.Comparer.ShouldBe(comparer);

            dic.Keys.ShouldBe(new[] { "1", "2", "3", "4", "5" });
            dic.Values.ShouldBe(collection);

            dic.ContainsKey("1").ShouldBeTrue();
            dic.Contains(collection[0]).ShouldBeTrue();

            dic["1"].Age.ShouldBe(1);

            dic.ContainsKey("0").ShouldBeFalse();
        }

        [Test]
        public void When_adding_and_removing_items()
        {
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(selector);

            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.Add(p1);
            dic.Count.ShouldBe(1);

            dic.Keys.Count.ShouldBe(1);
            dic.Values.Count.ShouldBe(1);

            dic.Contains(p1).ShouldBeTrue();
            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.TryGetValue("A", out var pA).ShouldBeTrue();
            pA.ShouldBe(pA);

            dic["A"].ShouldBe(pA);
            Should.Throw<KeyNotFoundException>(() => dic["a"].ToString());

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.TryGetValue("B", out var pB).ShouldBeFalse();
            
            dic.Add(p2);
            dic.Count.ShouldBe(2);

            dic.Keys.Count.ShouldBe(2);
            dic.Values.Count.ShouldBe(2);

            dic.Contains(p1).ShouldBeTrue();
            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeTrue();
            dic.ContainsKey("B").ShouldBeTrue();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.TryGetValue("B", out pB).ShouldBeTrue();
            pB.ShouldBe(p2);

            dic["B"].ShouldBe(pB);
            Should.Throw<KeyNotFoundException>(() => dic["b"].ToString());

            dic.Remove("C").ShouldBeFalse();
            dic.Remove("c").ShouldBeFalse();
            dic.Remove(new Person("C", 3)).ShouldBeFalse();

            dic.Keys.Count.ShouldBe(2);
            dic.Values.Count.ShouldBe(2);

            dic.Remove("a").ShouldBeFalse();
            dic.Remove("A").ShouldBeTrue();

            dic.Keys.Count.ShouldBe(1);
            dic.Values.Count.ShouldBe(1);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeTrue();
            dic.ContainsKey("B").ShouldBeTrue();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.Remove(p2).ShouldBeTrue();

            dic.Keys.Count.ShouldBe(0);
            dic.Values.Count.ShouldBe(0);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();
        }

        [Test]
        public void When_adding_and_removing_items_with_comparer()
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, comparer: comparer);

            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.Add(p1);
            dic.Count.ShouldBe(1);

            dic.Keys.Count.ShouldBe(1);
            dic.Values.Count.ShouldBe(1);

            dic.Contains(p1).ShouldBeTrue();
            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("a").ShouldBeTrue();

            dic.TryGetValue("A", out var pA).ShouldBeTrue();
            pA.ShouldBe(pA);

            dic["A"].ShouldBe(pA);
            dic["a"].ShouldBe(pA);

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();

            dic.TryGetValue("B", out var pB).ShouldBeFalse();

            dic.Add(p2);
            dic.Count.ShouldBe(2);

            dic.Keys.Count.ShouldBe(2);
            dic.Values.Count.ShouldBe(2);

            dic.Contains(p1).ShouldBeTrue();
            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("a").ShouldBeTrue();

            dic.Contains(p2).ShouldBeTrue();
            dic.ContainsKey("B").ShouldBeTrue();
            dic.ContainsKey("b").ShouldBeTrue();

            dic.TryGetValue("B", out pB).ShouldBeTrue();
            pB.ShouldBe(p2);

            dic["B"].ShouldBe(pB);
            dic["b"].ShouldBe(pB);

            dic.Remove("C").ShouldBeFalse();
            dic.Remove("c").ShouldBeFalse();
            dic.Remove(new Person("C", 3)).ShouldBeFalse();

            dic.Keys.Count.ShouldBe(2);
            dic.Values.Count.ShouldBe(2);

            dic.Remove("a").ShouldBeTrue();

            dic.Keys.Count.ShouldBe(1);
            dic.Values.Count.ShouldBe(1);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeTrue();
            dic.ContainsKey("B").ShouldBeTrue();
            dic.ContainsKey("b").ShouldBeTrue();

            dic.Remove(p2).ShouldBeTrue();

            dic.Keys.Count.ShouldBe(0);
            dic.Values.Count.ShouldBe(0);

            dic.Contains(p1).ShouldBeFalse();
            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("a").ShouldBeFalse();

            dic.Contains(p2).ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
            dic.ContainsKey("b").ShouldBeFalse();
        }

        [Test]
        public void When_clearing_dictionary()
        {
            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                new Person("A", 1), 
                new Person("B", 2)
            };

            dic.ShouldNotBeNull();
            dic.Count.ShouldBe(2);

            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("B").ShouldBeTrue();

            dic.Clear();

            dic.Count.ShouldBe(0);

            dic.ContainsKey("A").ShouldBeFalse();
            dic.ContainsKey("B").ShouldBeFalse();
        }

        [Test]
        public void When_adding_or_replacing_items()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("A", 11);

            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(p => p.Id);

            dic.Count.ShouldBe(0);

            dic.AddOrReplace(p1).ShouldBeTrue();

            dic.Count.ShouldBe(1);
            dic["A"].ShouldBe(p1);

            dic.AddOrReplace(p1).ShouldBeFalse();

            dic.Count.ShouldBe(1);
            dic["A"].ShouldBe(p1);

            dic.AddOrReplace(p2).ShouldBeTrue();

            dic.Count.ShouldBe(1);
            dic["A"].ShouldNotBe(p1);
            dic["A"].ShouldBe(p2);
        }

        [Test]
        public void When_copying_values()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                p1,
                p2
            };

            dic.Count.ShouldBe(2);

            var copy = new Person[3];
            dic.CopyTo(copy, 1);

            copy.Length.ShouldBe(3);
            copy[0].ShouldBeNull();
            
            copy[1].ShouldBe(p1);
            copy[2].ShouldBe(p2);
        }

        [Test]
        public void When_getting_enumerator_as_key_value_pair()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            IEnumerable<KeyValuePair<string, Person>> dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                p1,
                p2
            };

            using (var enumerator = dic.GetEnumerator())
            {
                enumerator.ShouldNotBeNull();

                enumerator.MoveNext().ShouldBeTrue();

                enumerator.Current.Key.ShouldBe("A");
                enumerator.Current.Value.ShouldBe(p1);

                enumerator.MoveNext().ShouldBeTrue();

                enumerator.Current.Key.ShouldBe("B");
                enumerator.Current.Value.ShouldBe(p2);

                enumerator.MoveNext().ShouldBeFalse();
            }
        }

        [Test]
        public void When_getting_enumerator_as_easy_dictionary()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            EasyDictionary<string, Person> dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                p1,
                p2
            };

            using (var enumerator = dic.GetEnumerator())
            {
                enumerator.ShouldNotBeNull();

                enumerator.MoveNext().ShouldBeTrue();

                enumerator.Current.ShouldBe(p1);

                enumerator.MoveNext().ShouldBeTrue();

                enumerator.Current.ShouldBe(p2);

                enumerator.MoveNext().ShouldBeFalse();
            }
        }

        [Test]
        public void When_getting_enumerator_as_ienumerable()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            IEnumerable dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                p1,
                p2
            };

            var enumerator = dic.GetEnumerator();

            enumerator.ShouldNotBeNull();

            enumerator.MoveNext().ShouldBeTrue();

            ((KeyValuePair<string, Person>)enumerator.Current).Key.ShouldBe("A");
            ((KeyValuePair<string, Person>)enumerator.Current).Value.ShouldBe(p1);

            enumerator.MoveNext().ShouldBeTrue();

            ((KeyValuePair<string, Person>)enumerator.Current).Key.ShouldBe("B");
            ((KeyValuePair<string, Person>)enumerator.Current).Value.ShouldBe(p2);

            enumerator.MoveNext().ShouldBeFalse();
        }

        [Test]
        public void When_getting_key_and_values_as_ienumerable()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            IReadOnlyDictionary<string, Person> dic = new EasyDictionary<string, Person>(p => p.Id)
            {
                p1,
                p2
            };

            dic.Count.ShouldBe(2);
            dic.Keys.ShouldBe(new [] { "A", "B" });
            dic.Values.ShouldBe(new [] { p1, p2 });
        }

        [Test]
        public void When_using_as_readonly_dictionary()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            IReadOnlyDictionary<string, Person> dic = new EasyDictionary<string, Person>(
                selector, 10, comparer)
            {
                p1,
                p2
            };

            dic.ShouldNotBeNull();
            dic.Count.ShouldBe(2);
            
            dic.Keys.ShouldBe(new [] { "A", "B" });
            dic.Values.ShouldBe(new [] { p1, p2 });

            dic.ContainsKey("A").ShouldBeTrue();
            dic.ContainsKey("a").ShouldBeTrue();

            dic.ContainsKey("B").ShouldBeTrue();
            dic.ContainsKey("b").ShouldBeTrue();
            
            dic.ContainsKey("C").ShouldBeFalse();
            dic.ContainsKey("c").ShouldBeFalse();

            Person found;
            dic.TryGetValue("A", out found).ShouldBeTrue();
            found.ShouldBe(p1);

            dic.TryGetValue("a", out found).ShouldBeTrue();
            found.ShouldBe(p1);

            dic.TryGetValue("B", out found).ShouldBeTrue();
            found.ShouldBe(p2);

            dic.TryGetValue("b", out found).ShouldBeTrue();
            found.ShouldBe(p2);
        }

        [Test]
        public void When_using_as_collection()
        {
            var p1 = new Person("A", 1);
            var p2 = new Person("B", 2);

            var comparer = StringComparer.OrdinalIgnoreCase;
            Func<Person, string> selector = p => p.Id;
            ICollection<Person> dic = new EasyDictionary<string, Person>(
                selector, 10, comparer)
            {
                p1,
                p2
            };

            dic.ShouldNotBeNull();
            dic.Count.ShouldBe(2);

            dic.Contains(p1).ShouldBeTrue();
            dic.Contains(p2).ShouldBeTrue();

            var p3 = new Person("C", 3);
            dic.Add(p3);

            dic.Contains(p3).ShouldBeTrue();

            dic.Remove(p1).ShouldBeTrue();

            dic.Count.ShouldBe(2);

            dic.Contains(p1).ShouldBeFalse();

            dic.Clear();

            dic.Count.ShouldBe(0);
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
}