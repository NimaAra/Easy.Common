namespace Easy.Common.Tests.Unit.HashSetExtensions;

using System;
using System.Collections.Generic;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class HashSetExtensionsTests
{
    [Test]
    public void When_adding_multiple_items_to_an_empty_set()
    {
        HashSet<int> set = new();

        int[] items = { 1, 2, 3, 4, 4 };

        set.AddRange(items);

        set.Count.ShouldBe(4);
        set.ShouldBe(new[] { 1, 2, 3, 4 }, ignoreOrder: true);
    }

    [Test]
    public void When_adding_multiple_items_to_a_non_empty_set()
    {
        HashSet<int> set = new(new[] { 1, 2, 3 });

        int[] items = { 2, 4, 4, 5 };

        set.AddRange(items);

        set.Count.ShouldBe(5);
        set.ShouldBe(new[] { 1, 2, 3, 4, 5 }, ignoreOrder: true);
    }

    [Test]
    public void When_adding_empty_items_to_a_non_empty_set()
    {
        HashSet<int> set = new(new[] { 1, 2, 3 });

        int[] items = Array.Empty<int>();

        set.AddRange(items);

        set.Count.ShouldBe(3);
        set.ShouldBe(new[] { 1, 2, 3 }, ignoreOrder: true);
    }
}