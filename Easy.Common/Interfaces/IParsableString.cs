using System;
using System.Diagnostics.CodeAnalysis;

namespace Easy.Common.Interfaces;

public interface IParsableString<T> : IParsable<T> where T : struct, IParsableString<T>
{
    static T IParsable<T>.Parse(string input, IFormatProvider? provider) => ParsableStringHelper<T>.Parse(input, provider);

    static bool IParsable<T>.TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out T result) =>
        ParsableStringHelper<T>.TryParse(input, provider, out result);
}