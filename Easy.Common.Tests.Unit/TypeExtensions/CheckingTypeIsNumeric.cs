namespace Easy.Common.Tests.Unit.TypeExtensions
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class CheckingTypeIsNumeric
    {
        [Test]
        public void Run()
        {
            typeof(bool).IsNumeric().ShouldBeFalse();
            typeof(string).IsNumeric().ShouldBeFalse();
            typeof(DateTime).IsNumeric().ShouldBeFalse();
            typeof(TimeSpan).IsNumeric().ShouldBeFalse();

            typeof(byte).IsNumeric().ShouldBeTrue();
            typeof(float).IsNumeric().ShouldBeTrue();
            typeof(decimal).IsNumeric().ShouldBeTrue();
            typeof(double).IsNumeric().ShouldBeTrue();
            typeof(short).IsNumeric().ShouldBeTrue();
            typeof(int).IsNumeric().ShouldBeTrue();
            typeof(long).IsNumeric().ShouldBeTrue();
            typeof(sbyte).IsNumeric().ShouldBeTrue();
            typeof(ushort).IsNumeric().ShouldBeTrue();
            typeof(uint).IsNumeric().ShouldBeTrue();
            typeof(ulong).IsNumeric().ShouldBeTrue();
        }
    }
}