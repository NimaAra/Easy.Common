namespace Easy.Common.Extensions
{
    using System;

    /// <summary>
    /// Provides a set of helper methods for working with <see cref="Guid"/>.
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Returns a <c>Base64</c> encoded <see cref="Guid"/>.
        /// <example>
        /// DRfscsSQbUu8bXRqAvcWQA== or DRfscsSQbUu8bXRqAvcWQA depending on <paramref name="trimEnd"/>.
        /// </example>
        /// <remarks>
        /// The result of this method is not <c>URL</c> safe.
        /// See: <see href="https://blog.codinghorror.com/equipping-our-ascii-armor/"/>
        /// </remarks>
        /// </summary>
        public static string AsShortCodeBase64(this Guid guid, bool trimEnd = true)
        {
            var raw = Convert.ToBase64String(guid.ToByteArray());
            return trimEnd ? raw.Substring(0, raw.Length - 2) : raw;
        }

        /// <summary>
        /// Generates a maximum of 16 character, <see cref="Guid"/> based string with very little chance of collision. 
        /// <example>3c4ebc5f5f2c4edc</example>.
        /// <remarks>
        /// The result of this method is <c>URL</c> safe.
        /// Slower than <see cref="AsShortCodeBase64"/>. 
        /// See: <see href="http://madskristensen.net/post/generate-unique-strings-and-numbers-in-c"/>
        /// </remarks>
        /// </summary>
        public static string AsShortCode(this Guid guid)
        {
            long i = 1;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var b in guid.ToByteArray())
            {
                i *= b + 1;
            }
            return (i - DateTime.Now.Ticks).ToString("x");
        }

        /// <summary>
        /// Generates a 19 character, <see cref="Guid"/> based number. 
        /// <example>4801539909457287012</example>.
        /// <remarks>
        /// Faster than <see cref="AsShortCodeBase64"/>. 
        /// See: <see href="http://madskristensen.net/post/generate-unique-strings-and-numbers-in-c"/>
        /// </remarks>
        /// </summary>
        public static long AsNumber(this Guid guid) => BitConverter.ToInt64(guid.ToByteArray(), 0);
    }
}