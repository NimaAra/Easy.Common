namespace Easy.Common.Tests.Unit.JsonHelper;

using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using JsonHelper = Easy.Common.JsonHelper;

[TestFixture]
internal sealed class JsonHelperTests
{
    [Test]
    public void GivenMinifiedJson_WhenPrettifying_ThenShouldSucceed()
    {
        const string JSON = """{"name": "Foo", "age": 42}""";

        JsonHelper.Prettify(JSON)
            .ShouldBe(
                """
                {
                  "name": "Foo",
                  "age": 42
                }
                """);
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

        var result = JsonHelper.DeserializeAs(template, JSON);

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

        var result = JsonHelper.DeserializeAs(template, JSON, options);

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

        var result = JsonHelper.DeserializeAs(template, ms);

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

        var result = JsonHelper.DeserializeAs(template, jsonSpan);

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

        var result = JsonHelper.DeserializeAs(template, jsonSpan);

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

        var result = JsonHelper.DeserializeAs(template, jsonMemory);

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

        var result = JsonHelper.DeserializeAs(template, jsonMemory);

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
        await JsonHelper.SerializeAndCompress(ms, payload);
        
        ms.Length.ShouldBe(41);

        var template = payload;

        ms.Position = 0;
        var deserialized = await JsonHelper.DeserializeFromCompressed(ms, template);
        
        deserialized.ShouldNotBeNull();
        deserialized.name.ShouldBe(payload.name);
        deserialized.age.ShouldBe(payload.age);
    }
}