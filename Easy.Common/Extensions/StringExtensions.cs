namespace Easy.Common.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

/// <summary>
/// Extensions for <see cref="string"/>
/// </summary>
public static class StringExtensions
{
    private static readonly char[] InvalidFileNameCharacters = Path.GetInvalidFileNameChars();
    private static readonly char[] InvalidPathCharacters = Path.GetInvalidPathChars();

    /// <summary>
    /// A nicer way of calling <see cref="string.IsNullOrEmpty(string)"/>
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// A nice way of calling the inverse of <see cref="string.IsNullOrEmpty(string)"/>
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? value) => !value.IsNullOrEmpty();

    /// <summary>
    /// A nice way of checking if a string is null, empty or whitespace 
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNullOrEmptyOrWhiteSpace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    /// <summary>
    /// A nice way of checking the inverse of (if a string is null, empty or whitespace) 
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns>
    /// <see langword="true"/> if the format parameter is not null or an empty string (""); otherwise, <see langword="false"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static bool IsNotNullOrEmptyOrWhiteSpace([NotNullWhen(true)] this string? value) => !value.IsNullOrEmptyOrWhiteSpace();

    /// <summary>
    /// Parses a string as Boolean, valid inputs are: <c>true|false|yes|no|1|0</c>.
    /// <remarks>Input is parsed as Case-Insensitive.</remarks>
    /// </summary>
    public static bool TryParseAsBool(this string value, out bool result)
    {
        const StringComparison CompPolicy = StringComparison.OrdinalIgnoreCase;

        if (value.Equals("true", CompPolicy) || value.Equals("yes", CompPolicy) || value.Equals("1", CompPolicy))
        {
            result = true;
            return true;
        }

        if (value.Equals("false", CompPolicy) || value.Equals("no", CompPolicy) || value.Equals("0", CompPolicy))
        {
            result = false;
            return true;
        }

        result = false;
        return false;
    }

    /// <summary>
    /// Allows for using strings in <see langword="null"/> coalescing operations.
    /// </summary>
    /// <param name="value">The string value to check.</param>
    /// <returns>
    /// Null if <paramref name="value"/> is empty or the original <paramref name="value"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static string? NullIfEmpty(this string? value) => value == string.Empty ? null : value;

    /// <summary>
    /// Tries to extract the value between the tag <paramref name="tagName"/> 
    /// from the <paramref name="input"/>.
    /// <remarks>This method is case insensitive.</remarks>
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="tagName">The tag whose value will be returned e.g <c>span, img</c>.</param>
    /// <param name="value">The extracted value.</param>
    /// <returns><c>True</c> if successful otherwise <c>False</c>.</returns>
    public static bool TryExtractValueFromTag(this string input, string tagName, [NotNullWhen(true)] out string? value)
    {
        string pattern = $"<{tagName}[^>]*>(.*)</{tagName}>";
        Match match = Regex.Match(input, pattern, RegexOptions.IgnoreCase);

        if (match.Success)
        {
            value = match.Groups[1].ToString();
            return true;
        }

        value = null;
        return false;
    }

    /// <summary>
    /// Returns a string array containing the trimmed substrings in this <paramref name="value"/>
    /// that are delimited by the provided <paramref name="separators"/>.
    /// </summary>
    public static string[] SplitAndTrim(this string value, params char[] separators) =>
        value.Trim()
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToArray();

    /// <summary>
    /// Checks if the <paramref name="input"/> contains the <paramref name="stringToCheckFor"/> 
    /// based on the provided <paramref name="comparison"/> rules.
    /// </summary>
    public static bool Contains(this string input, string stringToCheckFor, StringComparison comparison) =>
        input.IndexOf(stringToCheckFor, comparison) >= 0;

    /// <summary>
    /// Checks that given <paramref name="input"/> matches any of the potential matches.
    /// Inspired by: http://stackoverflow.com/a/20644611/23199
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, string match1, string match2) =>
        comparer.Equals(input, match1) || comparer.Equals(input, match2);

    /// <summary>
    /// Checks that given <paramref name="input"/> matches any of the potential matches.
    /// Inspired by: http://stackoverflow.com/a/20644611/23199
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, string match1, string match2, string match3) =>
        comparer.Equals(input, match1) || comparer.Equals(input, match2) || comparer.Equals(input, match3);

    /// <summary>
    /// Checks that given <paramref name="input"/> is in a list of 
    /// potential <paramref name="matches"/>.
    /// <remarks>Inspired by: <see href="http://stackoverflow.com/a/20644611/23199"/> </remarks>
    /// </summary>
    public static bool EqualsAny(this string input, StringComparer comparer, params string[] matches) =>
        matches.Any(x => comparer.Equals(x, input));

    /// <summary>
    /// Checks to see if the given input is a valid palindrome or not.
    /// </summary>
    public static bool IsPalindrome(this string input)
    {
        int min = 0;
        int max = input.Length - 1;
        while (true)
        {
            if (min > max) { return true; }

            var a = input[min];
            var b = input[max];
            if (char.ToLower(a) != char.ToLower(b)) { return false; }

            min++;
            max--;
        }
    }

    /// <summary>
    /// Truncates the <paramref name="input"/> to the maximum length of <paramref name="maxLength"/> 
    /// and replaces the truncated part with <paramref name="suffix"/>
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="maxLength">Total length of characters to maintain before truncation.</param>
    /// <param name="suffix">The suffix to add to the end of the truncated <paramref name="input"/></param>
    public static string Truncate(this string input, int maxLength, string suffix = "")
    {
        if (maxLength < 0) { return input; }
        if (maxLength == 0) { return string.Empty; }

        char[] chars = input.Take(maxLength).ToArray();

        if (chars.Length != input.Length)
        {
            return new string(chars) + suffix;
        }
        return new string(chars);
    }

    /// <summary>
    /// Removes different types of new lines from a given string.
    /// </summary>
    /// <param name="input">input string.</param>
    /// <returns>The given input minus any new line characters.</returns>
    public static string RemoveNewLines(this string input) => input.Replace("\n", string.Empty).Replace("\r", string.Empty);

    /// <summary>
    /// Separates a PascalCase string.
    /// </summary>
    /// <example> "ThisIsPascalCase".SeparatePascalCase(); // returns "This Is Pascal Case" </example>
    /// <param name="value">The format to split</param>
    /// <returns>The original string separated on each uppercase character.</returns>
    public static string SeparatePascalCase(this string value)
    {
        Ensure.NotNullOrEmptyOrWhiteSpace(value);
        return Regex.Replace(value, "([A-Z])", " $1").Trim();
    }

    /// <summary>
    /// Converts string to Pascal Case
    /// <example>This Is A Pascal Case String.</example>
    /// </summary>
    /// <param name="input">The given input.</param>
    /// <returns>The given <paramref name="input"/> converted to Pascal Case.</returns>
    public static string ToPascalCase(this string input)
    {
        CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
        TextInfo textInfo = cultureInfo.TextInfo;
        return textInfo.ToTitleCase(input);
    }

    /// <summary>
    /// Compares <paramref name="input"/> against <paramref name="target"/>, 
    /// the comparison is case-sensitive.
    /// </summary>
    /// <param name="input">The input string</param>
    /// <param name="target">The target string</param>
    public static bool IsEqualTo(this string input, string target)
    {
        if (input.Length != target.Length) { return false; }
        return string.CompareOrdinal(input, target) == 0;
    }

    /// <summary>
    /// Handy method to print arguments to <c>System.Console</c>.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="args">The arguments.</param>
    [DebuggerStepThrough]
    public static void Print(this string input, params object[] args) => Console.WriteLine(input, args);

    /// <summary>
    /// Generates a slug.
    /// <remarks>
    /// Credit goes to <see href="http://stackoverflow.com/questions/2920744/url-slugify-alrogithm-in-cs"/>.
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static string GenerateSlug(this string value, uint? maxLength = null)
    {
        // prepare string, remove diacritics, lower case and convert hyphens to whitespace
        string result = RemoveDiacritics(value).Replace("-", " ").ToLowerInvariant();

        result = Regex.Replace(result, @"[^a-z0-9\s-]", string.Empty); // remove invalid characters
        result = Regex.Replace(result, @"\s+", " ").Trim(); // convert multiple spaces into one space

        if (maxLength.HasValue)
        {
            result = result.Substring(0, result.Length <= maxLength
                ? result.Length : (int)maxLength.Value).Trim();
        }
        return Regex.Replace(result, @"\s", "-");
    }

    /// <summary>
    /// Removes the diacritics from the given <paramref name="input"/> 
    /// </summary>
    /// <remarks>
    /// Credit goes to <see href="http://stackoverflow.com/a/249126"/>.
    /// </remarks>
    [DebuggerStepThrough]
    public static string RemoveDiacritics(this string input)
    {
        string normalizedString = input.Normalize(NormalizationForm.FormD);
        StringBuilder stringBuilder = StringBuilderCache.Acquire();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return StringBuilderCache.GetStringAndRelease(stringBuilder).Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// A method to convert English digits to Persian numbers.
    /// </summary>
    [DebuggerStepThrough]
    public static string ToPersianNumber(this string input) =>
        input
            .Replace("0", "۰")
            .Replace("1", "۱")
            .Replace("2", "۲")
            .Replace("3", "۳")
            .Replace("4", "۴")
            .Replace("5", "۵")
            .Replace("6", "۶")
            .Replace("7", "۷")
            .Replace("8", "۸")
            .Replace("9", "۹");

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="xmlInput">The input containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>
    /// The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this string xmlInput, XName name, bool ignoreCase = true) =>
        xmlInput.GetElements(name, new XmlReaderSettings(), ignoreCase);

    /// <summary>
    /// Gets a sequence containing every element with the name equal to <paramref name="name"/>.
    /// </summary>
    /// <param name="xmlInput">The input containing XML</param>
    /// <param name="name">The name of the elements to return</param>
    /// <param name="settings">The settings used by the <see cref="XmlReader"/></param>
    /// <param name="ignoreCase">The flag indicating whether the name should be looked up in a case sensitive manner</param>
    /// <returns>
    /// The sequence containing all the elements <see cref="XElement"/> matching the <paramref name="name"/>.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<XElement> GetElements(this string xmlInput, XName name, XmlReaderSettings settings, bool ignoreCase = true)
    {
        using StringReader stringReader = new (xmlInput);
        using XmlReader xmlReader = XmlReader.Create(stringReader, settings);

        foreach (var xElement in xmlReader.GetEelements(name, ignoreCase))
        {
            yield return xElement;
        }
    }

    /// <summary>
    /// Compresses the given <paramref name="input"/> to <c>Base64</c> string.
    /// </summary>
    /// <param name="input">The string to be compressed</param>
    /// <returns>The compressed string in <c>Base64</c></returns>
    [DebuggerStepThrough]
    public static string Compress(this string input)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(input);
        using MemoryStream memStream = new();
        using GZipStream zipStream = new(memStream, CompressionMode.Compress, true);

        zipStream.Write(buffer, 0, buffer.Length);
        zipStream.Close();

        memStream.Position = 0;

        byte[] compressedData = new byte[memStream.Length];
        _ = memStream.Read(compressedData, 0, compressedData.Length);

        byte[] gZipBuffer = new byte[compressedData.Length + 4];
        Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
        Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
        
        return Convert.ToBase64String(gZipBuffer);
    }

    /// <summary>
    /// Decompresses a <c>Base64</c> compressed string.
    /// </summary>
    /// <param name="compressedInput">The string compressed in <c>Base64</c></param>
    /// <returns>The uncompressed string</returns>
    [DebuggerStepThrough]
    public static string Decompress(this string compressedInput)
    {
        byte[] gZipBuffer = Convert.FromBase64String(compressedInput);
        using MemoryStream memStream = new();

        int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
        memStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);
        memStream.Position = 0;

        byte[] buffer = new byte[dataLength];
        using GZipStream zipStream = new(memStream, CompressionMode.Decompress);
        _ = zipStream.Read(buffer, 0, buffer.Length);

        return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// Ensures the given <paramref name="fileName"/> can be used as a file name.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsValidFileName(this string fileName) =>
        fileName.IsNotNullOrEmptyOrWhiteSpace() && fileName.IndexOfAny(InvalidFileNameCharacters) == -1;

    /// <summary>
    /// Ensures the given <paramref name="path"/> can be used as a path.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsValidPathName(this string path) =>
        path.IsNotNullOrEmptyOrWhiteSpace() && path.IndexOfAny(InvalidPathCharacters) == -1;

    /// <summary>
    /// Returns a <see cref="Guid"/> from a <c>Base64</c> encoded <paramref name="input"/>.
    /// <example>
    /// DRfscsSQbUu8bXRqAvcWQA== or DRfscsSQbUu8bXRqAvcWQA depending on <paramref name="trimmed"/>.
    /// </example>
    /// <remarks>
    /// See: <see href="https://blog.codinghorror.com/equipping-our-ascii-armor/"/>
    /// </remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static Guid ToGuid(this string input, bool trimmed = true) =>
        trimmed ? new Guid(Convert.FromBase64String(input + "==")) : new Guid(Convert.FromBase64String(input));

    /// <summary>
    /// Returns all the start and end indexes of the occurrences of the 
    /// given <paramref name="startTag"/> and <paramref name="endTag"/> 
    /// in the given <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The input to search.</param>
    /// <param name="startTag">The starting tag e.g. <c>&lt;div></c>.</param>
    /// <param name="endTag">The ending tag e.g. <c>&lt;/div></c>.</param>
    /// <returns>
    /// A sequence <see cref="KeyValuePair{TKey,TValue}"/> where the key is 
    /// the starting position and value is the end position.
    /// </returns>
    [DebuggerStepThrough]
    public static IEnumerable<KeyValuePair<int, int>> GetStartAndEndIndexes(this string input, string startTag, string endTag)
    {
        int startIdx = 0;
        int endIdx;

        while ((startIdx = input.IndexOf(startTag, startIdx, StringComparison.Ordinal)) != -1 && 
               (endIdx = input.IndexOf(endTag, startIdx, StringComparison.Ordinal)) != -1)
        {
            KeyValuePair<int, int> result = new(startIdx, endIdx);
            startIdx = endIdx;
            yield return result;
        }
    }

    /// <summary>
    /// Returns the size of the given <paramref name="input"/> encoded 
    /// as <c>UTF-16</c> characters in bytes.
    /// </summary>
    [DebuggerStepThrough]
    public static int GetSize(this string input) => input.Length * sizeof(char);

    /// <summary>
    /// Obfuscates the given <paramref name="input"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static string Obfuscate(this string input, char fillWith = '*') =>
        string.Create(input.Length, input, (buffer, v) =>
        {
            int prefixLength = input.Length * 25 / 100;
            v.AsSpan(0, prefixLength).CopyTo(buffer);
            buffer[prefixLength..].Fill(fillWith);
        });
}