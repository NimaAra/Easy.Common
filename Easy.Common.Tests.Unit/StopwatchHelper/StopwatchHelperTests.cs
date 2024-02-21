namespace Easy.Common.Tests.Unit.StopwatchHelper;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StopwatchHelper = Easy.Common.StopwatchHelper;

[TestFixture]
internal sealed class StopwatchHelperTests
{
    private const double Tolerance = 2;

    [Test]
    public async Task When_getting_duration_in_milliseconds_since()
    {
        const int DELAY_DURATION = 150;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(150).ConfigureAwait(false);

        double duration = StopwatchHelper.GetDurationInMillisecondsSince(start);
        duration.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }

    [Test]
    public async Task When_getting_duration_in_seconds_since()
    {
        const int DELAY_DURATION = 1;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(DELAY_DURATION * 1000).ConfigureAwait(false);

        double duration = StopwatchHelper.GetDurationInSecondsSince(start);
        duration.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }

    [Test]
    public async Task When_getting_duration_since()
    {
        const int DELAY_DURATION = 200;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(DELAY_DURATION).ConfigureAwait(false);

        TimeSpan duration = StopwatchHelper.GetDurationSince(start);
        duration.TotalMilliseconds.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }

    [Test]
    public async Task When_getting_difference_in_milliseconds()
    {
        const int DELAY_DURATION = 150;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(DELAY_DURATION).ConfigureAwait(false);

        long end = Stopwatch.GetTimestamp();

        double duration = StopwatchHelper.GetDurationInMilliseconds(start, end);
        duration.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }

    [Test]
    public async Task When_getting_difference_in_seconds()
    {
        const int DELAY_DURATION = 1;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(DELAY_DURATION * 1000).ConfigureAwait(false);

        long end = Stopwatch.GetTimestamp();

        double duration = StopwatchHelper.GetDurationInSeconds(start, end);
        duration.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }

    [Test]
    public async Task When_getting_difference()
    {
        const int DELAY_DURATION = 200;
            
        long start = Stopwatch.GetTimestamp();

        await Task.Delay(DELAY_DURATION).ConfigureAwait(false);

        long end = Stopwatch.GetTimestamp();

        TimeSpan duration = StopwatchHelper.GetDuration(start, end);
        duration.TotalMilliseconds.ShouldBeInRange(DELAY_DURATION - Tolerance, DELAY_DURATION + Tolerance);
    }
}