namespace Easy.Common.Tests.Unit.CountryCodes;

using System;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class CountryCodeMappingTests
{
    [TestCase("aFg", "Afghanistan")]
    [TestCase("Irn", "Iran")]
    [TestCase("zwE", "Zimbabwe")]
    [TestCase("Gbr", "United Kingdom")]
    [TestCase("USa", "United States")]
    [TestCase("tUR", "Turkey")]
    public void When_getting_country_name_for_a_valid_country_code(string code, string expectedName)
    {
        string countryName;

        CountryCodesMapping.TryGetCountryName(code, out countryName)
            .ShouldBeTrue();
        countryName.ShouldBe(expectedName);

        CountryCodesMapping.TryGetCountryName(code.ToLower(), out countryName)
            .ShouldBeTrue();
        countryName.ShouldBe(expectedName);

        CountryCodesMapping.TryGetCountryName(code.ToUpper(), out countryName)
            .ShouldBeTrue();
        countryName.ShouldBe(expectedName);
    }

    [TestCase("AaFg")]
    [TestCase("Irn ")]
    [TestCase("xxx")]
    [TestCase("foo")]
    [TestCase("bar")]
    public void When_getting_country_name_for_an_invalid_country_code(string code)
    {
        string countryName;

        CountryCodesMapping.TryGetCountryName(code, out countryName)
            .ShouldBeFalse();
        countryName.ShouldBeNull();
    }

    [Test]
    public void When_getting_country_name_for_a_null_country_code()
    {
        string countryCode;

        Should
            .Throw<ArgumentException>(() => CountryCodesMapping.TryGetCountryName(null, out countryCode))
            .Message.ShouldBe("String must not be null, empty or whitespace.");
    }

    [Test]
    public void When_getting_country_name_for_an_empty_country_code()
    {
        string someEmptyCode = string.Empty;
        string countryCode;

        Should
            .Throw<ArgumentException>(() => CountryCodesMapping.TryGetCountryName(someEmptyCode, out countryCode))
            .Message.ShouldBe("String must not be null, empty or whitespace.");
    }

    [Test]
    public void When_getting_country_name_for_a_white_space_country_code()
    {
        string someWhiteSpaceCode = " ";
        string countryCode;

        Should
            .Throw<ArgumentException>(() => CountryCodesMapping.TryGetCountryName(someWhiteSpaceCode, out countryCode))
            .Message.ShouldBe("String must not be null, empty or whitespace.");
    }

    [Test]
    public void When_getting_copies_of_the_country_code_mappings()
    {
        var copyOne = CountryCodesMapping.Mappings;
        var copyTwo = CountryCodesMapping.Mappings;

        copyOne.ShouldNotBeSameAs(copyTwo);

        copyOne.Count.ShouldBe(126);

        copyOne.Remove("afg").ShouldBeTrue();
        copyOne.ContainsKey("afg").ShouldBeFalse();

        copyTwo.ContainsKey("afg").ShouldBeTrue();

        string name;
        CountryCodesMapping.TryGetCountryName("afg", out name).ShouldBeTrue();
        name.ShouldBe("Afghanistan");
    }
}