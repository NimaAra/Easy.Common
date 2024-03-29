﻿namespace Easy.Common.Tests.Unit.ProducerConsumer;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class ProducerConsumerQueueTests
{
    readonly Func<int, MyClass> _getData = n => new MyClass { Id = n };

    [Test]
    public void CreatingWithNegativeBoundedCapacity()
    {
        const uint BoundedCapacity = 0;

        Action action = () =>
        {
            ProducerConsumerQueue<int> _ = new (_ => { /* do nothing; */ }, 1, BoundedCapacity);
        };

        action.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("boundedCapacity should be greater than zero.");
    }

    [Test]
    public void CreatingWithPositiveBoundedCapacityAndNullConsumer()
    {
        Action action = () =>
        {
            var _ = new ProducerConsumerQueue<int>(null, 1, 1);
        };

        action.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'consumer')");
    }

    [Test]
    public void CreatingWithUnBoundedCapacity()
    {
        ProducerConsumerQueue<int> queue = new (_ => { /* do nothing; */ }, 1);
        queue.Capacity.ShouldBe(-1);
    }

    [Test]
    public void CreatingWithPositiveBoundedCapacityAndValidConsumerButZeroConcurrencyLevel()
    {
        Action action = () =>
        {
            ProducerConsumerQueue<int> _ = new (_ => { /* do nothing; */ }, 0, 1);
        };

        action.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("maxConcurrencyLevel should be greater than zero.");
    }

    [Test]
    public void CreatingWithPositiveBoundedCapacityAndValidConsumerAndValidWorker()
    {
        ProducerConsumerQueue<int> queue = new (_ => { /* do nothing; */ }, 2, 1);

        queue.Capacity.ShouldBe(1);
        queue.MaximumConcurrencyLevel.ShouldBe<uint>(2);
        queue.PendingCount.ShouldBe<uint>(0);
        queue.PendingItems.ShouldBeEmpty();
    }

    [Test]
    public void ConsumerWithOneWorkerShouldProcessEveryAddedItems()
    {
        Exception exceptionThrown = null;
        ConcurrentBag<MyClass> consumed = new ();

        Action<MyClass> consumer = x => { consumed.Add(x); };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 1);
        queue.OnException += (_, args) =>
        {
            using (consumed.Lock(100.Milliseconds())) { exceptionThrown = args; }
        };

        10.Times(n => queue.Add(_getData(n)));
        queue.CompleteAdding();

        queue.Completion.Wait(100.Milliseconds()).ShouldBeTrue();

        consumed.Count.ShouldBe(10);
        exceptionThrown.ShouldBeNull();
    }

    [Test]
    public void BlockingConsumerWithOneWorkerShouldBlockEverything()
    {
        var exceptionThrown = false;
        ConcurrentBag<MyClass> consumed = new ();

        Action<MyClass> consumer = x =>
        {
            Thread.Sleep(1.Minutes());
            consumed.Add(x);
        };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 1);
        queue.OnException += (_, _) =>
        {
            using (consumed.Lock(100.Milliseconds())) { exceptionThrown = true; }
        };

        queue.PendingCount.ShouldBe<uint>(0);

        10.Times(n => queue.Add(_getData(n)));

        Thread.Sleep(100.Milliseconds());

        queue.PendingCount.ShouldBe<uint>(9);

        queue.Completion.Wait(600.Milliseconds()).ShouldBeFalse();
        consumed.Count.ShouldBe(0);
        exceptionThrown.ShouldBeFalse();
    }

    [Test]
    public void BlockingConsumerWithTwoWorkersShouldBlockOnlyOneItem()
    {
        bool exceptionThrown = false;
        ConcurrentBag<MyClass> consumed = new ();

        int counter = 0;
        Action<MyClass> consumer = x =>
        {
            if (counter == 0)
            {
                counter++;
                Thread.Sleep(1.Minutes());
            }
            consumed.Add(x);
        };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 2);
        queue.OnException += (_, _) =>
        {
            using (consumed.Lock(100.Milliseconds())) { exceptionThrown = true; }
        };

        10.Times(n => queue.Add(_getData(n)));

        Thread.Sleep(100);

        exceptionThrown.ShouldBeFalse();
        queue.PendingCount.ShouldBe<uint>(0);
        queue.Completion.Wait(500.Milliseconds()).ShouldBeFalse();
        consumed.Count.ShouldBe(9);
    }

    [Test]
    public void BlockingConsumerWithOneWorkerAndFullCapacityShouldReturnFalseWhenTryingToAddNewItems()
    {
        ProducerConsumerQueue<string> queue = new (_ => Thread.Sleep(10.Seconds()), 1, 2);

        List<ProducerConsumerQueueException> exceptionsThrown = new ();
        queue.OnException += (_, exception) =>
        {
            using (exceptionsThrown.Lock(100.Milliseconds()))
            {
                exceptionsThrown.Add(exception);
            }
        };

        queue.MaximumConcurrencyLevel.ShouldBe<uint>(1);
        queue.Capacity.ShouldBe(2);
        queue.PendingCount.ShouldBe<uint>(0);

        queue.TryAdd("Foo").ShouldBeTrue();
        Thread.Sleep(200);
        queue.PendingCount.ShouldBe<uint>(0, "Because the worker has already removed the item from the queue.");

        queue.TryAdd("Bar").ShouldBeTrue();
        Thread.Sleep(200);
        queue.PendingCount.ShouldBe<uint>(1, "Because the worker is now blocked so no more item can be removed from the queue.");

        queue.TryAdd("Yaboo").ShouldBeTrue();
        Thread.Sleep(200);
        queue.PendingCount.ShouldBe<uint>(2);

        queue.TryAdd("Baboo").ShouldBeFalse("Because the bounded capacity has been reached.");
        queue.PendingCount.ShouldBe<uint>(2);

        queue.TryAdd("Taboo").ShouldBeFalse();
        queue.PendingCount.ShouldBe<uint>(2);

        using (exceptionsThrown.Lock(200.Milliseconds()))
        {
            exceptionsThrown.ShouldBeEmpty();
        }
    }

    [Test]
    public void SlowConsumerWithFullCapacityShouldReturnFalseWhenTryingToAddNewItemWithinSpecifiedTimeout()
    {
        ConcurrentBag<Exception> exceptions = new ();
        ConcurrentBag<int> consumedItems = new ();

        using ProducerConsumerQueue<int> queue = new (i =>
            {
                Thread.Sleep(25);
                consumedItems.Add(i);
            },
            maxConcurrencyLevel: 1,
            boundedCapacity: 1);
        queue.OnException += (_, e) => exceptions.Add(e);
                
        // Sanity checks
        consumedItems.ShouldBeEmpty();
        queue.MaximumConcurrencyLevel.ShouldBe<uint>(1);
        queue.Capacity.ShouldBe(1);
        queue.PendingCount.ShouldBe<uint>(0);

        queue.TryAdd(1, 10.Milliseconds()).ShouldBeTrue();

        Thread.Sleep(50);
        consumedItems.Count.ShouldBe(1);

        queue.TryAdd(2, 10.Milliseconds()).ShouldBeTrue();
        queue.TryAdd(3, 10.Milliseconds()).ShouldBeTrue();
        queue.PendingCount.ShouldBe<uint>(1);

        queue.TryAdd(4, 10.Milliseconds()).ShouldBeFalse();

        Thread.Sleep(100);

        queue.PendingCount.ShouldBe<uint>(0);
        queue.TryAdd(5, 10.Milliseconds()).ShouldBeTrue();

        Thread.Sleep(100);

        exceptions.Count.ShouldBe(0);

        queue.PendingCount.ShouldBe<uint>(0);
        consumedItems.Count.ShouldBe(4);
    }

    [Test]
    public void DisposedQueueShouldNotAllowAddingNewItems()
    {
        ConcurrentBag<MyClass> consumed = new ();
        Action<MyClass> consumer = x => { consumed.Add(x); };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 1);

        ProducerConsumerQueueException thrownException = null;
        queue.OnException += (_, args) =>
        {
            thrownException = args;
        };

        const int WorkItems = 10;
        for (var i = 0; i < 10; i++)
        {
            queue.Add(_getData(i));
        }
        queue.CompleteAdding();

        queue.Completion.Wait(5.Seconds()).ShouldBeTrue();

        Thread.Sleep(50.Milliseconds());
        consumed.Count.ShouldBe(WorkItems);

        Assert.DoesNotThrow(() => queue.Add(new MyClass()));

        Thread.Sleep(1.Seconds());
        thrownException.ShouldNotBeNull();
        thrownException.Message.ShouldBe("Exception occurred when adding item.");
    }

    [Test]
    public void DisposedQueueShouldCancelConsumersCorrectly()
    {
        Exception exceptionThrown = null;
        ConcurrentBag<int> consumed = new ();

        ProducerConsumerQueue<int> queue = new (i =>
        {
            Thread.Sleep(50.Milliseconds());
            consumed.Add(i);
        }, 1);

        queue.OnException += (_, args) =>
        {
            using (consumed.Lock(100.Milliseconds())) { exceptionThrown = args; }
        };

        queue.Add(1);
        queue.Add(2);
        queue.Add(3);
        queue.Add(4);
        queue.Add(5);
        queue.CompleteAdding();

        queue.Completion.Wait(500.Milliseconds()).ShouldBeTrue();

        consumed.Count.ShouldBeGreaterThanOrEqualTo(2);

        Thread.Sleep(1.Seconds());
        exceptionThrown.ShouldBeNull();
    }

    [Test]
    public void ShutdownQueueAfterSomeAddsShouldResultInTheProcessOfAllAddedItemsBeforeDisposal()
    {
        ConcurrentBag<MyClass> consumed = new ();

        Action<MyClass> consumer = x => { consumed.Add(x); };

        var queue = new ProducerConsumerQueue<MyClass>(consumer, 1);
        ProducerConsumerQueueException thrownException = null;
        queue.OnException += (_, args) =>
        {
            thrownException = args;
        };

        5.Times(n => { queue.Add(_getData(n)); });
        queue.CompleteAdding();

        queue.Completion.Wait(10.Seconds()).ShouldBeTrue();

        Action afterDisposedEnqueues = () => 3.Times(n => { queue.Add(_getData(n)); });
        afterDisposedEnqueues.ShouldNotThrow();

        Thread.Sleep(100);

        consumed.Count.ShouldBe(5);

        Thread.Sleep(1.Seconds());
        thrownException.ShouldNotBeNull();
        thrownException.Message.ShouldBe("Exception occurred when adding item.");
    }

    [Test]
    public void ThrownExceptionBySingleWorkerShouldBePublishedCorrectly()
    {
        Exception exception = null;
        ConcurrentBag<MyClass> consumed = new ();

        Action<MyClass> consumer = x =>
        {
            if (x.Id % 2 != 0)
            {
                throw new InvalidDataException("Something went wrong");
            }

            consumed.Add(x);
        };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 1);
        queue.OnException += (_, args) =>
        {
            exception = args;
        };

        queue.Add(_getData(0));

        Thread.Sleep(50);
        consumed.Count.ShouldBe(1);
        exception.ShouldBeNull("Because no exception has occurred yet.");

        queue.Add(_getData(1));

        Thread.Sleep(50);
        consumed.Count.ShouldBe(1);

        exception.ShouldNotBeNull("Because an exception must have occurred.");
        exception.ShouldBeOfType<ProducerConsumerQueueException>()
            .Message.ShouldBe("Exception occurred.");

        exception.InnerException
            .ShouldBeOfType<InvalidDataException>()
            .Message.ShouldBe("Something went wrong");
    }

    [Test]
    public void ThrownExceptionByMultipleWorkersShouldBePublishedCorrectly()
    {
        ConcurrentQueue<Exception> exceptions = new ();

        Action<MyClass> consumer = x =>
        {
            Task.Delay(50.Milliseconds());
            throw new InvalidDataException("Something went wrong for: " + x.Id);
        };

        ProducerConsumerQueue<MyClass> queue = new (consumer, 2);
        queue.OnException += (_, args) =>
        {
            exceptions.Enqueue(args.InnerException);
        };

        queue.Add(_getData(0));
        queue.Add(_getData(1));

        Task.Delay(100.Milliseconds()).Wait();
        exceptions.Count.ShouldBe(2);

        exceptions.ShouldContain(e => e is InvalidDataException && e.Message.Equals("Something went wrong for: 0"));
        exceptions.ShouldContain(e => e is InvalidDataException && e.Message.Equals("Something went wrong for: 1"));
    }

    [Test]
    public void When_waiting_for_consumption_to_complete()
    {
        ConcurrentBag<Exception> exceptions = new ();
        ConcurrentQueue<int> numbers = new ();
        Action<int> work = n =>
        {
            Thread.Sleep(50.Milliseconds());
            numbers.Enqueue(n);
        };

        using ProducerConsumerQueue<int> pcq = new (work, 1);
        pcq.OnException += (_, exception) => exceptions.Add(exception);

        pcq.Add(1);
        pcq.Add(2);
        pcq.Add(3);
        pcq.CompleteAdding();

        pcq.Completion.Result.ShouldBeTrue();

        numbers.ShouldBe(new[] { 1, 2, 3 });
        exceptions.ShouldBeEmpty();

        pcq.Capacity.ShouldBe(-1);
        pcq.PendingCount.ShouldBe((uint)0);
        pcq.PendingItems.ShouldBeEmpty();
    }

    [Test]
    public void When_disposing_while_items_still_in_queue()
    {
        ConcurrentBag<Exception> exceptions = new ();
        ConcurrentBag<int> numbers = new ();
        int counter = 0;
        Action<int> work = n =>
        {
            if (Interlocked.Increment(ref counter) == 1)
            {
                numbers.Add(n);
            } else
            {
                Thread.Sleep(500.Milliseconds());
            }
        };

        ProducerConsumerQueue<int> pcq = new (work, 1);
        pcq.OnException += (_, exception) => exceptions.Add(exception);

        pcq.Add(1);
        pcq.Add(2);
        pcq.Add(3);

        Thread.Sleep(100.Milliseconds());

        pcq.Dispose();

        pcq.Completion.Result.ShouldBeFalse();

        numbers.ShouldBe(new[] { 1 });
        exceptions.ShouldBeEmpty();

        Should.Throw<ObjectDisposedException>(() => pcq.Capacity.ShouldBe(-1))
            .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'System.Collections.Concurrent.BlockingCollection`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.");

        Should.Throw<ObjectDisposedException>(() => pcq.PendingCount.ShouldBe((uint)2))
            .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'System.Collections.Concurrent.BlockingCollection`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.");
        
        Should.Throw<ObjectDisposedException>(() => pcq.PendingItems.ShouldBe(new[] { 2, 3 }))
            .Message.ShouldBe("Cannot access a disposed object.\r\nObject name: 'System.Collections.Concurrent.BlockingCollection`1[[System.Int32, System.Private.CoreLib, Version=8.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]]'.");
    }

    private class MyClass
    {
        public int Id { get; set; }
    }
}