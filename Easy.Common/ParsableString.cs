using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Easy.Common;

using Easy.Common.Interfaces;

/// <summary>
/// Provides a JSON converter that serializes and deserializes value types implementing <see cref="IParsable{T}"/> using
/// their string representation.
/// </summary>
/// <remarks>
/// This converter enables support for types that can be parsed from and formatted to strings such as
/// structs implementing <see cref="IParsable{T}"/>.
/// When deserializing, the converter uses <see cref="IParsable{T}.TryParse"/> to convert JSON string values to the
/// target type. If parsing fails, a <see cref="JsonException"/> is thrown. This converter is useful for scenarios where
/// value types are represented as strings in JSON payloads, including property names.
/// </remarks>
public sealed class ParsableStringConverter<T> : JsonConverter<T> where T : struct, IParsable<T>
{
    /// <summary>
    /// Reads a value of type <typeparamref name="T"/> from the provided JSON reader using its string representation.
    /// </summary>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        return s is not null && T.TryParse(s, null, out T result)
            ? result
            : throw new JsonException($"Invalid value for {typeof(T).Name}.");
    }

    /// <summary>
    /// Writes the specified value as a JSON string using the provided writer and serialization options.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());

    /// <summary>
    /// Reads a JSON property name and converts it to an instance of type <typeparamref name="T"/>.
    /// </summary>
    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        return s is not null && T.TryParse(s, null, out T result)
            ? result
            : throw new JsonException($"Invalid property name for {typeof(T).Name}.");
    }

    /// <summary>
    /// Writes the specified value as a JSON property name using the provided writer.
    /// </summary>
    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WritePropertyName(value.ToString() ?? string.Empty);
}

/// <summary>
/// Provides helper methods for parsing strings into value types that implement the IParsableString{T} interface.
/// </summary>
public static class ParsableStringHelper<T> where T : struct, IParsableString<T>
{
    private static readonly Func<string, T> CachedFactory = CreateFactory();

    /// <summary>
    /// Attempts to parse the specified input string and returns a value that indicates whether the operation succeeded.
    /// </summary>
    public static bool TryParse([NotNullWhen(true)] string? input, IFormatProvider? provider, out T result)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = default;
            return false;
        }

        try
        {
            result = CachedFactory(input);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    internal static T Parse(string input, IFormatProvider? provider = null) => TryParse(input, provider, out T result)
        ? result
        : throw new FormatException($"Unable to parse as: {typeof(T).FullName}. Value: {input}");

    private static Func<string, T> CreateFactory()
    {
        ConstructorInfo ctor = typeof(T).GetConstructor([typeof(string)]) ??
                               throw new InvalidOperationException($"Type {typeof(T)} must have a constructor that takes a single string parameter.");

        ParameterExpression param = Expression.Parameter(typeof(string), "value");
        NewExpression newExpr = Expression.New(ctor, param);
        Expression<Func<string, T>> lambda = Expression.Lambda<Func<string, T>>(newExpr, param);
        return lambda.Compile();
    }
}