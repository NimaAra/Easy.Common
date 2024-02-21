namespace Easy.Common.Tests.Unit.TimerAndClockTests;

using System.Threading;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class TimerClockTests
{
    [Test]
    public void When_scheduling_an_action_to_execute_at_every_interval()
    {
        var counter = 0;

        using var clock = new TimerClock(10.Milliseconds());
        clock.Tick += (sender, args) => Interlocked.Increment(ref counter);

        clock.Enabled.ShouldBeFalse();

        clock.Enabled = true;

        Thread.Sleep(1.Seconds());
                
        var snapshot = counter;

        snapshot.ShouldBeInRange(10, 200);

        clock.Enabled.ShouldBeTrue();
        clock.Enabled = false;
        clock.Enabled.ShouldBeFalse();
    }

    [Test]
    public void When_scheduling_multiple_actions_to_execute_at_every_interval()
    {
        using var clockA = new TimerClock(500.Milliseconds());
        using var clockB = new TimerClock(100.Milliseconds());
        var counter = 0;

        clockA.Tick += (sender, args) => Interlocked.Increment(ref counter);
        clockA.Enabled = true;

        Thread.Sleep(900.Milliseconds());

        var snapshotA = counter;

        snapshotA.ShouldBe(1);

        clockB.Tick += (sender, args) => Interlocked.Increment(ref counter);
        clockB.Enabled = true;

        Thread.Sleep(1.Seconds());

        var snapshotB = counter;
        snapshotB.ShouldBeInRange(7, 20);
    }

    [Test]
    public void When_scheduling_an_action_to_execute_after_a_timeout()
    {
        using var clock = new TimerClock(300.Milliseconds());
        var counter = 0;

        clock.Tick += (sender, args) =>
        {
            var clockCopy = sender as TimerClock;
            // ReSharper disable once PossibleNullReferenceException
            clockCopy.Enabled = false;
            counter += 666;
        };
        clock.Enabled = true;

        Thread.Sleep(100.Milliseconds());

        counter.ShouldBe(0);
        
        Thread.Sleep(1.Seconds());

        counter.ShouldBe(666);
    }
}