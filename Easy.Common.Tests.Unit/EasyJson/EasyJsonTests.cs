namespace Easy.Common.Tests.Unit.EasyJson;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using EasyJson = Easy.Common.EasyJson;

[TestFixture]
internal sealed class EasyJsonTests
{
    [Test]
    public void GivenMinifiedJson_WhenPrettifying_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";

        string prettified = EasyJson.Prettify(JSON);

        prettified.ShouldContain(Environment.NewLine);

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var deserialized = EasyJson.DeserializeAs(template, prettified);

        deserialized.name.ShouldBe("Foo");
        deserialized.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, JSON);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingWithTemplateAndDifferentCasing_ThenShouldSucceed()
    {
        const string JSON = """{"Name": "Foo", "AgE": 42}""";

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };

        var result = EasyJson.DeserializeAs(template, JSON, options);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromStreamWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        using MemoryStream ms = new(Encoding.UTF8.GetBytes(JSON));

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, ms);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromReadOnlySpanCharWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        ReadOnlySpan<char> jsonSpan = JSON.AsSpan();

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, jsonSpan);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromReadOnlySpanByteWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        ReadOnlySpan<byte> jsonSpan = Encoding.UTF8.GetBytes(JSON).AsSpan();

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, jsonSpan);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromReadOnlyMemoryCharWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        ReadOnlyMemory<char> jsonMemory = JSON.AsMemory();

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, jsonMemory);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromReadOnlyMemoryByteWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        ReadOnlyMemory<byte> jsonMemory = Encoding.UTF8.GetBytes(JSON).AsMemory();

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, jsonMemory);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromJsonElementWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";

        JsonElement jsonElement = JsonSerializer.Deserialize<JsonElement>(JSON);

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, jsonElement);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public void GivenJson_WhenDeserializingFromUtf8JsonReaderWithTemplate_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";
        ReadOnlySpan<byte> jsonSpan = Encoding.UTF8.GetBytes(JSON).AsSpan();
        
        Utf8JsonReader reader = new Utf8JsonReader(jsonSpan);

        var template = new
        {
            name = string.Empty,
            age = 0
        };

        var result = EasyJson.DeserializeAs(template, ref reader);

        result.ShouldNotBeNull();
        result.name.ShouldBe("Foo");
        result.age.ShouldBe(42);
    }

    [Test]
    public async Task GivenJson_WhenSerializingAndCompressingAndDeserializingFromCompressed_ThenShouldSucceed()
    {
        var payload = new
        {
            name = "Foo",
            age = 42
        };

        using MemoryStream ms = new();
        await EasyJson.SerializeAndCompress(ms, payload);
        
        ms.Length.ShouldBe(41);

        var template = payload;

        ms.Position = 0;
        var deserialized = await EasyJson.DeserializeFromCompressedAs(ms, template);
        
        deserialized.ShouldNotBeNull();
        deserialized.name.ShouldBe(payload.name);
        deserialized.age.ShouldBe(payload.age);
    }
}