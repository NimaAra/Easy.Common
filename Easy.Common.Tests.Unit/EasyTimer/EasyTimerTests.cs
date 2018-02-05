namespace Easy.Common.Tests.Unit.EasyTimer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;
    using EasyTimer = Easy.Common.EasyTimer;

    [TestFixture]
    internal sealed class EasyTimerTests
    {
        [Test]
        public async Task When_running_timer_with_action()
        {
            var cts = new CancellationTokenSource();
            
            int counter = 0;
            Action work = () => Interlocked.Increment(ref counter);
            Task timerTask = EasyTimer.Start(
                work, 
                100.Milliseconds(),
                cts.Token);

            timerTask.Status.ShouldBeOneOf(
                TaskStatus.WaitingForActivation, TaskStatus.WaitingToRun, TaskStatus.Running);

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1000);

            timerTask.Status.ShouldBe(TaskStatus.WaitingForActivation);
            
            cts.Cancel();
            
            counter.ShouldBeGreaterThan(5);
            timerTask.Status.ShouldBe(TaskStatus.RanToCompletion);
        }

        [Test]
        public async Task When_running_timer_with_task()
        {
            var cts = new CancellationTokenSource();
            
            int counter = 0;
            Func<Task> work = async () =>
            {
                await Task.Yield();
                Interlocked.Increment(ref counter);
            };

            Task timerTask = EasyTimer.Start(
                work, 
                100.Milliseconds(),
                cts.Token);

            timerTask.Status.ShouldBeOneOf(
                TaskStatus.WaitingForActivation, TaskStatus.WaitingToRun, TaskStatus.Running);

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay(1000);

            timerTask.Status.ShouldBe(TaskStatus.WaitingForActivation);
            
            cts.Cancel();
            
            counter.ShouldBeGreaterThan(5);
            timerTask.Status.ShouldBe(TaskStatus.RanToCompletion);
        }
    }
}