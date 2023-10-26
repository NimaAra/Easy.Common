namespace Easy.Common.Tests.Unit.EasyComparer;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Easy.Common.EasyComparer;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class EasyComparerTests
{
    [Test]
    public void When_getting_instance_multiple_times()
    {
        var one = EasyComparer.Instance;
        var two = EasyComparer.Instance;
        one.ShouldBeSameAs(two);
    }

    [Test]
    public void When_comparing_a_non_null_with_null()
    {
        var left = new SomeClass { Bytes = Array.Empty<byte>() };
        var right = new SomeClass { Bytes = null };

        EasyComparer.Instance.Compare(left, right, false, false, out var result).ShouldBeFalse();
            
        result.Count.ShouldBe(11);

        foreach (Variance item in result)
        {
            if (item.Property.Name == "Bytes")
            {
                item.Varies.ShouldBeTrue();
            } else
            {
                item.Varies.ShouldBeFalse();
            }
        }
    }

    [Test]
    public void When_comparing_a_reference_object_to_itself_including_base_excluding_privates()
    {
        var obj = new SomeClass();

        EasyComparer.Instance.Compare(obj, obj, true, false, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(12);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Id");
        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");

        result.ShouldNotContain(p => p.Property.Name == "SomePrivate");
        result.ShouldNotContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_a_reference_object_to_itself_including_base_including_privates()
    {
        var obj = new SomeClass();

        EasyComparer.Instance.Compare(obj, obj, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(14);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Id");
        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");
        result.ShouldContain(p => p.Property.Name == "SomePrivate");
        result.ShouldContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_a_reference_object_to_itself_excluding_base_including_privates()
    {
        var obj = new SomeClass();

        EasyComparer.Instance.Compare(obj, obj, false, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");
        result.ShouldContain(p => p.Property.Name == "SomePrivate");
        result.ShouldContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_equal_reference_objects_including_base_excluding_privates()
    {
        var left = new SomeClass
        {
            Id = 6,
            Age = 1,
            Stopwatch = new Stopwatch(),
            Bytes = new byte[] { 1, 2, 0 },
            Name = "Foo",
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" },
            SomeNullable = 42,
            SomeDate = DateTime.UtcNow,
            SomeInternal = 1_234_567
        };

        var right = new SomeClass
        {
            Id = left.Id,
            Age = left.Age,
            Stopwatch = left.Stopwatch,
            Bytes = left.Bytes,
            Name = left.Name,
            SomeArray = left.SomeArray,
            SomeList = left.SomeList,
            SomeCollection = left.SomeCollection,
            SomeEnumerable = left.SomeEnumerable,
            SomeDictionary = left.SomeDictionary,
            SomeNullable = left.SomeNullable,
            SomeDate = left.SomeDate,
            SomeInternal = left.SomeInternal
        };

        EasyComparer.Instance.Compare(left, right, true, false, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(12);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_reference_objects_including_base_including_privates()
    {
        var left = new SomeClass
        {
            Id = 6,
            Age = 1,
            Stopwatch = new Stopwatch(),
            Bytes = new byte[] { 1, 2, 0 },
            Name = "Foo",
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" },
            SomeNullable = 42,
            SomeDate = DateTime.UtcNow,
            SomeInternal = 1_234_567
        };

        var right = new SomeClass
        {
            Id = left.Id,
            Age = left.Age,
            Stopwatch = left.Stopwatch,
            Bytes = left.Bytes,
            Name = left.Name,
            SomeArray = left.SomeArray,
            SomeList = left.SomeList,
            SomeCollection = left.SomeCollection,
            SomeEnumerable = left.SomeEnumerable,
            SomeDictionary = left.SomeDictionary,
            SomeNullable = left.SomeNullable,
            SomeDate = left.SomeDate,
            SomeInternal = left.SomeInternal
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(14);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_reference_objects_with_different_collections_but_same_underlying_values()
    {
        var left = new SomeClass
        {
            Id = 6,
            Bytes = new byte[] { 1, 2, 0 },
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" }
        };

        var right = new SomeClass
        {
            Id = 6,
            Bytes = new byte[] { 1, 2, 0 },
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" }
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(14);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_reference_objects_with_different_collections()
    {
        var left = new SomeClass
        {
            Bytes = new byte[] { 1, 2, 0 },
        };

        var right = new SomeClass
        {
            Bytes = new byte[] { 0, 1, 2 },
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeFalse();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(14);
        result.Count(v => v.Varies == false)
            .ShouldBe(13);

        var varriedProperty = result.Single(p => p.Property.Name == "Bytes");
        varriedProperty.Varies.ShouldBeTrue();
        varriedProperty.LeftValue.ShouldBe(new byte[] { 1, 2, 0 });
        varriedProperty.RightValue.ShouldBe(new byte[] { 0, 1, 2 });
    }

    [Test]
    public void When_comparing_a_struct_object_to_itself_including_base_excluding_privates()
    {
        var obj = new SomeStruct();

        EasyComparer.Instance.Compare(obj, obj, true, false, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(11);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");

        result.ShouldNotContain(p => p.Property.Name == "SomePrivate");
        result.ShouldNotContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_a_struct_object_to_itself_including_base_including_privates()
    {
        var obj = new SomeStruct();

        EasyComparer.Instance.Compare(obj, obj, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");
        result.ShouldContain(p => p.Property.Name == "SomePrivate");
        result.ShouldContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_a_struct_object_to_itself_excluding_base_including_privates()
    {
        var obj = new SomeStruct();

        EasyComparer.Instance.Compare(obj, obj, false, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.ShouldAllBe(v => v.Varies == false);

        result.ShouldContain(p => p.Property.Name == "Age");
        result.ShouldContain(p => p.Property.Name == "Stopwatch");
        result.ShouldContain(p => p.Property.Name == "Bytes");
        result.ShouldContain(p => p.Property.Name == "Name");
        result.ShouldContain(p => p.Property.Name == "SomeArray");
        result.ShouldContain(p => p.Property.Name == "SomeList");
        result.ShouldContain(p => p.Property.Name == "SomeCollection");
        result.ShouldContain(p => p.Property.Name == "SomeEnumerable");
        result.ShouldContain(p => p.Property.Name == "SomeDictionary");
        result.ShouldContain(p => p.Property.Name == "SomeNullable");
        result.ShouldContain(p => p.Property.Name == "SomeDate");
        result.ShouldContain(p => p.Property.Name == "SomePrivate");
        result.ShouldContain(p => p.Property.Name == "SomeInternal");
    }

    [Test]
    public void When_comparing_equal_struct_objects_including_base_excluding_privates()
    {
        var left = new SomeStruct
        {
            Age = 1,
            Stopwatch = new Stopwatch(),
            Bytes = new byte[] { 1, 2, 0 },
            Name = "Foo",
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" },
            SomeNullable = 42,
            SomeDate = DateTime.UtcNow,
            SomeInternal = 1_234_567
        };

        var right = new SomeStruct
        {
            Age = left.Age,
            Stopwatch = left.Stopwatch,
            Bytes = left.Bytes,
            Name = left.Name,
            SomeArray = left.SomeArray,
            SomeList = left.SomeList,
            SomeCollection = left.SomeCollection,
            SomeEnumerable = left.SomeEnumerable,
            SomeDictionary = left.SomeDictionary,
            SomeNullable = left.SomeNullable,
            SomeDate = left.SomeDate,
            SomeInternal = left.SomeInternal
        };

        EasyComparer.Instance.Compare(left, right, true, false, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(11);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_struct_objects_including_base_including_privates()
    {
        var left = new SomeStruct
        {
            Age = 1,
            Stopwatch = new Stopwatch(),
            Bytes = new byte[] { 1, 2, 0 },
            Name = "Foo",
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" },
            SomeNullable = 42,
            SomeDate = DateTime.UtcNow,
            SomeInternal = 1_234_567
        };

        var right = new SomeStruct
        {
            Age = left.Age,
            Stopwatch = left.Stopwatch,
            Bytes = left.Bytes,
            Name = left.Name,
            SomeArray = left.SomeArray,
            SomeList = left.SomeList,
            SomeCollection = left.SomeCollection,
            SomeEnumerable = left.SomeEnumerable,
            SomeDictionary = left.SomeDictionary,
            SomeNullable = left.SomeNullable,
            SomeDate = left.SomeDate,
            SomeInternal = left.SomeInternal
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_struct_objects_with_different_collections_but_same_underlying_values()
    {
        var left = new SomeStruct
        {
            Bytes = new byte[] { 1, 2, 0 },
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" }
        };

        var right = new SomeStruct
        {
            Bytes = new byte[] { 1, 2, 0 },
            SomeArray = new[] { 1, 2, 3 },
            SomeList = new List<int> { 7, 8, 9 },
            SomeCollection = new Collection<int> { 4, 5, 6 },
            SomeEnumerable = Enumerable.Range(1, 4),
            SomeDictionary = new Dictionary<int, string> { [0] = "Bar" }
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeTrue();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.ShouldAllBe(v => v.Varies == false);
    }

    [Test]
    public void When_comparing_equal_struct_objects_with_different_collections()
    {
        var left = new SomeStruct
        {
            Bytes = new byte[] { 1, 2, 0 },
        };

        var right = new SomeStruct
        {
            Bytes = new byte[] { 0, 1, 2 },
        };

        EasyComparer.Instance.Compare(left, right, true, true, out var result)
            .ShouldBeFalse();

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
        result.Count.ShouldBe(13);
        result.Count(v => v.Varies == false)
            .ShouldBe(12);

        var varriedProperty = result.Single(p => p.Property.Name == "Bytes");
        varriedProperty.Varies.ShouldBeTrue();
        varriedProperty.LeftValue.ShouldBe(new byte[] { 1, 2, 0 });
        varriedProperty.RightValue.ShouldBe(new byte[] { 0, 1, 2 });
    }

    private class SomeBase
    {
        public short Id { get; set; }
    }

    private sealed class SomeClass : SomeBase
    {
        public int Age { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public byte[] Bytes { get; set; }
        public string Name { get; set; }
        public int[] SomeArray { get; set; }
        public IList<int> SomeList { get; set; }
        public ICollection<int> SomeCollection { get; set; }
        public IEnumerable<int> SomeEnumerable { get; set; }
        public IDictionary<int, string> SomeDictionary { get; set; }
        public int? SomeNullable { get; set; }
        public DateTime SomeDate { get; set; }
        private uint SomePrivate { get; set; }
        internal long SomeInternal { get; set; }
    }

    private struct SomeStruct
    {
        public int Age { get; set; }
        public Stopwatch Stopwatch { get; set; }
        public byte[] Bytes { get; set; }
        public string Name { get; set; }
        public int[] SomeArray { get; set; }
        public IList<int> SomeList { get; set; }
        public ICollection<int> SomeCollection { get; set; }
        public IEnumerable<int> SomeEnumerable { get; set; }
        public IDictionary<int, string> SomeDictionary { get; set; }
        public int? SomeNullable { get; set; }
        public DateTime SomeDate { get; set; }
        private uint SomePrivate { get; set; }
        internal long SomeInternal { get; set; }
    }
}