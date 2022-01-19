namespace Easy.Common.Tests.Unit.DoubleExtensions
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class DoubleExtensionsTests
    {
        [TestCase(-1, (uint)0)]
        [TestCase(0, (uint)0)]
        [TestCase(1, (uint)0)]
        [TestCase(0.1, (uint)1)]
        [TestCase(0.11, (uint)2)]
        [TestCase(0.110, (uint)2)]
        [TestCase(1.110, (uint)2)]
        [TestCase(21.110, (uint)2)]
        [TestCase(21.0005, (uint)4)]
        [TestCase(21.10005, (uint)5)]
        [TestCase(-21.10005, (uint)5)]
        [TestCase(-0.10005, (uint)5)]
        public void When_getting_decimal_places(double value, uint expectedResult)
        {
            value.GetDecimalPlaces().ShouldBe(expectedResult);
        }

        [Test]
        public void When_getting_decimal_places_for_invalid_double()
        {
            Should.Throw<ArgumentException>(() => double.MaxValue.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: ");

            Should.Throw<ArgumentException>(() => double.MinValue.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: ");

            Should.Throw<ArgumentException>(() => double.PositiveInfinity.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: ");

            Should.Throw<ArgumentException>(() => double.NegativeInfinity.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: ");

            Should.Throw<ArgumentException>(() => double.NaN.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: NaN");

            Should.Throw<ArgumentException>(() => double.Epsilon.GetDecimalPlaces())
                .Message.ShouldStartWith("Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: ");
        }

        [TestCase(double.MaxValue, (uint)0, double.MaxValue)]
        [TestCase(double.MinValue, (uint)0, double.MinValue)]
        [TestCase(double.PositiveInfinity, (uint)0, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, (uint)0, double.NegativeInfinity)]
        [TestCase(double.NaN, (uint)0, double.NaN)]
        [TestCase(double.Epsilon, (uint)0, double.Epsilon)]

        [TestCase(double.MaxValue, (uint)1, double.MaxValue)]
        [TestCase(double.MinValue, (uint)9, double.MinValue)]
        [TestCase(double.PositiveInfinity, (uint)5, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, (uint)7, double.NegativeInfinity)]
        [TestCase(double.NaN, (uint)3, double.NaN)]
        [TestCase(double.Epsilon, (uint)8, double.Epsilon)]

        [TestCase(0, (uint)0, 0)]
        [TestCase(-1, (uint)0, -1)]
        [TestCase(-1, (uint)0, -1)]
        [TestCase(1.02, (uint)0, 1.02)]
        [TestCase(-1.02, (uint)0, -1.02)]
        [TestCase(-1.0000402, (uint)0, -1.0000402)]
        [TestCase(-1.0000402, (uint)1, -1.1)]
        [TestCase(1.02030402, (uint)4, 1.0203)]
        [TestCase(1.02031402, (uint)4, 1.0203)]
        [TestCase(1.02039402, (uint)4, 1.0203)]
        [TestCase(1.02009402, (uint)4, 1.02)]
        [TestCase(-1.02009402, (uint)4, -1.0201)]
        [TestCase(1.02027402, (uint)2, 1.02)]
        [TestCase(1.02927402, (uint)2, 1.02)]
        [TestCase(1.02927402, (uint)3, 1.029)]
        [TestCase(-1.02927402, (uint)3, -1.03)]
        public void When_getting_the_floor_of_a_double_value_to_the_specified_decimal_places(double value, uint decimalPlaces, double expectedResult)
        {
            value.Floor(decimalPlaces).ShouldBe(expectedResult);
        }

        [TestCase(double.MaxValue, (uint)0, double.MaxValue)]
        [TestCase(double.MinValue, (uint)0, double.MinValue)]
        [TestCase(double.PositiveInfinity, (uint)0, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, (uint)0, double.NegativeInfinity)]
        [TestCase(double.NaN, (uint)0, double.NaN)]
        [TestCase(double.Epsilon, (uint)0, double.Epsilon)]

        [TestCase(double.MaxValue, (uint)1, double.MaxValue)]
        [TestCase(double.MinValue, (uint)9, double.MinValue)]
        [TestCase(double.PositiveInfinity, (uint)5, double.PositiveInfinity)]
        [TestCase(double.NegativeInfinity, (uint)7, double.NegativeInfinity)]
        [TestCase(double.NaN, (uint)3, double.NaN)]
        [TestCase(double.Epsilon, (uint)8, double.Epsilon)]

        [TestCase(0, (uint)0, 0)]
        [TestCase(-1, (uint)0, -1)]
        [TestCase(-1, (uint)0, -1)]
        [TestCase(1.02, (uint)0, 1.02)]
        [TestCase(-1.02, (uint)0, -1.02)]
        [TestCase(-1.0000402, (uint)0, -1.0000402)]
        [TestCase(-1.0000402, (uint)1, -1.0)]
        [TestCase(1.02030402, (uint)4, 1.0204)]
        [TestCase(1.02031402, (uint)4, 1.0204)]
        [TestCase(1.02039402, (uint)4, 1.0204)]
        [TestCase(1.02009402, (uint)4, 1.0201)]
        [TestCase(-1.02009402, (uint)4, -1.0200)]
        [TestCase(1.02027402, (uint)2, 1.03)]
        [TestCase(1.02927402, (uint)2, 1.030)]
        [TestCase(1.02927402, (uint)3, 1.030)]
        [TestCase(-1.02927402, (uint)3, -1.029)]
        public void When_getting_the_ceiling_of_a_double_value_to_the_specified_decimal_places(double value, uint decimalPlaces, double expectedResult)
        {
            value.Ceiling(decimalPlaces).ShouldBe(expectedResult);
        }

        [TestCase(double.MaxValue, double.MaxValue, 0.00000001, true)]
        [TestCase(double.MinValue, double.MinValue, 0.00000001, true)]
        [TestCase(double.MinValue, double.MaxValue, 0.00000001, false)]
        [TestCase(double.NaN, double.NaN, 0.00000001, true)]
        [TestCase(double.NaN, double.MaxValue, 0.00000001, false)]
        [TestCase(double.PositiveInfinity, double.PositiveInfinity, 0.00000001, true)]
        [TestCase(double.PositiveInfinity, double.NegativeInfinity, 0.00000001, false)]
        [TestCase(double.MaxValue, double.NegativeInfinity, 0.00000001, false)]
        [TestCase(double.NaN, double.NegativeInfinity, 0.00000001, false)]

        [TestCase(0, 0, 0.0, true)]
        [TestCase(0, 0, 0.0000001, true)]
        [TestCase(0, 0, 0.01, true)]
        [TestCase(1, 1, 0.01, true)]
        [TestCase(-1, -1, 0.01, true)]

        // Large
        [TestCase(1000000, 1000000, 0.00001, true)]
        [TestCase(1000001, 1000000, 0.00001, false)]
        [TestCase(10000, 10001, 0.00001, false)]
        [TestCase(10001, 10000, 0.00001, false)]

        // Negative Large
        [TestCase(-1000000, -1000001, 0.00001, false)]
        [TestCase(-1000001, -1000000, 0.00001, false)]
        [TestCase(-10000, -10001, 0.00001, false)]
        [TestCase(-10001, -10000, 0.00001, false)]

        // Around one
        [TestCase(1.0000001, 1.0000002, 0.00001, true)]
        [TestCase(1.0000002, 1.0000001, 0.00001, true)]
        [TestCase(1.0002, 1.0001, 0.00001, false)]
        [TestCase(1.0001, 1.0002, 0.00001, false)]

        // Around -one
        [TestCase(-1.000001, -1.000002, 0.00001, true)]
        [TestCase(-1.000002, -1.000001, 0.00001, true)]
        [TestCase(-1.0001, -1.0002, 0.00001, false)]
        [TestCase(-1.0002, -1.0001, 0.00001, false)]

        // Between 1 and 0
        [TestCase(0.000000001000001, 0.000000001000002, 0.00001, true)]
        [TestCase(0.000000001000002, 0.000000001000001, 0.00001, true)]
        [TestCase(0.000000000001002, 0.000000000001001, 0.00001, true)]
        [TestCase(0.000000000001001, 0.000000000001002, 0.00001, true)]

        // Between -1 and 0
        [TestCase(-0.000000001000001, -0.000000001000002, 0.00001, true)]
        [TestCase(-0.000000001000002, -0.000000001000001, 0.00001, true)]
        [TestCase(-0.000000000001002, -0.000000000001001, 0.00001, true)]
        [TestCase(-0.000000000001001, -0.000000000001002, 0.00001, true)]

        // Involving 0
        [TestCase(0.0, 0.0, 0.00001, true)]
        [TestCase(0.0, -0.0, 0.00001, true)]
        [TestCase(-0.0, -0.0, 0.00001, true)]
        [TestCase(0.00000001, 0.0, 0.00001, true)]
        [TestCase(0.00000001, 0.0, double.Epsilon, false)]
        [TestCase(0.0, 0.00000001, 0.00001, true)]
        [TestCase(0.0, 0.00000001, double.Epsilon, false)]
        [TestCase(-0.00000001, 0.0, 0.00001, true)]
        [TestCase(-0.00000001, 0.0, double.Epsilon, false)]
        [TestCase(0.0, -0.00000001, 0.00001, true)]
        [TestCase(0.0, -0.00000001, double.Epsilon, false)]

        // Involving Extreme values (overflow potential
        [TestCase(double.MaxValue, double.MaxValue, 0.00001, true)]
        [TestCase(double.MaxValue, -double.MaxValue, 0.00001, false)]
        [TestCase(-double.MaxValue, double.MaxValue, 0.00001, false)]
        [TestCase(double.MaxValue, double.MaxValue / 2, 0.00001, false)]
        [TestCase(double.MaxValue, -double.MaxValue / 2, 0.00001, false)]
        [TestCase(-double.MaxValue, double.MaxValue / 2, 0.00001, false)]

        // Involving Infinities
        [TestCase(double.PositiveInfinity, double.PositiveInfinity, 0.00001, true)]
        [TestCase(double.NegativeInfinity, double.NegativeInfinity, 0.00001, true)]
        [TestCase(double.NegativeInfinity, double.PositiveInfinity, 0.00001, false)]
        [TestCase(double.PositiveInfinity, double.MaxValue, 0.00001, false)]
        [TestCase(double.NegativeInfinity, -double.MaxValue, 0.00001, false)]

        // Involving NaN
        [TestCase(double.NaN, double.NaN, 0.00001, true)]
        [TestCase(double.NaN, 0.0, 0.00001, false)]
        [TestCase(-0.0, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, -0.0, 0.00001, false)]
        [TestCase(0.0, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, double.PositiveInfinity, 0.00001, false)]
        [TestCase(double.PositiveInfinity, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, double.NegativeInfinity, 0.00001, false)]
        [TestCase(double.NegativeInfinity, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, double.MaxValue, 0.00001, false)]
        [TestCase(double.MaxValue, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, -double.MaxValue, 0.00001, false)]
        [TestCase(-double.MaxValue, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, double.MinValue, 0.00001, false)]
        [TestCase(double.MinValue, double.NaN, 0.00001, false)]
        [TestCase(double.NaN, -double.MinValue, 0.00001, false)]
        [TestCase(-double.MinValue, double.NaN, 0.00001, false)]

        // Involving opposite sides of 0
        [TestCase(1.000000001, -1.0, 0.00001, false)]
        [TestCase(-1.0d, 1.000000001, 0.00001, false)]
        [TestCase(-1.000000001, 1.0, 0.00001, false)]
        [TestCase(1.0, -1.000000001, 0.00001, false)]
        [TestCase(10 * double.MinValue, 10 * -double.MinValue, 0.00001, false)]
        [TestCase(10000 * double.MinValue, 10000 * -double.MinValue, 0.00001, false)]

        // Involving very close to 0
        [TestCase(double.MinValue, double.MinValue, 0.00001, true)]
        [TestCase(double.MinValue, -double.MinValue, 0.00001, false)]
        [TestCase(-double.MinValue, double.MinValue, 0.00001, false)]
        [TestCase(double.MinValue, 0, 0.00001, false)]
        [TestCase(0, double.MinValue, 0.00001, false)]
        [TestCase(-double.MinValue, 0, 0.00001, false)]
        [TestCase(0, -double.MinValue, 0.00001, false)]
        [TestCase(0.000000001, -double.MinValue, 0.00001, false)]
        [TestCase(0.000000001, double.MinValue, 0.00001, false)]
        [TestCase(double.MinValue, 0.000000001, 0.00001, false)]
        [TestCase(-double.MinValue, 0.000000001, 0.00001, false)]

        [TestCase(0, 0, 0.0, true)]
        [TestCase(0, 0, 0.0000001, true)]
        [TestCase(0, 0, 0.01, true)]
        [TestCase(1, 1, 0.01, true)]
        [TestCase(-1, -1, 0.01, true)]
        [TestCase(1.1, 1.1, 0.01, true)]
        [TestCase(1.01, 1.02, 0.1, true)]
        [TestCase(1.01, 1.03, 0.1, true)]
        [TestCase(1.01, 1.03, 1, true)]
        [TestCase(1.01, 1.03, 0.001, false)]

        [TestCase(1000000, 1000000, 0.00001, true)]
        [TestCase(1000001, 1000000, 0.00001, false)]
        [TestCase(10000, 10001, 0.00001, false)]
        [TestCase(10001, 10000, 0.00001, false)]
        public void When_comparing_two_doubles_fuzzily(double left, double right, double epsilon, bool expectedResult)
        {
            left.EqualsFuzzy(right, epsilon).ShouldBe(expectedResult);
        }

        [TestCase(0, true)]
        [TestCase(-1, true)]
        [TestCase(1, true)]
        [TestCase(0.1, true)]
        [TestCase(-0.0001, true)]
        [TestCase(double.Epsilon, true)]
        [TestCase(double.MaxValue, true)]
        [TestCase(double.MinValue, true)]
        
        [TestCase(double.NaN, false)]
        [TestCase(double.NegativeInfinity, false)]
        [TestCase(double.PositiveInfinity, false)]
        public void When_checking_if_doubles_are_finite(double value, bool expectedResult)
        {
            value.IsFinite().ShouldBe(expectedResult);
        }
    }
}