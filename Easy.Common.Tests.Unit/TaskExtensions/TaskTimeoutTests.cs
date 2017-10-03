namespace Easy.Common.Tests.Unit.TaskExtensions
{
    using System;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class TaskTimeoutTests
    {
        [Test]
        public async Task When_executing_task_with_result_that_does_not_timeout()
        {
            var theTask = GetSomething();

            var timeoutPeriod = 3.Seconds();
            var answer = await theTask.TimeoutAfter(timeoutPeriod);
            answer.ShouldBe(42);
        }
        
        [Test]
        public void When_executing_task_with_result_that_does_timeout()
        {
            var theTask = GetSomething();

            var timeoutPeriod = 1.Seconds();
            Should.Throw<TimeoutException>(async () => await theTask.TimeoutAfter(timeoutPeriod))
                .Message.ShouldBe("Task timed out after: 00:00:01");
        }

        [Test]
        public void When_executing_task_with_that_does_not_timeout()
        {
            var theTask = DoSomething();

            var timeoutPeriod = 3.Seconds();
            Should.NotThrow(async () => await theTask.TimeoutAfter(timeoutPeriod));
        }

        [Test]
        public void When_executing_task_that_does_timeout()
        {
            var theTask = DoSomething();

            var timeoutPeriod = 1.Seconds();
            Should.Throw<TimeoutException>(async () => await theTask.TimeoutAfter(timeoutPeriod))
                .Message.ShouldBe("Task timed out after: 00:00:01");
        }

        [Test]
        public async Task When_executing_multiple_tasks_with_result_that_does_not_timeout()
        {
            var task1 = GetSomething();
            var task2 = GetSomething();
            var task3 = GetSomething();

            var tasks = new[] {task1, task2, task3};
            
            var timeoutPeriod = 3.Seconds();
            var completedTasks = await tasks.TimeoutAfter(timeoutPeriod);

            completedTasks.ShouldBeSameAs(tasks);
            completedTasks.ForEach(r => r.Result.ShouldBe(42));
        }

        [Test]
        public void When_executing_multiple_tasks_with_result_that_does_timeout()
        {
            var task1 = GetSomething();
            var task2 = GetSomething(4);
            var task3 = GetSomething();

            var theTasks = new[] { task1, task2, task3 };

            var timeoutPeriod = 3.Seconds();
            Should.Throw<TimeoutException>(async () => await theTasks.TimeoutAfter(timeoutPeriod))
                .Message.ShouldBe("At least one of the tasks timed out after: 00:00:03");
        }

        [Test]
        public void When_executing_multiple_tasks_that_does_not_timeout()
        {
            var task1 = DoSomething();
            var task2 = DoSomething();
            var task3 = DoSomething();

            var tasks = new[] { task1, task2, task3 };

            var timeoutPeriod = 3.Seconds();
            Should.NotThrow(async () => await tasks.TimeoutAfter(timeoutPeriod));
        }

        [Test]
        public void When_executing_multiple_tasks_that_does_timeout()
        {
            var task1 = DoSomething();
            var task2 = DoSomething(4);
            var task3 = DoSomething();

            var theTasks = new[] { task1, task2, task3 };

            var timeoutPeriod = 3.Seconds();
            Should.Throw<TimeoutException>(async () => await theTasks.TimeoutAfter(timeoutPeriod))
                .Message.ShouldBe("At least one of the tasks timed out after: 00:00:03");
        }

        private static async Task<int> GetSomething(int seconds = 2)
        {
            var waitFor = TimeSpan.FromSeconds(seconds);
            await Task.Delay(waitFor);
            return 42;
        }

        private static async Task DoSomething(int seconds = 2)
        {
            var waitFor = TimeSpan.FromSeconds(seconds);
            await Task.Delay(waitFor);
        }
    }
}