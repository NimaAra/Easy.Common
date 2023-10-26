namespace Easy.Common.Tests.Unit.LinkedQueue;

using System;
using System.Collections.Generic;
using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class LinkedQueueTests
{
    [Test]
    public void When_creating_an_empty_queue()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        ILinkedQueue<int> queue = new LinkedQueue<int>();

        queue.IsReadOnly.ShouldBeFalse();
        queue.Count.ShouldBe(0);

        int result;

        queue.TryPeek(out result).ShouldBeFalse();
        result.ShouldBe(0);

        queue.TryDequeue(out result).ShouldBeFalse();
        result.ShouldBe(0);

        queue.Contains(1).ShouldBe(false);
        queue.Remove(1).ShouldBeFalse();

        Should.Throw<InvalidOperationException>(() => queue.RemoveAt(1))
            .Message.ShouldBe("Sequence contains no elements");
    }

    [Test]
    public void When_enqueueing_items()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>();

        queue.Enqueue(1);

        queue.IsReadOnly.ShouldBeFalse();
        queue.Count.ShouldBe(1);

        queue.TryPeek(out int peeked).ShouldBeTrue();
        peeked.ShouldBe(1);

        queue.Contains(1).ShouldBeTrue();

        queue.TryDequeue(out int item).ShouldBeTrue();
        item.ShouldBe(1);

        queue.TryDequeue(out int anotherItem).ShouldBeFalse();
        anotherItem.ShouldBe(0);
    }

    [Test]
    public void When_creating_a_queue_with_existing_items()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>(new[] { 1, 2, 3 });

        queue.Contains(2).ShouldBeTrue();

        queue.Enqueue(2);

        queue.ShouldBe(new[] {1, 2, 3, 2});
    }

    [Test]
    public void When_adding_and_removing_items()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>();

        queue.Enqueue(1);
        queue.Add(2);

        queue.ShouldBe(new[] { 1, 2 });

        queue.Enqueue(3);

        queue.ShouldBe(new[] { 1, 2, 3 });

        queue.RemoveAt(1);

        queue.ShouldBe(new[] { 1,  3 });

        queue.TryPeek(out int peeked).ShouldBeTrue();
        peeked.ShouldBe(1);
    }

    [Test]
    public void When_clearing_items()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>(new[] { 1, 2, 3 });

        queue.Enqueue(4);

        queue.ShouldBe(new [] {1, 2, 3, 4});

        queue.Clear();

        queue.ShouldBeEmpty();
    }

    [Test]
    public void When_copying_items_to_an_array()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>(new[] { 1, 2, 3 });

        int[] array = new int[5];

        queue.CopyTo(array, 1);
            
        queue.ShouldBe(new[] { 1, 2, 3 });
        array.ShouldBe(new [] {0, 1, 2, 3, 0});
    }

    [Test]
    public void When_enumerating_items()
    {
        ILinkedQueue<int> queue = new LinkedQueue<int>(new[] { 1, 2 });

        using IEnumerator<int> enumerator = queue.GetEnumerator();

        enumerator.Current.ShouldBe(0);
        enumerator.MoveNext().ShouldBeTrue();

        enumerator.Current.ShouldBe(1);
        enumerator.MoveNext().ShouldBeTrue();

        enumerator.Current.ShouldBe(2);
        enumerator.MoveNext().ShouldBeFalse();

        enumerator.Current.ShouldBe(2);
        enumerator.MoveNext().ShouldBeFalse();
    }
}