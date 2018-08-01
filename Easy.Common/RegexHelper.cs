namespace Easy.Common
{
    using System.Diagnostics;
    using System.Linq;
    using Easy.Common.Extensions;

    /// <summary>
    /// Provides a set of methods to help work with <see cref="System.Text.RegularExpressions.Regex"/>.
    /// </summary>
    public sealed class RegexHelper
    {
        /// <summary>
        ///  Contains characters that may be used as regular expression arguments.
        /// </summary>
        private static readonly char[] RegexCharacters =
        {
            'G', 'Z', 'A', 'n', 'W', 'w', 'v', 't', 's', 'S', 'r', 'k', 'f', 'D', 'd', 'B', 'b'
        };

        /// <summary>
        /// Converts the given regex <paramref name="pattern"/> to a case-insensitive version.
        /// <example>
        ///<c>BaR</c> will be converted to <c>[bB][aA][rR]</c>.
        /// </example>
        /// <remarks>
        /// This should be used as a much faster alternative to adding
        /// <see cref="System.Text.RegularExpressions.RegexOptions.IgnoreCase"/> or using the
        /// <c>(?i)</c> for example <c>(?i)BaR(?-i)</c>
        /// </remarks>
        /// </summary>
        [DebuggerStepThrough]
        public static string ToCaseInsensitiveRegexPattern(string pattern)
        {
            if (pattern.IsNullOrEmptyOrWhiteSpace()) { return pattern; }

            var patternIndexes = pattern.GetStartAndEndIndexes("(?<", ">").ToArray();
            var hasPattern = patternIndexes.Length > 0;
            var isInPattern = false;

            var builder = StringBuilderCache.Acquire();
            for (var i = 0; i < pattern.Length; i++)
            {
                var prev = i == 0 ? new char() : pattern[i - 1];
                var currChar = pattern[i];

                if (hasPattern)
                {
                    foreach (var pair in patternIndexes)
                    {
                        if (i >= pair.Key && i <= pair.Value)
                        {
                            isInPattern = true;
                            break;
                        }

                        isInPattern = false;
                    }
                }

                if (!char.IsLetter(currChar) 
                    || prev == '\\' && RegexCharacters.Contains(currChar)
                    || isInPattern)
                {
                    builder.Append(currChar);
                    continue;
                }

                builder.Append('[');

                if (char.IsUpper(currChar))
                {
                    builder.Append(char.ToLower(currChar)).Append(currChar);
                }
                else
                {
                    builder.Append(currChar).Append(char.ToUpper(currChar));
                }
                builder.Append(']');
            }
            return StringBuilderCache.GetStringAndRelease(builder);
        }
    }
}