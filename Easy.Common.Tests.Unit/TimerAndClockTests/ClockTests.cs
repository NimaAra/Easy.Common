namespace Easy.Common.Tests.Unit.TimerAndClockTests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Easy.Common;
using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class ClockTests
{
    [Test]
    public void When_testing_clock_as_local()
    {
        IClock clock = Clock.Instance;
        DateTime localNowFromDateTime = DateTime.Now;
        DateTimeOffset localNowFromClock = clock.Now;

        localNowFromClock.DateTime.Kind.ShouldBe(DateTimeKind.Unspecified);
        localNowFromClock.Date.ShouldBe(localNowFromDateTime.Date);
        localNowFromClock.Year.ShouldBe(localNowFromDateTime.Year);
        localNowFromClock.Month.ShouldBe(localNowFromDateTime.Month);
        localNowFromClock.Day.ShouldBe(localNowFromDateTime.Day);
        localNowFromClock.DayOfYear.ShouldBe(localNowFromDateTime.DayOfYear);
        localNowFromClock.DayOfWeek.ShouldBe(localNowFromDateTime.DayOfWeek);
        localNowFromClock.Hour.ShouldBe(localNowFromDateTime.Hour);
        localNowFromClock.Minute.ShouldBe(localNowFromDateTime.Minute);
    }

    [Test]
    [Ignore("Performance monitoring only")]
    public void When_running_clock_to_measure_performance()
    {
        Parallel.Invoke(Action, Action, Action, Action);

        static void Action()
        {
            IClock clock = Clock.Instance;
            var distinctValues = new HashSet<DateTimeOffset>();
            var sw = Stopwatch.StartNew();

            while (sw.Elapsed.TotalSeconds < 5)
            {
                distinctValues.Add(clock.Now);
            }

            sw.Stop();

            Trace.WriteLine($"Precision: {sw.Elapsed.TotalMilliseconds / distinctValues.Count:0.000000} ms ({distinctValues.Count.ToString()} samples)");
        }

        /*
         * Precision: 0.005160 ms(969122 samples)
         * Precision: 0.003269 ms(1529764 samples)
         * Precision: 0.003143 ms(1591150 samples)
         * Precision: 0.005149 ms(971052 samples)
         */
    }
}