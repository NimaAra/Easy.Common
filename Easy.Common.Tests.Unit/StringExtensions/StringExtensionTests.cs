namespace Easy.Common.Tests.Unit.StringExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
            input.IsNotNullOrEmpty().ShouldBe(result);
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

        [Test]
        public void When_parsing_null_string_as_boolean()
        {
            Should.Throw<ArgumentNullException>(() =>
            {
                bool result;
                ((string) null).TryParseAsBool(out result).ShouldBeFalse();
                result.ShouldBeFalse();
            })
                .Message.ShouldBe("Value cannot be null. (Parameter 'value')");
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("  ")]
        public void When_parsing_invalid_string_as_boolean(string input)
        {
            bool result;
            input.TryParseAsBool(out result).ShouldBeFalse();
            result.ShouldBeFalse();
        }

        [TestCase("abc", "b", true)]
        [TestCase("ab c", "b", true)]
        [TestCase("", "", true)]
        [TestCase("abc", "", true)]
        [TestCase("b", "b", true)]
        [TestCase("B", "b", false)]
        [TestCase("Jack is happy", "is", true)]
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
            "This IS A Pascal Cased String".SeparatePascalCase().ShouldBe("This  I S  A  Pascal  Cased  String");
            "This Is A Pascal Cased String.".SeparatePascalCase().ShouldBe("This  Is  A  Pascal  Cased  String.");
        }

        [Test]
        public void When_converting_string_to_pascal_case()
        {
            "This Is A Pascal Cased String".ToPascalCase().ShouldBe("This Is A Pascal Cased String");
            "This is A pascal Cased string".ToPascalCase().ShouldBe("This Is A Pascal Cased String");
            "This is A pascal CasedString".ToPascalCase().ShouldBe("This Is A Pascal Casedstring");
            "this IS a pascal cased string".ToPascalCase().ShouldBe("This IS A Pascal Cased String");
            "this is a pascal cased string.".ToPascalCase().ShouldBe("This Is A Pascal Cased String.");
        }

        [Test]
        public void When_truncating_strings()
        {
            Should.Throw<ArgumentNullException>(() => ((string)null).Truncate(1))
                .Message.ShouldBe("Value cannot be null. (Parameter 'input')");

            Should.Throw<ArgumentNullException>(() => "someText".Truncate(2, null))
                .Message.ShouldBe("Value cannot be null. (Parameter 'suffix')");

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
            Should.Throw<ArgumentNullException>(() => ((string)null).RemoveNewLines())
                .Message.ShouldBe("Value cannot be null. (Parameter 'input')");

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
            Should.Throw<ArgumentNullException>(() => ((string)null).IsPalindrome())
                .Message.ShouldBe("Value cannot be null. (Parameter 'input')");

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
        public void When_extracting_value_from_tag()
        {
            string result;

            Should.Throw<ArgumentNullException>(() => ((string)null).TryExtractValueFromTag("foo", out result))
                .Message.ShouldBe("Value cannot be null. (Parameter 'input')");

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

        [Test]
        public void When_checking_a_string_is_a_valid_path_name()
        {
            "A".IsValidPathName().ShouldBeTrue();
            "MyFile".IsValidPathName().ShouldBeTrue();
            "MyFile.txt".IsValidPathName().ShouldBeTrue();
            "MyFile.txt ".IsValidPathName().ShouldBeTrue();
            " MyFile.txt".IsValidPathName().ShouldBeTrue();
            "My File.txt".IsValidPathName().ShouldBeTrue();
            "My-File.txt".IsValidPathName().ShouldBeTrue();
            "My-%File.txt".IsValidPathName().ShouldBeTrue();
            "My-!File.txt".IsValidPathName().ShouldBeTrue();
            "My-!File.txt/Foo".IsValidPathName().ShouldBeTrue();
            "/".IsValidPathName().ShouldBeTrue();
            "\\".IsValidPathName().ShouldBeTrue();
            "foo\\bar".IsValidPathName().ShouldBeTrue();
            "MyFile/".IsValidPathName().ShouldBeTrue();
            "\\MyFile".IsValidPathName().ShouldBeTrue();

            "MyFile>".IsValidPathName().ShouldBeTrue();
            "<MyFile>".IsValidPathName().ShouldBeTrue();
            "<MyFile".IsValidPathName().ShouldBeTrue();
            "<".IsValidPathName().ShouldBeTrue();

            "".IsValidPathName().ShouldBeFalse();
            " ".IsValidPathName().ShouldBeFalse();
            "  ".IsValidPathName().ShouldBeFalse();
        }

        [Test]
        public void When_gettingIndexes()
        {
            const string StartTag = "(?<";
            const string EndTag = ">";

            var result = "(?<name>ab\\SX)".GetStartAndEndIndexes(StartTag, EndTag).ToArray();
            result.ShouldNotBeEmpty();
            result.Length.ShouldBe(1);
            result[0].ShouldBe(new KeyValuePair<int, int>(0, 7));

            result = "(?<nameab\\SX)".GetStartAndEndIndexes(StartTag, EndTag).ToArray();
            result.ShouldBeEmpty();

            result = "(?<name>ab\\SX)(?<foo>ab\\SX)".GetStartAndEndIndexes(StartTag, EndTag).ToArray();
            result.ShouldNotBeEmpty();
            result.Length.ShouldBe(2);
            result[0].ShouldBe(new KeyValuePair<int, int>(0, 7));
            result[1].ShouldBe(new KeyValuePair<int, int>(14, 20));

            result = "<div>foo</div>".GetStartAndEndIndexes("<div>", "</div>").ToArray();
            result.ShouldNotBeEmpty();
            result.Length.ShouldBe(1);
            result[0].ShouldBe(new KeyValuePair<int, int>(0, 8));
        }

        [Test]
        public void When_generating_slug()
        {
            "Foo".GenerateSlug().ShouldBe("foo");
            "FooIsBar".GenerateSlug().ShouldBe("fooisbar");
            "Foo Is Not Bar".GenerateSlug().ShouldBe("foo-is-not-bar");
            "Foo/maybe=Bar".GenerateSlug().ShouldBe("foomaybebar");
            @"Foo\maybe=-Bar".GenerateSlug().ShouldBe("foomaybe-bar");

            "Foo".GenerateSlug(0).ShouldBeEmpty();
            "Foo".GenerateSlug(2).ShouldBe("fo");
            "FooIsBar".GenerateSlug(4).ShouldBe("fooi");
            "Foo Is Not Bar".GenerateSlug(6).ShouldBe("foo-is");
        }

        [Test]
        public void When_converting_english_digits_to_persinal_numbers()
        {
            "0".ToPersianNumber().ShouldBe("۰");
            "1".ToPersianNumber().ShouldBe("۱");

            "A123456c789032/5- X".ToPersianNumber().ShouldBe("A۱۲۳۴۵۶c۷۸۹۰۳۲/۵- X");
        }

        [Test]
        public void When_splitting_and_trimming_string()
        {
            var result = "hello body okay foo thERE".SplitAndTrim(' ');
            result.ShouldNotBeNull();
            result.Length.ShouldBe(5);
            result.ShouldBe(new [] {"hello", "body", "okay", "foo", "thERE"});
        }

        [Test]
        public void When_getting_null_if_string_is_empty()
        {
            "foo".NullIfEmpty().ShouldBe("foo");
            "-".NullIfEmpty().ShouldBe("-");
            " ".NullIfEmpty().ShouldBe(" ");
            string.Empty.NullIfEmpty().ShouldBeNull();
            ((string)null).NullIfEmpty().ShouldBeNull();
        }

        [Test]
        public void When_getting_size_of_string()
        {
            string.Empty.GetSize().ShouldBe(0);
            "\r".GetSize().ShouldBe(2);
            "\r\n".GetSize().ShouldBe(4);
            "A".GetSize().ShouldBe(2);
            "AB".GetSize().ShouldBe(4);
            "ABC".GetSize().ShouldBe(6);
            "֎".GetSize().ShouldBe(2);
            "❤".GetSize().ShouldBe(2);

            Should.Throw<NullReferenceException>(() => ((string) null).GetSize())
                .Message.ShouldBe("Object reference not set to an instance of an object.");
        }

        [Test]
        public void When_obfuscating_a_string()
        {
            string.Empty.Obfuscate().ShouldBeEmpty();
            
            "!".Obfuscate().ShouldBe("*");
            "!".Obfuscate('!').ShouldBe("!");
            "some-input".Obfuscate().ShouldBe("so********");
            "some-other-input".Obfuscate('!').ShouldBe("some!!!!!!!!!!!!");
        }
    }
}