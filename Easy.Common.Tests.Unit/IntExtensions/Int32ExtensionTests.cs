namespace Easy.Common.Tests.Unit.IntExtensions
{
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class Int32ExtensionTests
    {
        [Test]
        public void Int32_tests()
        {
            const int Number = 2;

            var ticks = Number.Ticks();
            ticks.Ticks.ShouldBe(Number);

            var milliSeconds = Number.Milliseconds();
            milliSeconds.TotalMilliseconds.ShouldBe(Number);

            var seconds = Number.Seconds();
            seconds.TotalSeconds.ShouldBe(Number);

            var minutes = Number.Minutes();
            minutes.TotalMinutes.ShouldBe(Number);
            
            var hours = Number.Hours();
            hours.TotalHours.ShouldBe(Number);
            
            var days = Number.Days();
            days.TotalDays.ShouldBe(Number);
            
            var weeks = Number.Weeks();
            weeks.TotalDays.ShouldBe(Number * 7);
        } 
    }
}