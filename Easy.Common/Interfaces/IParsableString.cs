using System;
using System.Diagnostics.CodeAnalysis;

namespace Easy.Common.Interfaces;

/// <summary>
/// Specifies the contract for types that can be parsed from a <see cref="string"/> representation.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IParsableString<T> : IParsable<T> where T : struct, IParsableString<T>
{
    /// <summary>
    /// Parses the given <see cref="string"/> representation to an instance of <typeparamref name="T"/>.
    /// </summary>
    static T IParsable<T>.Parse(string input, IFormatProvider? provider) => ParsableStringHelper<T>.Parse(input, provider);

    /// <summary>
    /// Attempts to parse the given <see cref="string"/> representation to an instance of <typeparamref name="T"/>.
    /// </summary>
    static bool IParsable<T>.TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out T result) =>
        ParsableStringHelper<T>.TryParse(input, provider, out result);
}