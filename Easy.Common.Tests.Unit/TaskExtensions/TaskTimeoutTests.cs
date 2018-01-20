namespace Easy.Common.Tests.Unit.TaskExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class TaskTimeoutTests
    {
        [Test]
        public void When_executing_task_with_result_that_cancels_before_timeout()
        {
            var task = GetSomethingAfter(5.Seconds());

            var cts = new CancellationTokenSource(2.Seconds());

            Should.Throw<OperationCanceledException>(async () =>
            {
                await task.TimeoutAfter(3.Seconds(), cts.Token);
            });
        }

        [Test]
        public async Task When_executing_task_with_result_that_does_not_timeout()
        {
            var task = GetSomethingAfter(2.Seconds());

            var result = await task.TimeoutAfter(3.Seconds());
            result.ShouldBe(42);
        }
        
        [Test]
        public void When_executing_task_with_result_that_does_timeout()
        {
            var task = GetSomethingAfter(2.Seconds());

            Should.Throw<TimeoutException>(async () => await task.TimeoutAfter(1.Seconds()))
                .Message.ShouldBe("Task timed out after: 00:00:01");
        }

        [Test]
        public void When_executing_task_with_that_does_not_timeout()
        {
            var task = DoSomethingAfter(2.Seconds());

            Should.NotThrow(async () => await task.TimeoutAfter(3.Seconds()));
        }

        [Test]
        public void When_executing_task_that_cancels_before_timeout()
        {
            var task = DoSomethingAfter(5.Seconds());

            var cts = new CancellationTokenSource(2.Seconds());

            Should.Throw<OperationCanceledException>(async () =>
            {
                await task.TimeoutAfter(3.Seconds(), cts.Token);
            });
        }

        [Test]
        public void When_executing_task_that_does_timeout()
        {
            var task = DoSomethingAfter(2.Seconds());
            
            Should.Throw<TimeoutException>(async () => await task.TimeoutAfter(1.Seconds()))
                .Message.ShouldBe("Task timed out after: 00:00:01");
        }

        [Test]
        public void When_executing_multiple_tasks_with_result_that_cancel_before_timeout()
        {
            var task1 = GetSomethingAfter(2.Seconds());
            var task2 = GetSomethingAfter(2.Seconds());
            var task3 = GetSomethingAfter(2.Seconds());

            var tasks = new[] {task1, task2, task3};

            var cts = new CancellationTokenSource(500.Milliseconds());

            Should.Throw<OperationCanceledException>(async () =>
            {
                await tasks.TimeoutAfter(3.Seconds(), cts.Token);
            });
        }

        [Test]
        public async Task When_executing_multiple_tasks_with_result_that_does_not_timeout()
        {
            var task1 = GetSomethingAfter(2.Seconds());
            var task2 = GetSomethingAfter(2.Seconds());
            var task3 = GetSomethingAfter(2.Seconds());

            var tasks = new[] {task1, task2, task3};

            var completedTasks = await tasks.TimeoutAfter(3.Seconds());

            completedTasks.ShouldBeSameAs(tasks);
            completedTasks.ForEach(r => r.Result.ShouldBe(42));
        }

        [Test]
        public void When_executing_multiple_tasks_with_result_that_does_timeout()
        {
            var task1 = GetSomethingAfter(2.Seconds());
            var task2 = GetSomethingAfter(4.Seconds());
            var task3 = GetSomethingAfter(2.Seconds());

            var task = new[] { task1, task2, task3 };

            Should.Throw<TimeoutException>(async () => await task.TimeoutAfter(3.Seconds()))
                .Message.ShouldBe("At least one of the tasks timed out after: 00:00:03");
        }

        [Test]
        public void When_executing_multiple_tasks_that_cancel_before_timeout()
        {
            var task1 = DoSomethingAfter(2.Seconds());
            var task2 = DoSomethingAfter(2.Seconds());
            var task3 = DoSomethingAfter(2.Seconds());

            var tasks = new[] {task1, task2, task3};

            var cts = new CancellationTokenSource(500.Milliseconds());

            Should.Throw<OperationCanceledException>(async () =>
            {
                await tasks.TimeoutAfter(3.Seconds(), cts.Token);
            });
        }

        [Test]
        public void When_executing_multiple_tasks_that_does_not_timeout()
        {
            var task1 = DoSomethingAfter(2.Seconds());
            var task2 = DoSomethingAfter(2.Seconds());
            var task3 = DoSomethingAfter(2.Seconds());

            var tasks = new[] { task1, task2, task3 };

            Should.NotThrow(async () => await tasks.TimeoutAfter(3.Seconds()));
        }

        [Test]
        public void When_executing_multiple_tasks_that_does_timeout()
        {
            var task1 = DoSomethingAfter(2.Seconds());
            var task2 = DoSomethingAfter(4.Seconds());
            var task3 = DoSomethingAfter(2.Seconds());

            var tasks = new[] { task1, task2, task3 };

            var timeoutPeriod = 3.Seconds();
            Should.Throw<TimeoutException>(async () => await tasks.TimeoutAfter(timeoutPeriod))
                .Message.ShouldBe("At least one of the tasks timed out after: 00:00:03");
        }

        private static async Task<int> GetSomethingAfter(TimeSpan waitFor)
        {
            await Task.Delay(waitFor).ConfigureAwait(false);
            return 42;
        }

        private static async Task DoSomethingAfter(TimeSpan waitFor) 
            => await Task.Delay(waitFor).ConfigureAwait(false);
    }
}