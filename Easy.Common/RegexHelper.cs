namespace Easy.Common;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Easy.Common.Extensions;

/// <summary>
/// Provides a set of methods to help work with <see cref="System.Text.RegularExpressions.Regex"/>.
/// </summary>
public sealed class RegexHelper
{
    private static readonly Regex _emailPrimaryRegex = 
        new Regex(@"(@)(.+)$", RegexOptions.Compiled, 200.Milliseconds());
        
    private static readonly Regex _emailSecondaryRegex = 
        new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase, 
            250.Milliseconds());

    private readonly IdnMapping _idn = new IdnMapping();

    private bool _isValid = true;
        
    /// <summary>
    ///  Contains characters that may be used as regular expression arguments.
    /// </summary>
    private static readonly char[] RegexCharacters =
    {
        'G', 'Z', 'A', 'n', 'W', 'w', 'v', 't', 's', 'S', 'r', 'k', 'f', 'D', 'd', 'B', 'b'
    };

    /// <summary>
    /// Evaluates the given <paramref name="input"/> as a valid email address.
    /// <see href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format"/>
    /// </summary>
    [DebuggerStepThrough]
    public bool IsValidEmail(string input)
    {
        if (input.IsNullOrEmptyOrWhiteSpace()) { return false; }

        _isValid = true;

        string replaced;
        try
        {
            replaced = _emailPrimaryRegex.Replace(input, DomainMapper);
        } catch (RegexMatchTimeoutException)
        {
            return false;
        }

        if (!_isValid) { return false; }

        try
        {
            return _emailSecondaryRegex.IsMatch(replaced);
        } catch (RegexMatchTimeoutException)
        {
            return false;
        }

        string DomainMapper(Match match)
        {
            var domainName = match.Groups[2].Value;
            try {
                domainName = _idn.GetAscii(domainName);
            }
            catch (ArgumentException) {
                _isValid = false;
            }
            return match.Groups[1].Value + domainName;
        }
    }

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