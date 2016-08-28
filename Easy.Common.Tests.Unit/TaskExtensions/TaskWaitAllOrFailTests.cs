namespace Easy.Common.Tests.Unit.TaskExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class TaskWaitAllOrFailTests
    {
        [Test]
        public void Then_waiting_for_all_successfull_tasks_should_yield_correct_result()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                return 1;
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                return 3;
            });

            var task = new[] {t1, t2, t3}.WhenAllOrFail();

            task.Result.Length.ShouldBe(3);
            task.Status.ShouldBe(TaskStatus.RanToCompletion);

            task.Result[0].ShouldBe(1);
            task.Result[1].ShouldBe(2);
            task.Result[2].ShouldBe(3);
        }
        
        [Test]
        public void Then_waiting_for_all_cancelled_tasks_should_yield_correct_result()
        {
            var ct = new CancellationToken(true);
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                ct.ThrowIfCancellationRequested();
                return 1;
            }, ct);

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                ct.ThrowIfCancellationRequested(); 
                return 2;
            }, ct);

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                ct.ThrowIfCancellationRequested();
                return 3;
            }, ct);

            var task = new[] {t1, t2, t3}.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .InnerException.ShouldBeOfType<TaskCanceledException>();
            task.Status.ShouldBe(TaskStatus.Canceled);
        }

        [Test]
        public void Then_waiting_for_some_asks_and_first_task_cancelled_should_yield_correct_result()
        {
            var ct = new CancellationToken(true);
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                ct.ThrowIfCancellationRequested();
                return 1;
            }, ct);

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                return 3;
            });

            var task = new[] { t1, t2, t3 }.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .InnerException.ShouldBeOfType<TaskCanceledException>();
            task.Status.ShouldBe(TaskStatus.Canceled);
        }

        [Test]
        public void Then_waiting_for_some_asks_and_second_task_cancelled_should_yield_correct_result()
        {
            var ct = new CancellationToken(true);
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                return 1;
            }, ct);

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                ct.ThrowIfCancellationRequested();
                return 2;
            }, ct);

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                return 3;
            }, ct);

            var task = new[] { t1, t2, t3 }.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .InnerException.ShouldBeOfType<TaskCanceledException>();
            task.Status.ShouldBe(TaskStatus.Canceled);
        }
    
        [Test]
        public void Then_waiting_for_some_asks_and_third_task_cancelled_should_yield_correct_result()
        {
            var ct = new CancellationToken(true);
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                return 1;
            }, ct);

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                return 2;
            }, ct);

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                ct.ThrowIfCancellationRequested();
                return 3;
            }, ct);

            var task = new[] { t1, t2, t3 }.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .InnerException.ShouldBeOfType<TaskCanceledException>();
            task.Status.ShouldBe(TaskStatus.Canceled);
        }

        [Test]
        public void Then_waiting_for_some_asks_and_first_task_faulted_should_yield_correct_result()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                var zero = 0;
                return 1 / zero;
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                return 3;
            });

            var task = new[] {t1, t2, t3}.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .Flatten().InnerException.ShouldBeOfType<DivideByZeroException>();
            task.Status.ShouldBe(TaskStatus.Faulted);
        }
        
        [Test]
        public void Then_waiting_for_some_asks_and_second_task_faulted_should_yield_correct_result()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                return 1;
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                var zero = 0;
                return 2 / zero;
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                return 3;
            });

            var task = new[] {t1, t2, t3}.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .Flatten().InnerException.ShouldBeOfType<DivideByZeroException>();

            task.Status.ShouldBe(TaskStatus.Faulted);
        }
        
        [Test]
        public void Then_waiting_for_some_asks_and_third_task_faulted_should_yield_correct_result()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                return 1;
            });

            var t2 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                return 2;
            });

            var t3 = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(150);
                var zero = 0;
                return 3 / zero;
            });

            var task = new[] {t1, t2, t3}.WhenAllOrFail();

            Action result = () => task.Result.ToString();
            result.ShouldThrow<AggregateException>()
                .Flatten().InnerException.ShouldBeOfType<DivideByZeroException>();

            task.Status.ShouldBe(TaskStatus.Faulted);
        }
    }
}