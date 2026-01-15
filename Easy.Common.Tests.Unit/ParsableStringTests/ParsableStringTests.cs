namespace Easy.Common.Tests.Unit.ParsableStringTests;

using Easy.Common.Interfaces;
using NUnit.Framework;
using Shouldly;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[TestFixture]
public sealed class ParsableStringTests
{
    [Test]
    public void When_parsing_an_enum_to_and_from_string()
    {
        SomeEnum oneDay = SomeEnum.OneDay;
        oneDay.ToString().ShouldBe("1D");

        SomeEnum parsedOneDay = "1D";
        parsedOneDay.ShouldBe(oneDay);
    }
    
    [Test]
    public void When_parsing_an_id_to_and_from_string()
    {
        SomeId id = "ABC|Isin";
        id.Id.ShouldBe("ABC");
        id.Type.ShouldBe(SomeIdType.Isin);

        id.ToString().ShouldBe("ABC|Isin");
    }

    [Test]
    public void When_serializing_an_enum_to_and_from_json()
    {
        SomeEnum oneDay = SomeEnum.OneDay;
        string json = JsonSerializer.Serialize(oneDay);
        json.ShouldBe("\"1D\"");

        SomeEnum deserialized = JsonSerializer.Deserialize<SomeEnum>(json);
        deserialized.ShouldBe(oneDay);
    }

    [Test]
    public void When_serializing_an_id_to_and_from_json()
    {
        SomeId id = "ABC|Isin";
        string json = JsonSerializer.Serialize(id);
        json.ShouldBe("\"ABC|Isin\"");

        SomeId deserialized = JsonSerializer.Deserialize<SomeId>(json);
        deserialized.ShouldBe(id);
    }

    [JsonConverter(typeof(ParsableStringConverter<SomeEnum>))]
    private readonly record struct SomeEnum(string Value) : IParsableString<SomeEnum>
    {
        public static readonly SomeEnum OneDay = new("1D");
        public static readonly SomeEnum OneWeek = new("1W");

        public static implicit operator string(SomeEnum input) => input.Value;
        public static implicit operator SomeEnum(string input) => new(input);

        public override string ToString() => Value;

        public static bool TryParse(string? input, IFormatProvider? provider, out SomeEnum result) =>
            ParsableStringHelper<SomeEnum>.TryParse(input, provider, out result);
    }

    [JsonConverter(typeof(ParsableStringConverter<SomeId>))]
    private readonly record struct SomeId(string Id, SomeIdType Type) : IParsableString<SomeId>
    {
        private const char Separator = '|';

        public bool IsDefault => string.IsNullOrWhiteSpace(Id);

        public static implicit operator string(SomeId id) => id.ToString();
        public static implicit operator SomeId(string id) => Parse(id);

        public override string ToString() => string.Concat(Id, Separator, Type);

        public override int GetHashCode() => HashCode.Combine(Id, Type);

        public static bool TryParse(string? input, IFormatProvider? provider, out SomeId result)
        {
            result = default;

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            ReadOnlySpan<char> inputSpan = input.AsSpan();
            int lastSeparatorIndex = inputSpan.LastIndexOf(Separator);
            if (lastSeparatorIndex == -1)
            {
                return false;
            }

            ReadOnlySpan<char> idSpan = inputSpan.Slice(0, lastSeparatorIndex);
            ReadOnlySpan<char> typeSpan = inputSpan.Slice(lastSeparatorIndex + 1);

            if (!Enum.TryParse(typeSpan, ignoreCase: true, out SomeIdType idType))
            {
                return false;
            }

            result = new SomeId(idSpan.ToString(), idType);
            return true;
        }

        private static SomeId Parse(string input) => TryParse(input, null, out SomeId result)
            ? result
            : throw new FormatException($"Unable to parse as: {typeof(SomeId).FullName}. Value: {input}");
    }

    private enum SomeIdType : sbyte
    {
        Isin,
        Cusip,
        Ric
    }
}