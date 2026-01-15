using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Easy.Common;

using Easy.Common.Interfaces;

public sealed class ParsableStringConverter<T> : JsonConverter<T> where T : struct, IParsable<T>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        return s is not null && T.TryParse(s, null, out T result)
            ? result
            : throw new JsonException($"Invalid value for {typeof(T).Name}.");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString());

    public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string? s = reader.GetString();
        return s is not null && T.TryParse(s, null, out T result)
            ? result
            : throw new JsonException($"Invalid property name for {typeof(T).Name}.");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WritePropertyName(value.ToString() ?? string.Empty);
}

public static class ParsableStringHelper<T> where T : struct, IParsableString<T>
{
    private static readonly Func<string, T> CachedFactory = CreateFactory();

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