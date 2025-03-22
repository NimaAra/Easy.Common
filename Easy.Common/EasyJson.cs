namespace Easy.Common;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Provides a set of methods to make working with JSON easier.
/// </summary>
public static class EasyJson
{
    private static readonly JsonSerializerOptions _prettyOptions = new() { WriteIndented = true };

    /// <summary>
    /// Indicates whether un-configured <see cref="JsonSerializerOptions"/> instances
    /// should be set to use the reflection-based <see cref="DefaultJsonTypeInfoResolver"/>.
    /// </summary>
    public static bool IsReflectionEnabledByDefault => JsonSerializer.IsReflectionEnabledByDefault;

    /// <summary>
    /// Writes one JSON value (including objects or arrays) to the provided writer.
    /// </summary>
    public static void Serialize<TValue>(Utf8JsonWriter writer, TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(writer, value, options);

    /// <summary>
    /// Writes one JSON value (including objects or arrays) to the provided writer.
    /// </summary>
    public static void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(writer, value, inputType, options);

    /// <summary>
    /// Writes one JSON value (including objects or arrays) to the provided writer.
    /// </summary>
    public static void Serialize<TValue>(Utf8JsonWriter writer, TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Serialize(writer, value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="string"/>.
    /// </summary>
    public static string Serialize<TValue>(TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(value, options);

    /// <summary>
    /// Writes one JSON value (including objects or arrays) to the provided writer.
    /// </summary>
    public static void Serialize(Utf8JsonWriter writer, object? value, JsonTypeInfo jsonTypeInfo) =>
        JsonSerializer.Serialize(writer, value, jsonTypeInfo);

    /// <summary>
    /// Writes one JSON value (including objects or arrays) to the provided writer.
    /// </summary>
    public static void Serialize(Utf8JsonWriter writer, object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.Serialize(writer, value, inputType, context);

    /// <summary>
    /// Converts the provided value into a <see cref="byte"/> array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes<TValue>(TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToUtf8Bytes(value, options);

    /// <summary>
    /// Converts the provided value into a <see cref="byte"/> array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes(object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToUtf8Bytes(value, inputType, options);
    
    /// <summary>
    /// Converts the provided value into a <see cref="byte"/> array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.SerializeToUtf8Bytes(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="byte"/> array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes(object? value, JsonTypeInfo jsonTypeInfo) =>
        JsonSerializer.SerializeToUtf8Bytes(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="byte"/> array.
    /// </summary>
    public static byte[] SerializeToUtf8Bytes(object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.SerializeToUtf8Bytes(value, inputType, context);

    /// <summary>
    /// Converts the provided value into a <see cref="string"/>.
    /// </summary>
    public static string Serialize(object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(value, inputType, options);

    /// <summary>
    /// Converts the provided value into a <see cref="string"/>.
    /// </summary>
    public static string Serialize<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Serialize(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="string"/>.
    /// </summary>
    public static string Serialize(object? value, JsonTypeInfo jsonTypeInfo) =>
        JsonSerializer.Serialize(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="string"/>.
    /// </summary>
    public static string Serialize(object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.Serialize(value, inputType, context);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    public static Task SerializeAsync<TValue>(
        Stream utf8Json, TValue value, JsonSerializerOptions? options = null, CancellationToken cToken = default) =>
            JsonSerializer.SerializeAsync(utf8Json, value, options, cToken);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    public static void Serialize<TValue>(Stream utf8Json, TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(utf8Json, value, options);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static Task SerializeAsync(
        Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null, CancellationToken cToken = default) =>
            JsonSerializer.SerializeAsync(utf8Json, value, inputType, options, cToken);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static void Serialize(Stream utf8Json, object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(utf8Json, value, inputType, options);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    public static Task SerializeAsync<TValue>(
        Stream utf8Json, TValue value, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cancellationToken = default) =>
            JsonSerializer.SerializeAsync(utf8Json, value, jsonTypeInfo, cancellationToken);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to serialize.</typeparam>
    public static void Serialize<TValue>(Stream utf8Json, TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.SerializeAsync(utf8Json, value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static Task SerializeAsync(Stream utf8Json, object? value, JsonTypeInfo<object?> jsonTypeInfo, CancellationToken cToken = default) =>
        JsonSerializer.SerializeAsync(utf8Json, value, jsonTypeInfo, cToken);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static void Serialize(Stream utf8Json, object? value, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.Serialize(utf8Json, value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static Task SerializeAsync(
        Stream utf8Json, object? value, Type inputType, JsonSerializerContext context, CancellationToken cToken = default) =>
            JsonSerializer.SerializeAsync(utf8Json, value, inputType, context, cToken);

    /// <summary>
    /// Converts the provided value to UTF-8 encoded JSON text and write it to the <see cref="System.IO.Stream"/>.
    /// </summary>
    public static void Serialize(Stream utf8Json, object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.Serialize(utf8Json, value, inputType, context);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonDocument"/>.
    /// </summary>
    public static JsonDocument SerializeToDocument<TValue>(TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToDocument(value, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonDocument"/>.
    /// </summary>
    public static JsonDocument SerializeToDocument(object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToDocument(value, inputType, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonDocument"/>.
    /// </summary>
    public static JsonDocument SerializeToDocument<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.SerializeToDocument(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonDocument"/>.
    /// </summary>
    public static JsonDocument SerializeToDocument(object? value, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.SerializeToDocument(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonDocument"/>.
    /// </summary>
    public static JsonDocument SerializeToDocument(object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.SerializeToDocument(value, inputType, context);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonElement"/>.
    /// </summary>
    public static JsonElement SerializeToElement<TValue>(TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToElement(value, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonElement"/>.
    /// </summary>
    public static JsonElement SerializeToElement(object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToElement(value, inputType, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonElement"/>.
    /// </summary>
    public static JsonElement SerializeToElement<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.SerializeToElement(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonElement"/>.
    /// </summary>
    public static JsonElement SerializeToElement(object? value, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.SerializeToElement(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonElement"/>.
    /// </summary>
    public static JsonElement SerializeToElement(object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.SerializeToElement(value, inputType, context);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? SerializeToNode<TValue>(TValue value, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToNode(value, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? SerializeToNode(object? value, Type inputType, JsonSerializerOptions? options = null) =>
        JsonSerializer.SerializeToNode(value, inputType, options);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? SerializeToNode<TValue>(TValue value, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.SerializeToNode(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? SerializeToNode(object? value, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.SerializeToNode(value, jsonTypeInfo);

    /// <summary>
    /// Converts the provided value into a <see cref="JsonNode"/>.
    /// </summary>
    public static JsonNode? SerializeToNode(object? value, Type inputType, JsonSerializerContext context) =>
        JsonSerializer.SerializeToNode(value, inputType, context);

    /// <summary>
    /// Parses the text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(string json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<TValue>(json, options);

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of the type specified by a generic type parameter.
    /// </summary>
    public static TValue? Deserialize<TValue>(ReadOnlySpan<char> json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<TValue>(json, options);

    /// <summary>
    /// Parses the text representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(string json, Type returnType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize(json, returnType, options);

    /// <summary>
    /// Parses the text representing a single JSON value into an instance of a specified type.
    /// </summary>
    public static object? Deserialize(ReadOnlySpan<char> json, Type returnType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize(json, returnType, options);

    /// <summary>
    /// Parses the text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(string json, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Deserialize(json, jsonTypeInfo);

    /// <summary>
    /// Parses the text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(ReadOnlySpan<char> json, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Deserialize(json, jsonTypeInfo);

    /// <summary>
    /// Parses the text representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(string json, Type returnType, JsonSerializerContext context) =>
        JsonSerializer.Deserialize(json, returnType, context);

    /// <summary>
    /// Parses the text representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(ReadOnlySpan<char> json, Type returnType, JsonSerializerContext context) =>
        JsonSerializer.Deserialize(json, returnType, context);

    /// <summary>
    /// Reads one JSON value (including objects or arrays) from the provided reader into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(ref Utf8JsonReader reader, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<TValue>(ref reader, options);

    /// <summary>
    /// Reads one JSON value (including objects or arrays) from the provided reader into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(ref Utf8JsonReader reader, Type returnType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize(ref reader, returnType, options);

    /// <summary>
    /// Reads one JSON value (including objects or arrays) from the provided reader into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(ref Utf8JsonReader reader, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Deserialize(ref reader, jsonTypeInfo);

    /// <summary>
    /// Reads one JSON value (including objects or arrays) from the provided reader into an instance specified by the <paramref name="jsonTypeInfo"/>.
    /// </summary>
    public static object? Deserialize(ref Utf8JsonReader reader, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.Deserialize(ref reader, jsonTypeInfo);

    /// <summary>
    /// Reads one JSON value (including objects or arrays) from the provided reader into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(ref Utf8JsonReader reader, Type returnType, JsonSerializerContext context) =>
        JsonSerializer.Deserialize(ref reader, returnType, context);

    /// <summary>
    /// Converts the <see cref="JsonElement"/> representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(JsonElement element, JsonSerializerOptions? options = null) =>
        element.Deserialize<TValue>(options);

    /// <summary>
    /// Converts the <see cref="JsonElement"/> representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(JsonElement element, Type returnType, JsonSerializerOptions? options = null) =>
        element.Deserialize(returnType, options);

    /// <summary>
    /// Converts the <see cref="JsonElement"/> representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(JsonElement element, JsonTypeInfo<TValue> jsonTypeInfo) =>
        element.Deserialize(jsonTypeInfo);

    /// <summary>
    /// Converts the <see cref="JsonElement"/> representing a single JSON value into an instance specified by the <paramref name="jsonTypeInfo"/>.
    /// </summary>
    public static object? Deserialize(JsonElement element, JsonTypeInfo<object?> jsonTypeInfo) =>
        element.Deserialize(jsonTypeInfo);

    /// <summary>
    /// Converts the <see cref="JsonElement"/> representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(JsonElement element, Type returnType, JsonSerializerContext context) =>
        element.Deserialize(returnType, context);

    /// <summary>
    /// Parses the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(ReadOnlySpan<byte> utf8Json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<TValue>(utf8Json, options);

    /// <summary>
    /// Parses the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize(utf8Json, returnType, options);

    /// <summary>
    /// Parses the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// </summary>
    public static TValue? Deserialize<TValue>(ReadOnlySpan<byte> utf8Json, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Deserialize(utf8Json, jsonTypeInfo);

    /// <summary>
    /// Parses the UTF-8 encoded text representing a single JSON value into an instance specified by the <paramref name="jsonTypeInfo"/>.
    /// </summary>
    public static object? Deserialize(ReadOnlySpan<byte> utf8Json, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.Deserialize(utf8Json, jsonTypeInfo);

    /// <summary>
    /// Parses the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// </summary>
    public static object? Deserialize(ReadOnlySpan<byte> utf8Json, Type returnType, JsonSerializerContext context) =>
        JsonSerializer.Deserialize(utf8Json, returnType, context);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static ValueTask<TValue?> DeserializeAsync<TValue>(
        Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cToken = default) =>
            JsonSerializer.DeserializeAsync<TValue>(utf8Json, options, cToken);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static TValue? Deserialize<TValue>(Stream utf8Json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<TValue>(utf8Json, options);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static ValueTask<object?> DeserializeAsync(
        Stream utf8Json, Type returnType, JsonSerializerOptions? options = null, CancellationToken cToken = default) =>
            JsonSerializer.DeserializeAsync(utf8Json, returnType, options, cToken);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static object? Deserialize(Stream utf8Json, Type returnType, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize(utf8Json, returnType, options);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static ValueTask<TValue?> DeserializeAsync<TValue>(
        Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo, CancellationToken cToken = default) =>
            JsonSerializer.DeserializeAsync(utf8Json, jsonTypeInfo, cToken);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into an instance specified by the <paramref name="jsonTypeInfo"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static ValueTask<object?> DeserializeAsync(Stream utf8Json, JsonTypeInfo<object?> jsonTypeInfo, CancellationToken cToken = default) =>
        JsonSerializer.DeserializeAsync(utf8Json, jsonTypeInfo, cToken);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <typeparamref name="TValue"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static TValue? Deserialize<TValue>(Stream utf8Json, JsonTypeInfo<TValue> jsonTypeInfo) =>
        JsonSerializer.Deserialize(utf8Json, jsonTypeInfo);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into an instance specified by the <paramref name="jsonTypeInfo"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static object? Deserialize(Stream utf8Json, JsonTypeInfo<object?> jsonTypeInfo) =>
        JsonSerializer.Deserialize(utf8Json, jsonTypeInfo);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static ValueTask<object?> DeserializeAsync(
        Stream utf8Json, Type returnType, JsonSerializerContext context, CancellationToken cToken = default) =>
            JsonSerializer.DeserializeAsync(utf8Json, returnType, context, cToken);

    /// <summary>
    /// Reads the UTF-8 encoded text representing a single JSON value into a <paramref name="returnType"/>.
    /// The Stream will be read to completion.
    /// </summary>
    public static object? Deserialize(Stream utf8Json, Type returnType, JsonSerializerContext context) =>
        JsonSerializer.Deserialize(utf8Json, returnType, context);

    /// <summary>
    /// Wraps the UTF-8 encoded text into an <see cref="IAsyncEnumerable{TValue}" />
    /// that can be used to deserialize root-level JSON arrays in a streaming manner.
    /// </summary>
    public static IAsyncEnumerable<TValue?> DeserializeAsyncEnumerable<TValue>(
        Stream utf8Json, JsonSerializerOptions? options = null, CancellationToken cToken = default) =>
            JsonSerializer.DeserializeAsyncEnumerable<TValue>(utf8Json, options, cToken);

    /// <summary>
    /// Deserializes payload from the given <paramref name="jsonElement"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, JsonElement jsonElement, JsonSerializerOptions? options = null) =>
        jsonElement.Deserialize<T>(options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, string json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, Stream json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, ReadOnlySpan<char> json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, ReadOnlySpan<byte> json, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(json, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="reader"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, ref Utf8JsonReader reader, JsonSerializerOptions? options = null) =>
        JsonSerializer.Deserialize<T>(ref reader, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, ReadOnlyMemory<byte> json, JsonSerializerOptions? options = null) =>
        DeserializeAs(template, json.Span, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static T? DeserializeAs<T>(T template, ReadOnlyMemory<char> json, JsonSerializerOptions? options = null) =>
        DeserializeAs(template, json.Span, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> based on the given <paramref name="template"/>.
    /// </summary>
    public static Task<T?> DeserializeFromCompressedAs<T>(Stream json, T template, JsonSerializerOptions? options = null) =>
        DeserializeFromCompressed<T>(json, options);

    /// <summary>
    /// Deserializes payload from the given <paramref name="json"/> as <typeparamref name="T"/>.
    /// </summary>
    public static async Task<T?> DeserializeFromCompressed<T>(Stream json, JsonSerializerOptions? options = null)
    {
        await using GZipStream decompressor = new GZipStream(json, CompressionMode.Decompress, leaveOpen: true);
        return await JsonSerializer.DeserializeAsync<T>(decompressor, options);
    }

    /// <summary>
    /// Serializes the given <paramref name="payload"/> then compresses the result and stores it into <paramref name="target"/>.
    /// </summary>
    public static async Task SerializeAndCompress<T>(
        Stream target, T payload, JsonSerializerOptions? options = null, CompressionLevel level = CompressionLevel.Optimal)
    {
        await using GZipStream compressor = new(target, level, true);
        await JsonSerializer.SerializeAsync(compressor, payload, options);
    }

    /// <summary>
    /// Prettifies the given <paramref name="json"/>
    /// </summary>
    public static string Prettify(string json)
    {
        object? asObj = JsonSerializer.Deserialize<object>(json);
        return JsonSerializer.Serialize(asObj, _prettyOptions);
    }
}