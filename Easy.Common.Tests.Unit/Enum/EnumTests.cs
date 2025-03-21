namespace Easy.Common.Tests.Unit.Enum;

using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using EasyJson = Easy.Common.EasyJson;

[TestFixture]
internal sealed class EnumTests
{
    [Test]
    public void When_creating_an_enum()
    {
        MyEnum.OptionA.ShouldBe(MyEnum.OptionA);

        MyEnum.OptionA.ShouldNotBe(MyEnum.OptionB);

        IEnum theEnum = MyEnum.OptionA;

        theEnum.ShouldNotBeNull();

        theEnum.Id.ShouldBe((uint)0);
        theEnum.Name.ShouldBe("OptionA");
    }

    [Test]
    public void When_creating_an_explicit_enum()
    {
        MyOtherEnum.OptionA.ShouldBe(MyOtherEnum.OptionA);

        MyOtherEnum.OptionA.ShouldNotBe(MyOtherEnum.OptionB);

        IEnum<string> theEnum = MyOtherEnum.OptionA;

        theEnum.ShouldNotBeNull();

        theEnum.Id.ShouldBe("OptionA");
    }

    [Test]
    public void When_getting_enum_values()
    {
        IReadOnlyList<MyEnum> values = MyEnum.Values();
        values.ShouldNotBeEmpty();
        values.ShouldBe([MyEnum.OptionA, MyEnum.OptionB], ignoreOrder: true);
    }

    [Test]
    public void When_getting_explicit_enum_values()
    {
        IReadOnlyList<MyOtherEnum> values = MyOtherEnum.Values();
        values.ShouldNotBeEmpty();
        values.ShouldBe([MyOtherEnum.OptionA, MyOtherEnum.OptionB], ignoreOrder: true);
    }

    [Test]
    public void When_calling_toString()
    {
        MyEnum.OptionB.ToString().ShouldBe("[1] OptionB");
    }

    [Test]
    public void When_calling_toString_explicit()
    {
        MyOtherEnum.OptionB.ToString().ShouldBe("[OptionB]");
    }

    [Test]
    public void When_serializing()
    {
        MyEnum[] enums = [MyEnum.OptionA, MyEnum.OptionB];

        JsonSerializerOptions options = new()
        {
            WriteIndented = false,
            Converters = { new MyEnumConverter() }
        };
        string json = JsonSerializer.Serialize(enums, options);
        json.ShouldBe("""[{"Age":12,"Name":"OptionA","Id":0},{"Age":42,"Name":"OptionB","Id":1}]""");
    }

    [Test]
    public void When_serializing_explicit()
    {
        MyOtherEnum[] enums = [MyOtherEnum.OptionA, MyOtherEnum.OptionB];

        JsonSerializerOptions options = new()
        {
            WriteIndented = false,
            Converters = { new MyOtherEnumConverter() }
        };
        string json = JsonSerializer.Serialize(enums, options);
        json.ShouldBe("""["OptionA","OptionB"]""");
    }

    [Test]
    public void When_deserializing()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Converters = { new MyEnumConverter() }
        };
        string json = """
[
  {
    "Age": 12,
    "Name": "OptionA",
    "Id": 0
  },
  {
    "Age": 42,
    "Name": "OptionB",
    "Id": 1
  }
]
""";
        MyEnum[] enums = JsonSerializer.Deserialize<MyEnum[]>(json, options);
        enums.ShouldNotBeNull();
        enums.ShouldBe([MyEnum.OptionA, MyEnum.OptionB]);
    }

    [Test]
    public void When_deserializing_explicit()
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = false,
            Converters = { new MyOtherEnumConverter() }
        };
        string json = """["OptionA","OptionB"]""";
        MyOtherEnum[] enums = JsonSerializer.Deserialize<MyOtherEnum[]>(json, options);
        enums.ShouldNotBeNull();
        enums.ShouldBe([MyOtherEnum.OptionA, MyOtherEnum.OptionB]);
    }

    sealed record class MyEnum : Enum<MyEnum>
    {
        public static readonly MyEnum OptionA = new() { Age = 12 };
        public static readonly MyEnum OptionB = new() { Age = 42 };

        public MyEnum([CallerMemberName] string name = null!) : base(name!)
        {
            SomeReadOnlyProperty = 42L;
        }

        public required uint Age { get; init; }

        public long SomeReadOnlyProperty { get; }

        public override string ToString() => $"[{Id}] {Name}";
    }

    sealed record class MyOtherEnum : Enum<MyOtherEnum, string>
    {
        public static readonly MyOtherEnum OptionA = new() { Age = 12 };
        public static readonly MyOtherEnum OptionB = new() { Age = 42 };

        private MyOtherEnum([CallerMemberName] string id = null!) : base(id)
        {
        }

        public required uint Age { get; init; }

        public long SomeReadOnlyProperty { get; }

        public override string ToString() => $"[{Id}]";
    }

    sealed class MyEnumConverter : JsonConverter<MyEnum>
    {
        private JsonSerializerOptions? _options;

        public override MyEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var template = new { Id = 0u, Name = string.Empty, Age = 0u };

            var tmp = EasyJson.DeserializeAs(template, ref reader, options);

            ArgumentNullException.ThrowIfNull(tmp);

            return new MyEnum(tmp.Name) { Id = tmp.Id, Age = tmp.Age };
        }

        public override void Write(Utf8JsonWriter writer, MyEnum value, JsonSerializerOptions options)
        {
            if (_options is null)
            {
                _options = new(options)
                {
                    IgnoreReadOnlyFields = true,
                    IgnoreReadOnlyProperties = true
                };
                _options.Converters.Remove(this);
            }

            JsonSerializer.Serialize(writer, value, _options);
        }
    }

    sealed class MyOtherEnumConverter : JsonConverter<MyOtherEnum>
    {
        public override MyOtherEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string idValue = reader.GetString();

            if (idValue == MyOtherEnum.OptionA.Id)
            {
                return MyOtherEnum.OptionA;
            }

            if (idValue == MyOtherEnum.OptionB.Id)
            {
                return MyOtherEnum.OptionB;
            }

            throw new ArgumentOutOfRangeException($"Invalid value: {idValue}");
        }

        public override void Write(Utf8JsonWriter writer, MyOtherEnum value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.Id);
    }
}