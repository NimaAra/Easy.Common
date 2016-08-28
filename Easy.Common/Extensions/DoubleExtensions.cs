namespace Easy.Common.Extensions
{
    using System;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="double"/>.
    /// </summary>
    public static class DoubleExtensions
    {
        /// <summary>
        /// Returns the number of decimal places for a given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The double value</param>
        /// <returns>The number of decimal places</returns>
        public static uint GetDecimalPlaces(this double value)
        {
            Ensure.Not(double.IsNaN(value) || double.IsInfinity(value) || value.Equals(double.MaxValue)
                || value.Equals(double.MinValue) || value.Equals(double.Epsilon),
                "Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: " + value);

            if (value.IsDefault()) { return 0; }
            return BitConverter.GetBytes(decimal.GetBits((decimal)value)[3])[2];
        }

        /// <summary>
        /// Returns the Floor of the given <paramref name="value"/> to the specified <paramref name="decimalPlaces"/> decimal places.
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="decimalPlaces">The number of decimal places</param>
        /// <returns>The Floor as double value</returns>
        public static double Floor(this double value, uint decimalPlaces)
        {
            if (decimalPlaces == 0 || double.IsNaN(value) || double.IsInfinity(value)
                || value.Equals(double.MaxValue) || value.Equals(double.MinValue) || value.Equals(double.Epsilon))
            {
                return value;
            }

            // We need to promote to decimal to avoid the double approximation
            var copy = (decimal)value;
            var power = (decimal)Math.Pow(10, decimalPlaces);
            return (double)(Math.Floor(copy * power) / power);
        }


        /// <summary>
        /// Returns the Ceiling of the given <paramref name="value"/> to the specified <paramref name="decimalPlaces"/> decimal places.
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="decimalPlaces">The number of decimal places</param>
        /// <returns>The Ceiling as double value</returns>
        public static double Ceiling(this double value, uint decimalPlaces)
        {
            if (decimalPlaces == 0 || double.IsNaN(value) || double.IsInfinity(value)
                || value.Equals(double.MaxValue) || value.Equals(double.MinValue) || value.Equals(double.Epsilon))
            {
                return value;
            }

            // We need to promote to decimal to avoid the double approximation
            var copy = (decimal)value;
            var power = (decimal)Math.Pow(10, decimalPlaces);
            return (double)(Math.Ceiling(copy * power) / power);
        }

        /// <summary>
        /// Determines if two double values are almost equal based on a specified tolerance.
        /// </summary>
        /// <param name="left">The left double</param>
        /// <param name="right">The right double</param>
        /// <param name="epsilon">The tolerance</param>
        /// <returns>Result of equality</returns>
        public static bool EqualsFuzzy(this double left, double right, double epsilon = 0.00001)
        {
            if (left.Equals(right)) { return true; }

            if (double.IsInfinity(left) || double.IsNaN(left) || double.IsInfinity(right) || double.IsNaN(right))
            {
                return left.Equals(right);
            }

            var diff = Math.Abs(left - right);
            if (diff < epsilon) { return true; }
            return false;
        }
    }

}