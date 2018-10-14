namespace Easy.Common.Tests.Unit.StopwatchHelper
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shouldly;
    using StopwatchHelper = Easy.Common.StopwatchHelper;

    [TestFixture]
    internal sealed class StopwatchHelperTests
    {
        [Test]
        public async Task When_getting_duration_in_milliseconds_since()
        {
            const int DELAY_DURATION = 150;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(150).ConfigureAwait(false);

            var duration = StopwatchHelper.GetDurationInMillisecondsSince(start);
            duration.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }

        [Test]
        public async Task When_getting_duration_in_seconds_since()
        {
            const int DELAY_DURATION = 1;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(DELAY_DURATION * 1000).ConfigureAwait(false);

            var duration = StopwatchHelper.GetDurationInSecondsSince(start);
            duration.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }

        [Test]
        public async Task When_getting_duration_since()
        {
            const int DELAY_DURATION = 200;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(DELAY_DURATION).ConfigureAwait(false);

            var duration = StopwatchHelper.GetDurationSince(start);
            duration.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }

        [Test]
        public async Task When_getting_difference_in_milliseconds()
        {
            const int DELAY_DURATION = 150;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(150).ConfigureAwait(false);

            var end = Stopwatch.GetTimestamp();

            var duration = StopwatchHelper.GetDurationInMilliseconds(start, end);
            duration.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }

        [Test]
        public async Task When_getting_difference_in_seconds()
        {
            const int DELAY_DURATION = 1;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(DELAY_DURATION * 1000).ConfigureAwait(false);

            var end = Stopwatch.GetTimestamp();

            var duration = StopwatchHelper.GetDurationInSeconds(start, end);
            duration.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }

        [Test]
        public async Task When_getting_difference()
        {
            const int DELAY_DURATION = 200;
            
            var start = Stopwatch.GetTimestamp();

            await Task.Delay(DELAY_DURATION).ConfigureAwait(false);

            var end = Stopwatch.GetTimestamp();

            var duration = StopwatchHelper.GetDuration(start, end);
            duration.TotalMilliseconds.ShouldBeGreaterThanOrEqualTo(DELAY_DURATION);
        }
    }
}