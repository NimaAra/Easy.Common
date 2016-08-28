namespace Easy.Common.Tests.Unit.TimerAndClockTests
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class EasyTimerTests
    {
        [Test]
        public void Testing_an_action_which_executes_at_every_interval_synchronously()
        {
            var result = 0;

            var timer = EasyTimer.SetInterval(() =>
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                "Thread: {0}".Print(threadId);
                Thread.Sleep(200);
                Interlocked.Increment(ref result);
                "{0:HH:mm:ss.fff} - Thread: {1} | Result: {2}".Print(DateTime.UtcNow, threadId, result);
            }, 100.Milliseconds());

            Thread.Sleep(700);

            result.ShouldBeGreaterThanOrEqualTo(4);
            timer.Dispose();
        }

        [Test]
        public void Testing_an_action_which_executes_at_every_interval_asynchronously()
        {
            var result = 0;

            var timer = EasyTimer.SetInterval(() =>
            {
                var threadId = Thread.CurrentThread.ManagedThreadId;
                "Thread: {0}".Print(threadId);
                Thread.Sleep(200);
                Interlocked.Increment(ref result);
                "{0:HH:mm:ss.fff} - Thread: {1} | Result: {2}".Print(DateTime.UtcNow, threadId, result);
            }, 100.Milliseconds());

            Thread.Sleep(700);

            result.ShouldBeGreaterThanOrEqualTo(4);
            timer.Dispose();
        }

        [Test]
        public void Testing_a_task_which_executes_at_every_interval()
        {
            var interval = 100.Milliseconds();

            var resultDic = new ConcurrentDictionary<DateTime, int>();

            var timer = EasyTimer.SetInterval(() =>
            {
                "{0:HH:mm:ss.fff} - Interval ticked, executing work...".Print(DateTime.UtcNow);
                resultDic.TryAdd(DateTime.UtcNow, 0);
                "{0:HH:mm:ss.fff} - Interval ticked, executed work".Print(DateTime.UtcNow);
            }, interval);

            Thread.Sleep(200);

            resultDic.Count.ShouldBeLessThanOrEqualTo(2);

            "{0} - TEST - Canceling timer".Print(DateTime.UtcNow.ToString("HH:mm:ss.fff"));
            timer.Dispose();
            "{0} - TEST - Canceled timer".Print(DateTime.UtcNow.ToString("HH:mm:ss.fff"));

            Thread.Sleep(300);

            resultDic.Count.ShouldBeLessThanOrEqualTo(2);
            Assert.DoesNotThrow(() => timer.Dispose());
        }

        [Test]
        public void Testing_a_task_which_executes_at_every_interval_but_is_canceled_before_the_fist_interval()
        {
            var result = 0;
            var interval = 100.Milliseconds();

            var timer = EasyTimer.SetInterval(() =>
            {
                Interlocked.Increment(ref result);
            }, interval);

            var assertOne = result;

            Thread.Sleep(50);
            var assertTwo = result;

            timer.Dispose();

            Thread.Sleep(30);
            var assertThree = result;

            Thread.Sleep(200);
            var assertFour = result;

            assertOne.ShouldBe(0, "Because interval has not passed yet");
            assertTwo.ShouldBe(0, "Because interval still has not passed yet");
            assertThree.ShouldBe(0, "Because the intervalTask was signaled to be canceled");
            assertFour.ShouldBe(0, "Because the intervalTask was signaled to be canceled");
            Assert.DoesNotThrow(() => timer.Dispose());
        }

        [Test]
        public void Testing_a_task_which_executes_after_a_timeout()
        {
            var result = 0;
            var timeout = 100.Milliseconds();

            var timer = EasyTimer.SetTimeout(() =>
            {
                Interlocked.Increment(ref result);
            }, timeout);

            var assertOne = result;

            Thread.Sleep(10);
            var assertTwo = result;

            Thread.Sleep(30);
            var assertThree = result;

            Thread.Sleep(120);
            var assertFour = result;

            Thread.Sleep(100);
            var assertFive = result;

            Thread.Sleep(200);
            var assertSix = result;

            Assert.AreEqual(0, assertOne, "Because timeout has not passed yet");
            Assert.AreEqual(0, assertTwo, "Because timeout still has not passed yet");
            Assert.AreEqual(0, assertThree, "Because timeout still has not passed yet");
            Assert.AreEqual(1, assertFour, "Because timeout has now passed");
            Assert.AreEqual(1, assertFive, "Because timeout has now passed and the work will only execute once");
            Assert.AreEqual(1, assertSix, "Because timeout has now passed and the work will only execute once");

            timer.Dispose();
        }

        [Test]
        public void Testing_a_task_which_executes_after_a_timeout_but_is_canceled_before_timeout_happens()
        {
            var result = 0;
            var timeout = 100.Milliseconds();

            var timer = EasyTimer.SetTimeout(() =>
            {
                Interlocked.Increment(ref result);
            }, timeout);

            var stopTimer = new Timer(state =>
            {
                var local = timer;
                local.Dispose();
            }, null, 50.Milliseconds(), TimeSpan.Zero);

            var assertOne = result;

            Thread.Sleep(20);
            var assertTwo = result;

            Thread.Sleep(500);
            var assertThree = result;

            Thread.Sleep(200);
            var assertFour = result;

            Assert.AreEqual(0, assertOne, "Because timeout has not passed yet");
            Assert.AreEqual(0, assertTwo, "Because timeout still has not passed yet");
            Assert.AreEqual(0, assertThree, "[1].Because the task was signaled to be canceled");
            Assert.AreEqual(0, assertFour, "[2].Because the task was signaled to be canceled");
            timer.Dispose();
            stopTimer.Dispose();
        }
    }
}