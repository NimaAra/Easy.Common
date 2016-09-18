namespace Easy.Common.Tests.Unit.String
{
    using System;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public sealed class StringExtensionsTests
    {
        [TestCase("abc", false)]
        [TestCase("_", false)]
        [TestCase(" ", false)]
        [TestCase("", true)]
        [TestCase(null, true)]
        public void When_checking_a_string_is_null_or_empty(string input, bool result)
        {
            input.IsNullOrEmpty().ShouldBe(result);
        }

        [TestCase("abc", true)]
        [TestCase("_", true)]
        [TestCase(" ", true)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void When_checking_a_string_is_not_null_or_empty(string input, bool result)
        {
            EnumerableExtensions.IsNotNullOrEmpty(input).ShouldBe(result);
        }

        [TestCase("abc", false)]
        [TestCase("abc ", false)]
        [TestCase(" abc", false)]
        [TestCase(" abc ", false)]
        [TestCase("_", false)]
        [TestCase(" ", true)]
        [TestCase("", true)]
        [TestCase(null, true)]
        public void When_checking_a_string_is_null_or_empty_or_white_space(string input, bool result)
        {
            input.IsNullOrEmptyOrWhiteSpace().ShouldBe(result);
        }

        [TestCase("abc", true)]
        [TestCase("abc ", true)]
        [TestCase(" abc", true)]
        [TestCase(" abc ", true)]
        [TestCase("_", true)]
        [TestCase(" ", false)]
        [TestCase("", false)]
        [TestCase(null, false)]
        public void When_checking_a_string_is_not_null_or_empty_or_white_space(string input, bool result)
        {
            input.IsNotNullOrEmptyOrWhiteSpace().ShouldBe(result);
        }

        [TestCase("AB", "DEF", false)]
        [TestCase("ABC", "DEF", false)]
        [TestCase("abc", "def", false)]
        [TestCase("AbC", "aBc", false)]
        [TestCase("abc", "DEF", false)]
        [TestCase("", "DEF", false)]
        [TestCase(null, "DEF", false)]
        [TestCase("ABC", null, false)]
        [TestCase("", " ", false)]
        [TestCase("ABC", "ABC", true)]
        [TestCase("abc", "abc", true)]
        [TestCase("aBc", "aBc", true)]
        [TestCase("ABc", "ABc", true)]
        [TestCase("abC", "abC", true)]
        [TestCase("?8C_", "?8C_", true)]
        [TestCase("&¬`", "&¬`", true)]
        [TestCase(" ", " ", true)]
        [TestCase("", "", true)]
        [TestCase(null, null, true)]
        public void When_checking_a_string_is_equal_to_another_string(string left, string right, bool result)
        {
            left.IsEqualTo(right).ShouldBe(result);
            right.IsEqualTo(left).ShouldBe(result);
        }

        [TestCase("yes")]
        [TestCase("YES")]
        [TestCase("yES")]
        [TestCase("YeS")]
        [TestCase("YEs")]
        [TestCase("true")]
        [TestCase("TRUE")]
        [TestCase("TrUe")]
        [TestCase("1")]
        public void When_parsing_valid_string_as_true(string input)
        {
            bool result;
            input.TryParseAsBool(out result).ShouldBeTrue();
            result.ShouldBeTrue();
        }

        [TestCase("no")]
        [TestCase("NO")]
        [TestCase("nO")]
        [TestCase("No")]
        [TestCase("false")]
        [TestCase("FALSE")]
        [TestCase("fALsE")]
        [TestCase("0")]
        public void When_parsing_valid_string_as_false(string input)
        {
            bool result;
            input.TryParseAsBool(out result).ShouldBeTrue();
            result.ShouldBeFalse();
        }

        [TestCase("a")]
        [TestCase("o")]
        [TestCase("2")]
        [TestCase("-1")]
        [TestCase("-0")]
        [TestCase("1yes")]
        [TestCase("1no")]
        [TestCase("yes_")]
        [TestCase("no-")]
        [TestCase("1yes1")]
        [TestCase("0-no-")]
        public void When_parsing_invalid_boolean_string(string input)
        {
            bool result;
            input.TryParseAsBool(out result).ShouldBeFalse();
            result.ShouldBeFalse();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("  ")]
        public void When_parsing_invalid_string_as_boolean(string input)
        {
            bool result;
            Func<string, bool> parseFunc = str => str.TryParseAsBool(out result);

            Should.Throw<ArgumentException>(() => parseFunc(input))
                .Message.ShouldBe("String must not be null, empty or whitespace.");
        }

        [TestCase("abc", "b", true)]
        [TestCase("ab c", "b", true)]
        [TestCase("", "", true)]
        [TestCase("abc", "", true)]
        [TestCase("b", "b", true)]
        [TestCase("B", "b", false)]
        [TestCase("Jack is happy", "is", true)]
        [TestCase("Jack is happy", "IS", false)]
        [TestCase("Jack is happy", "IS", false)]
        [TestCase("Jack is happy", "sad", false)]
        [TestCase("Jack is happy", "SAD", false)]
        [TestCase("", "IS", false)]
        public void When_checking_if_a_string_contains_another_string_case_sensitive(string input, string stringToCheckFor, bool result)
        {
            input.Contains(stringToCheckFor, StringComparison.Ordinal).ShouldBe(result);
        }

        [TestCase("abc", "b", true)]
        [TestCase("ab c", "b", true)]
        [TestCase("", "", true)]
        [TestCase("abc", "", true)]
        [TestCase("b", "b", true)]
        [TestCase("B", "b", true)]
        [TestCase("Jack is happy", "is", true)]
        [TestCase("Jack is happy", "IS", true)]
        [TestCase("Jack is happy", "IS", true)]
        [TestCase("Jack is happy", "sad", false)]
        [TestCase("Jack is happy", "SAD", false)]
        [TestCase("", "IS", false)]
        public void When_checking_if_a_string_contains_another_string_case_insensitive(string input, string stringToCheckFor, bool result)
        {
            input.Contains(stringToCheckFor, StringComparison.OrdinalIgnoreCase).ShouldBe(result);
        }

        [Test]
        public void When_checking_if_a_string_is_equal_to_any_of_the_given_sequence_of_strings()
        {
            "abc".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc").ShouldBeTrue();
            "abc".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def").ShouldBeTrue();
            "def".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def").ShouldBeTrue();
            "def".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def", "ghi").ShouldBeTrue();
            "ghi".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def", "ghi").ShouldBeTrue();

            "aBc".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc").ShouldBeTrue();
            "ABC".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def").ShouldBeTrue();
            "dEf".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def").ShouldBeTrue();
            "DEF".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def", "ghi").ShouldBeTrue();
            "gHi".EqualsAny(StringComparer.OrdinalIgnoreCase, "abc", "def", "ghi").ShouldBeTrue();

            "aBc".EqualsAny(StringComparer.OrdinalIgnoreCase, "ABC").ShouldBeTrue();
            "ABC".EqualsAny(StringComparer.OrdinalIgnoreCase, "aBc", "Def").ShouldBeTrue();
            "dEf".EqualsAny(StringComparer.OrdinalIgnoreCase, "aBC", "deF").ShouldBeTrue();
            "DEF".EqualsAny(StringComparer.OrdinalIgnoreCase, "ABc", "dEf", "ghI").ShouldBeTrue();
            "gHi".EqualsAny(StringComparer.OrdinalIgnoreCase, "AbC", "DEF", "gHi").ShouldBeTrue();

            "abc".EqualsAny(StringComparer.OrdinalIgnoreCase, "def").ShouldBeFalse();
            "abc".EqualsAny(StringComparer.OrdinalIgnoreCase, "def", "ghi").ShouldBeFalse();
            "abc".EqualsAny(StringComparer.OrdinalIgnoreCase, "def", "ghi", "abk").ShouldBeFalse();

            "aBc".EqualsAny(StringComparer.OrdinalIgnoreCase, "def").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.OrdinalIgnoreCase, "def", "ghi").ShouldBeFalse();
            "AbC".EqualsAny(StringComparer.OrdinalIgnoreCase, "def", "ghi", "abk").ShouldBeFalse();

            "aBc".EqualsAny(StringComparer.OrdinalIgnoreCase, "dEf").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.OrdinalIgnoreCase, "DEf", "GHi").ShouldBeFalse();
            "AbC".EqualsAny(StringComparer.OrdinalIgnoreCase, "DEF", "gHI", "ABk").ShouldBeFalse();

            "abc".EqualsAny(StringComparer.Ordinal, "abc").ShouldBeTrue();
            "abc".EqualsAny(StringComparer.Ordinal, "abc", "def").ShouldBeTrue();
            "def".EqualsAny(StringComparer.Ordinal, "abc", "def").ShouldBeTrue();
            "def".EqualsAny(StringComparer.Ordinal, "abc", "def", "ghi").ShouldBeTrue();
            "ghi".EqualsAny(StringComparer.Ordinal, "abc", "def", "ghi").ShouldBeTrue();
            "gHi".EqualsAny(StringComparer.Ordinal, "AbC", "DEF", "gHi").ShouldBeTrue();

            "aBc".EqualsAny(StringComparer.Ordinal, "abc").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.Ordinal, "abc", "def").ShouldBeFalse();
            "dEf".EqualsAny(StringComparer.Ordinal, "abc", "def").ShouldBeFalse();
            "DEF".EqualsAny(StringComparer.Ordinal, "abc", "def", "ghi").ShouldBeFalse();
            "gHi".EqualsAny(StringComparer.Ordinal, "abc", "def", "ghi").ShouldBeFalse();

            "aBc".EqualsAny(StringComparer.Ordinal, "ABC").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.Ordinal, "aBc", "Def").ShouldBeFalse();
            "dEf".EqualsAny(StringComparer.Ordinal, "aBC", "deF").ShouldBeFalse();
            "DEF".EqualsAny(StringComparer.Ordinal, "ABc", "dEf", "ghI").ShouldBeFalse();

            "abc".EqualsAny(StringComparer.Ordinal, "def").ShouldBeFalse();
            "abc".EqualsAny(StringComparer.Ordinal, "def", "ghi").ShouldBeFalse();
            "abc".EqualsAny(StringComparer.Ordinal, "def", "ghi", "abk").ShouldBeFalse();

            "aBc".EqualsAny(StringComparer.Ordinal, "def").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.Ordinal, "def", "ghi").ShouldBeFalse();
            "AbC".EqualsAny(StringComparer.Ordinal, "def", "ghi", "abk").ShouldBeFalse();

            "aBc".EqualsAny(StringComparer.Ordinal, "dEf").ShouldBeFalse();
            "ABC".EqualsAny(StringComparer.Ordinal, "DEf", "GHi").ShouldBeFalse();
            "AbC".EqualsAny(StringComparer.Ordinal, "DEF", "gHI", "ABk").ShouldBeFalse();
        }

        [Test]
        public void When_separating_a_pascal_cased_string()
        {
            "ThisIsAPascalCasedString".SeparatePascalCase().ShouldBe("This Is A Pascal Cased String");
            "thisIsAPascalCasedString".SeparatePascalCase().ShouldBe("this Is A Pascal Cased String");
            "thisIsAPascalcasedString".SeparatePascalCase().ShouldBe("this Is A Pascalcased String");
            "This_IsA_PascalCasedString".SeparatePascalCase().ShouldBe("This_ Is A_ Pascal Cased String");
            "This Is A Pascal Cased String".SeparatePascalCase().ShouldBe("This  Is  A  Pascal  Cased  String");
        }

        [Test]
        public void When_converting_string_to_title_case()
        {
            "This Is A Pascal Cased String".ToTitleCase().ShouldBe("This Is A Pascal Cased String");
            "This is A pascal Cased string".ToTitleCase().ShouldBe("This Is A Pascal Cased String");
            "This is A pascal CasedString".ToTitleCase().ShouldBe("This Is A Pascal Casedstring");
            "this is a pascal cased string".ToTitleCase().ShouldBe("This Is A Pascal Cased String");
        }

        [Test]
        public void When_truncating_strings()
        {
            string nullStr = null;
            Should.Throw<ArgumentNullException>(() => nullStr.Truncate(1))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: input");

            string nullSuffix = null;
            Should.Throw<ArgumentNullException>(() => "someText".Truncate(2, nullSuffix))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: suffix");

            string.Empty.Truncate(1).ShouldBe(string.Empty);
            "1".Truncate(-1).ShouldBe("1");
            "1".Truncate(0).ShouldBe(string.Empty);
            "1".Truncate(1).ShouldBe("1");
            "1".Truncate(2).ShouldBe("1");
            "12".Truncate(1).ShouldBe("1");
            "123".Truncate(2).ShouldBe("12");
            "1234567".Truncate(2).ShouldBe("12");
            "12345678".Truncate(4).ShouldBe("1234");

            string.Empty.Truncate(1, "...").ShouldBe(string.Empty);
            "1".Truncate(-1, "...").ShouldBe("1");
            "1".Truncate(0, "...").ShouldBe("");
            "1".Truncate(1, "...").ShouldBe("1");
            "1".Truncate(2, "...").ShouldBe("1");
            "12".Truncate(1, "...").ShouldBe("1...");
            "123".Truncate(2, "...").ShouldBe("12...");
            "1234567".Truncate(2, "...").ShouldBe("12...");
            "12345678".Truncate(4, "...").ShouldBe("1234...");
        }

        [Test]
        public void When_removing_new_lines()
        {
            string nullStr = null;
            Should.Throw<ArgumentNullException>(() => nullStr.RemoveNewLines())
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: input");

            string.Empty.RemoveNewLines().ShouldBe(string.Empty);
            "hello".RemoveNewLines().ShouldBe("hello");
            "hello\r".RemoveNewLines().ShouldBe("hello");
            "hello\n".RemoveNewLines().ShouldBe("hello");
            "\rHello\r".RemoveNewLines().ShouldBe("Hello");
            "\nHello\n".RemoveNewLines().ShouldBe("Hello");
            "\rHello\n".RemoveNewLines().ShouldBe("Hello");
            "\r\nHello\r\n".RemoveNewLines().ShouldBe("Hello");
            "\r\n\nHello\r\n".RemoveNewLines().ShouldBe("Hello");
            "\r\n\nHello\r\r\n".RemoveNewLines().ShouldBe("Hello");
            "\r\n\nHe\rllo\r\r\n".RemoveNewLines().ShouldBe("Hello");
        }

        [Test]
        public void When_checking_if_a_string_is_a_palindrome()
        {
            string nullStr = null;
            Should.Throw<ArgumentNullException>(() => nullStr.IsPalindrome())
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: input");

            string.Empty.IsPalindrome().ShouldBeTrue();
            "1".IsPalindrome().ShouldBeTrue();
            "a".IsPalindrome().ShouldBeTrue();
            "aa".IsPalindrome().ShouldBeTrue();
            "ab".IsPalindrome().ShouldBeFalse();
            "ba".IsPalindrome().ShouldBeFalse();
            "bab".IsPalindrome().ShouldBeTrue();
            "bab ".IsPalindrome().ShouldBeFalse();
            " bab".IsPalindrome().ShouldBeFalse();
            " bab ".IsPalindrome().ShouldBeTrue();
            "civic".IsPalindrome().ShouldBeTrue();
            "deified".IsPalindrome().ShouldBeTrue();
            "Hannah".IsPalindrome().ShouldBeTrue();
            "devoved".IsPalindrome().ShouldBeTrue();
            "repaper".IsPalindrome().ShouldBeTrue();
            "Dot".IsPalindrome().ShouldBeFalse();
            "Is".IsPalindrome().ShouldBeFalse();
            "Palindrome".IsPalindrome().ShouldBeFalse();
        }

        [Test]
        public void When_extracing_value_from_tag()
        {
            string result;

            string nullStr = null;
            Should.Throw<ArgumentNullException>(() => nullStr.TryExtractValueFromTag("foo", out result))
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: input");

            string.Empty.TryExtractValueFromTag("foo", out result).ShouldBeFalse();
            result.ShouldBeNull();

            "<foo>bar</foo>".TryExtractValueFromTag("foo", out result).ShouldBeTrue();
            result.ShouldBe("bar");

            "<foo>bar</foo>".TryExtractValueFromTag("<foo>", out result).ShouldBeFalse();
            result.ShouldBeNull();

            "<Foo>bar</foo>".TryExtractValueFromTag("foo", out result).ShouldBeTrue();
            result.ShouldBe("bar");

            "<FOO>bar</FOO>".TryExtractValueFromTag("foo", out result).ShouldBeTrue();
            result.ShouldBe("bar");

            "<span>me</span>".TryExtractValueFromTag("span", out result).ShouldBeTrue();
            result.ShouldBe("me");

            "My name is <span>dupree</span> but".TryExtractValueFromTag("span", out result).ShouldBeTrue();
            result.ShouldBe("dupree");
        }

        [Test]
        public void When_compressing_then_decompressing_string()
        {
            const string Content = "This is a sample input string! @:-)";

            var compressedContent = Content.Compress();

            compressedContent.ShouldNotBeNullOrWhiteSpace();

            compressedContent.Decompress().ShouldBe(Content);
        }

        [Test]
        public void When_checking_a_string_is_a_valid_file_name()
        {
            "A".IsValidFileName().ShouldBeTrue();
            "MyFile".IsValidFileName().ShouldBeTrue();
            "MyFile.txt".IsValidFileName().ShouldBeTrue();
            "MyFile.txt ".IsValidFileName().ShouldBeTrue();
            " MyFile.txt".IsValidFileName().ShouldBeTrue();
            "My File.txt".IsValidFileName().ShouldBeTrue();
            "My-File.txt".IsValidFileName().ShouldBeTrue();
            "My-%File.txt".IsValidFileName().ShouldBeTrue();
            "My-!File.txt".IsValidFileName().ShouldBeTrue();

            "".IsValidFileName().ShouldBeFalse();
            " ".IsValidFileName().ShouldBeFalse();
            "  ".IsValidFileName().ShouldBeFalse();
            "/".IsValidFileName().ShouldBeFalse();
            "\\".IsValidFileName().ShouldBeFalse();
            "MyFile/".IsValidFileName().ShouldBeFalse();
            "\\MyFile".IsValidFileName().ShouldBeFalse();
            "MyFile>".IsValidFileName().ShouldBeFalse();
            "<MyFile>".IsValidFileName().ShouldBeFalse();
            "<MyFile".IsValidFileName().ShouldBeFalse();
            "<".IsValidFileName().ShouldBeFalse();
        }
    }
}