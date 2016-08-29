namespace Easy.Common.Tests.Unit.TimerAndClockTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Easy.Common;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class ClockTests
    {
        [Test]
        public void When_testing_clock_as_utc()
        {
            using (var clock = new Clock())
            {
                var utcNowFromDateTime = DateTime.UtcNow;
                var utcNowFromClock = clock.UtcNow;

                utcNowFromClock.Kind.ShouldBe(utcNowFromDateTime.Kind);
                utcNowFromClock.Date.ShouldBe(utcNowFromDateTime.Date);
                utcNowFromClock.Year.ShouldBe(utcNowFromDateTime.Year);
                utcNowFromClock.Month.ShouldBe(utcNowFromDateTime.Month);
                utcNowFromClock.Day.ShouldBe(utcNowFromDateTime.Day);
                utcNowFromClock.DayOfYear.ShouldBe(utcNowFromDateTime.DayOfYear);
                utcNowFromClock.DayOfWeek.ShouldBe(utcNowFromDateTime.DayOfWeek);
                utcNowFromClock.Hour.ShouldBe(utcNowFromDateTime.Hour);
                utcNowFromClock.Minute.ShouldBe(utcNowFromDateTime.Minute);
                utcNowFromClock.Second.ShouldBe(utcNowFromDateTime.Second);
            }
        }

        [Test]
        public void When_testing_clock_as_local()
        {
            using (var clock = new Clock())
            {
                var localNowFromDateTime = DateTime.Now;
                var localNowFromClock = clock.Now;

                localNowFromClock.Kind.ShouldBe(localNowFromDateTime.Kind);
                localNowFromClock.Date.ShouldBe(localNowFromDateTime.Date);
                localNowFromClock.Year.ShouldBe(localNowFromDateTime.Year);
                localNowFromClock.Month.ShouldBe(localNowFromDateTime.Month);
                localNowFromClock.Day.ShouldBe(localNowFromDateTime.Day);
                localNowFromClock.DayOfYear.ShouldBe(localNowFromDateTime.DayOfYear);
                localNowFromClock.DayOfWeek.ShouldBe(localNowFromDateTime.DayOfWeek);
                localNowFromClock.Hour.ShouldBe(localNowFromDateTime.Hour);
                localNowFromClock.Minute.ShouldBe(localNowFromDateTime.Minute);
            }
        }

        [Test]
        [Ignore("Performance monitoring only")]
        public void When_running_clock_to_measure_performance()
        {
            Action action = () =>
            {
                var clock = new Clock();
                var distinctValues = new HashSet<DateTime>();
                var sw = Stopwatch.StartNew();

                while (sw.Elapsed.TotalSeconds < 5)
                {
                    distinctValues.Add(clock.UtcNow);
                }

                sw.Stop();

                Trace.WriteLine($"Precision: {sw.Elapsed.TotalMilliseconds / distinctValues.Count:0.000000} ms ({distinctValues.Count.ToString()} samples)");
                clock.Dispose();
            };

            Parallel.Invoke(action, action, action, action);

            /*
             * Precision: 0.005160 ms(969122 samples)
             * Precision: 0.003269 ms(1529764 samples)
             * Precision: 0.003143 ms(1591150 samples)
             * Precision: 0.005149 ms(971052 samples)
             */
        }
    }
}