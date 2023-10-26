namespace Easy.Common.Tests.Unit.TaskExtensions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class TaskGeneralTests
{
    [Test]
    public async Task When_iterating_over_tasks_finishing_in_order()
    {
        var tasks = new[]
        {
            DoSomethingAfter(500.Milliseconds()),
            DoSomethingAfter(1000.Milliseconds()),
            DoSomethingAfter(1500.Milliseconds())
        };

        var finishedTasksIds = new List<int>(3);
        Action<Task> work = t => finishedTasksIds.Add(t.Id);

        await tasks.ForEachInOrder(work);

        finishedTasksIds.ShouldNotBeEmpty();
        finishedTasksIds.Count.ShouldBe(3);

        finishedTasksIds[0].ShouldBe(tasks[0].Id);
        finishedTasksIds[1].ShouldBe(tasks[1].Id);
        finishedTasksIds[2].ShouldBe(tasks[2].Id);
    }

    [Test]
    public async Task When_iterating_over_tasks_finishing_out_of_order()
    {
        var tasks = new[]
        {
            DoSomethingAfter(1500.Milliseconds()),
            DoSomethingAfter(1000.Milliseconds()),
            DoSomethingAfter(500.Milliseconds())
        };

        var finishedTasksIds = new List<int>(3);
        Action<Task> work = t => finishedTasksIds.Add(t.Id);

        await tasks.ForEachInOrder(work);

        finishedTasksIds.ShouldNotBeEmpty();
        finishedTasksIds.Count.ShouldBe(3);

        finishedTasksIds[0].ShouldBe(tasks[0].Id);
        finishedTasksIds[1].ShouldBe(tasks[1].Id);
        finishedTasksIds[2].ShouldBe(tasks[2].Id);
    }

    [Test]
    public async Task When_iterating_over_tasks_with_results__finishing_in_order()
    {
        var tasks = new[]
        {
            GetSomethingAfter(500.Milliseconds(), 1),
            GetSomethingAfter(1000.Milliseconds(), 2),
            GetSomethingAfter(1500.Milliseconds(), 3)
        };

        var finishedTasksIds = new List<int>(3);
        Action<int> work = x => finishedTasksIds.Add(x);

        await tasks.ForEachInOrder(work);

        finishedTasksIds.ShouldNotBeEmpty();
        finishedTasksIds.Count.ShouldBe(3);

        finishedTasksIds[0].ShouldBe(1);
        finishedTasksIds[1].ShouldBe(2);
        finishedTasksIds[2].ShouldBe(3);
    }

    [Test]
    public async Task When_iterating_over_tasks_with_results_finishing_out_of_order()
    {
        var tasks = new[]
        {
            GetSomethingAfter(1500.Milliseconds(), 1),
            GetSomethingAfter(1000.Milliseconds(), 2),
            GetSomethingAfter(500.Milliseconds(), 3)
        };

        var finishedTasksIds = new List<int>(3);
        Action<int> work = x => finishedTasksIds.Add(x);

        await tasks.ForEachInOrder(work);

        finishedTasksIds.ShouldNotBeEmpty();
        finishedTasksIds.Count.ShouldBe(3);

        finishedTasksIds[0].ShouldBe(1);
        finishedTasksIds[1].ShouldBe(2);
        finishedTasksIds[2].ShouldBe(3);
    }

    [Test]
    public async Task When_awaiting_multiple_tasks()
    {
        var bag = new ConcurrentBag<int>();
            
        var tasks = new Task[3];

        tasks[0] = Task.Run(() => bag.Add(1));
        tasks[1] = Task.Run(async () =>
        {
            await Task.Delay(100);
            bag.Add(2);
        });
        tasks[2] = Task.Run(async () =>
        {
            await Task.Delay(500);
            bag.Add(3);
        });

        await tasks;

        bag.Count.ShouldBe(3);
        bag.ShouldContain(1);
        bag.ShouldContain(2);
        bag.ShouldContain(3);
    }

    [Test]
    public async Task When_awaiting_multiple_tasks_returning_value()
    {
        var tasks = new[]
        {
            Task.FromResult(1),
            Task.FromResult(2),
            Task.FromResult(3)
        };

        var result = await tasks;

        result.ShouldBeOfType<int[]>();
        result.Length.ShouldBe(tasks.Length);
        result[0].ShouldBe(1);
        result[1].ShouldBe(2);
        result[2].ShouldBe(3);
    }

    private static Task DoSomethingAfter(TimeSpan waitFor) => Task.Delay(waitFor);

    private static async Task<int> GetSomethingAfter(TimeSpan waitFor, int resultToReturn)
    {
        await Task.Delay(waitFor);
        return resultToReturn;
    }
}