namespace Easy.Common.Tests.Unit.TimeSpan;

using System;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class TimeSpanExtensionsTests
{
    [Test]
    public void When_humanizing_timespans()
    {
        TimeSpan.Zero.Humanize()
            .ShouldBe("0");

        TimeSpan.MaxValue.Humanize()
            .ShouldBe("+\u221e");

        TimeSpan.MinValue.Humanize()
            .ShouldBe("-\u221e");

        TimeSpan.FromDays(365).Humanize()
            .ShouldBe("1y");

        TimeSpan.FromDays(547.5).Humanize()
            .ShouldBe("1.5y");

        TimeSpan.FromDays(31).Humanize()
            .ShouldBe("1M");

        TimeSpan.FromDays(47).Humanize()
            .ShouldBe("1.5161290322580645M");

        TimeSpan.FromDays(30).Humanize()
            .ShouldBe("4.285714285714286w");

        TimeSpan.FromDays(7).Humanize()
            .ShouldBe("1w");

        TimeSpan.FromDays(14).Humanize()
            .ShouldBe("2w");

        TimeSpan.FromDays(6).Humanize()
            .ShouldBe("6d");

        TimeSpan.FromDays(1).Humanize()
            .ShouldBe("1d");

        TimeSpan.FromHours(24).Humanize()
            .ShouldBe("1d");

        TimeSpan.FromHours(23).Humanize()
            .ShouldBe("23h");

        TimeSpan.FromHours(1).Humanize()
            .ShouldBe("1h");

        TimeSpan.FromMinutes(59).Humanize()
            .ShouldBe("59m");

        TimeSpan.FromMinutes(1).Humanize()
            .ShouldBe("1m");

        TimeSpan.FromSeconds(59).Humanize()
            .ShouldBe("59s");

        TimeSpan.FromSeconds(1).Humanize()
            .ShouldBe("1s");

        TimeSpan.FromMilliseconds(999).Humanize()
            .ShouldBe("999ms");

        TimeSpan.FromMilliseconds(1).Humanize()
            .ShouldBe("1ms");

        TimeSpan.FromMicroseconds(1_000).Humanize()
            .ShouldBe("1ms");

        TimeSpan.FromMicroseconds(999).Humanize()
            .ShouldBe("999µs");

        TimeSpan.FromMicroseconds(1).Humanize()
            .ShouldBe("1µs");

        TimeSpan.FromMicroseconds(.1).Humanize()
            .ShouldBe("100ns");
    }
}