namespace Easy.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Provides a set of useful methods for working with <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensionsTests
    {
        /// <summary>
        /// Generates a random <see cref="double"/> between the given <paramref name="min"/> and <paramref name="max"/>.
        /// </summary>
        public static double GenerateRandomBetween(this Random random, double min, double max)
        {
            Ensure.That(min <= max, $"min: {min.ToString(CultureInfo.InvariantCulture)} should be less than max: {max.ToString(CultureInfo.InvariantCulture)}");
            return random.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Generates a random set of numbers between the given <paramref name="min"/> and <paramref name="max"/>.
        /// <remarks>
        /// <paramref name="max"/> is exclusive. Credit goes to: <see href="http://codereview.stackexchange.com/a/61372"/>
        /// </remarks>
        /// </summary>
        public static int[] GenerateRandomSequence(this Random random, int count, int min, int max)
        {
            Ensure.Not(max <= min || count < 0 || count > max - min && max - min > 0, $"The given range of: {min.ToString()} to {max.ToString()} " +
                                                      $"({((long)max - min).ToString()} value(s)), with the count of: " +
                                                      $"{ count.ToString()} is illegal.");

            var candidates = new HashSet<int>();

            for (var top = max - count; top < max; top++)
            {
                if (!candidates.Add(random.Next(min, top + 1)))
                {
                    candidates.Add(top);
                }
            }

            var result = candidates.ToArray();
            for (var i = result.Length - 1; i > 0; i--)
            {
                var k = random.Next(i + 1);
                var tmp = result[k];
                result[k] = result[i];
                result[i] = tmp;
            }
            return result;
        }
    }
}