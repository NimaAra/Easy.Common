#pragma warning disable IDE0060

namespace Easy.Common;

using System;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Threading.Tasks;

/// <summary>
/// Provides a set of methods to make working with JSON easier.
/// </summary>
public static class JsonHelper
{
    private static readonly JsonSerializerOptions _prettyOptions = new() { WriteIndented = true };

    /// <summary>
    /// Prettifies the given <paramref name="json"/>
    /// </summary>
    public static string Prettify(string json)
    {
        object? asObj = JsonSerializer.Deserialize<object>(json);
        return JsonSerializer.Serialize(asObj, _prettyOptions);
    }

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
    public static async Task<T?> DeserializeFromCompressed<T>(Stream json, T template, JsonSerializerOptions? options = default)
    {
        await using GZipStream decompressor = new(json, CompressionMode.Decompress);
        using MemoryStream decompressed = new();
        await decompressor.CopyToAsync(decompressed);
        decompressed.Position = 0;

        return await JsonSerializer.DeserializeAsync<T>(decompressed, options);
    }

    /// <summary>
    /// Serializes the given <paramref name="payload"/> then compresses the result and stores it into <paramref name="target"/>.
    /// </summary>
    public static async Task SerializeAndCompress<T>(
        Stream target, T payload, JsonSerializerOptions? options = default, CompressionLevel level = CompressionLevel.Optimal)
    {
        await using GZipStream compressor = new(target, level, true);
        await JsonSerializer.SerializeAsync(compressor, payload, options);

        compressor.Close();
    }
}