namespace Easy.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A Base36 Encoder and Decoder
    /// </summary>
    public static class Base36
    {
        private const string Base36Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Encode the given number into a <see cref="Base36"/>string.
        /// </summary>
        /// <param name="input">The number to encode.</param>
        /// <returns>Encoded <paramref name="input"/> as string.</returns>
        public static string Encode(long input)
        {
            Ensure.That<ArgumentException>(input >= 0, "Input cannot be negative.");

            var arr = Base36Characters.ToCharArray();
            var result = new Stack<char>();
            while (input != 0)
            {
                result.Push(arr[input % 36]);
                input /= 36;
            }
            return new string(result.ToArray());
        }

        /// <summary>
        /// Decode the <see cref="Base36"/> encoded string into a long.
        /// </summary>
        /// <param name="input">The number to decode.</param>
        /// <returns>Decoded <paramref name="input"/> as long.</returns> 
        public static long Decode(string input)
        {
            Ensure.NotNull(input, nameof(input));

            var reversed = input.ToLower().Reverse();
            long result = 0;
            var pos = 0;
            foreach (var c in reversed)
            {
                result += Base36Characters.IndexOf(c) * (long)Math.Pow(36, pos);
                pos++;
            }
            return result;
        }
    }
}
