namespace Easy.Common.Tests.Unit.RegexHelper
{
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using Shouldly;
    using RegexHelper = Easy.Common.RegexHelper;

    [TestFixture]
    internal sealed class RegexHelperTests
    {
        [Test]
        public void When_validating_valid_emails()
        {
            var helper = new RegexHelper();

            helper.IsValidEmail("david.jones@proseware.com").ShouldBeTrue();
            helper.IsValidEmail("d.j@server1.proseware.com").ShouldBeTrue();
            helper.IsValidEmail("jones@ms1.proseware.com").ShouldBeTrue();
            helper.IsValidEmail("j@proseware.com9").ShouldBeTrue();
            helper.IsValidEmail("js#internal@proseware.com").ShouldBeTrue();
            helper.IsValidEmail("j_9@[129.126.118.1]").ShouldBeTrue();
            helper.IsValidEmail("js@proseware.com9").ShouldBeTrue();
            helper.IsValidEmail("j.s@server1.proseware.com").ShouldBeTrue();
            helper.IsValidEmail("\"j\\\"s\\\"\"@proseware.com").ShouldBeTrue();
            helper.IsValidEmail("js@contoso.中国").ShouldBeTrue();
            helper.IsValidEmail("foo@bar.com").ShouldBeTrue();
            
            /*
             * This is a valid email address but is being rejected.
             * See: https://github.com/dotnet/docs/issues/5305
             */
            // [ToDo] - helper.IsValidEmail("#foo@bar.com").ShouldBeTrue();
        }

        [Test]
        public void When_validating_invalid_emails()
        {
            var helper = new RegexHelper();

            helper.IsValidEmail("j.@server1.proseware.com").ShouldBeFalse();
            helper.IsValidEmail("j..s@proseware.com").ShouldBeFalse();
            helper.IsValidEmail("js*@proseware.com").ShouldBeFalse();
            helper.IsValidEmail("js@proseware..com").ShouldBeFalse();
            helper.IsValidEmail("js@proseware..com").ShouldBeFalse();
        }

        [Test]
        public void When_converting_input_to_case_incensitive_regex_pattern()
        {
            const string SampleInput = "this is some STUFF fOo-bar";
            const string Argument = "foo-bAr";
            var caseIncensitiveArgument = RegexHelper.ToCaseInsensitiveRegexPattern(Argument);

            caseIncensitiveArgument.ShouldBe("[fF][oO][oO]-[bB][aA][rR]");
            Regex.IsMatch(SampleInput, Argument).ShouldBeFalse();
            Regex.IsMatch(SampleInput, caseIncensitiveArgument).ShouldBeTrue();

            new [] { "aNt", "A-", "a|", "A", "a" }
                .ShouldAllBe(x => Regex.IsMatch(x, RegexHelper.ToCaseInsensitiveRegexPattern("A.*")));

            RegexHelper.ToCaseInsensitiveRegexPattern(null).ShouldBeNull();
            RegexHelper.ToCaseInsensitiveRegexPattern(string.Empty).ShouldBe(string.Empty);
            RegexHelper.ToCaseInsensitiveRegexPattern(" ").ShouldBe(" ");

            RegexHelper.ToCaseInsensitiveRegexPattern("a").ShouldBe("[aA]");
            RegexHelper.ToCaseInsensitiveRegexPattern("ab").ShouldBe("[aA][bB]");
            RegexHelper.ToCaseInsensitiveRegexPattern("aB").ShouldBe("[aA][bB]");

            RegexHelper.ToCaseInsensitiveRegexPattern("ab\\S").ShouldBe("[aA][bB]\\S");
            RegexHelper.ToCaseInsensitiveRegexPattern("ab\\SS").ShouldBe("[aA][bB]\\S[sS]");
            RegexHelper.ToCaseInsensitiveRegexPattern("ab\\Ss").ShouldBe("[aA][bB]\\S[sS]");

            RegexHelper.ToCaseInsensitiveRegexPattern("ab\\m").ShouldBe("[aA][bB]\\[mM]");

            RegexHelper.ToCaseInsensitiveRegexPattern("\\D").ShouldBe("\\D");
            RegexHelper.ToCaseInsensitiveRegexPattern("\\DX\\Q").ShouldBe("\\D[xX]\\[qQ]");

            RegexHelper.ToCaseInsensitiveRegexPattern("(?<name>ab\\SX)")
                .ShouldBe("(?<name>[aA][bB]\\S[xX])");
            
            RegexHelper.ToCaseInsensitiveRegexPattern("(?<name>ab\\SX)(?<foo>A)")
                .ShouldBe("(?<name>[aA][bB]\\S[xX])(?<foo>[aA])");
            
            RegexHelper.ToCaseInsensitiveRegexPattern("(?<name>ab\\SX)(<foo>A)")
                .ShouldBe("(?<name>[aA][bB]\\S[xX])(<[fF][oO][oO]>[aA])");
            
            RegexHelper.ToCaseInsensitiveRegexPattern("(?<name>ab\\SX)(?<fooA)")
                .ShouldBe("(?<name>[aA][bB]\\S[xX])(?<[fF][oO][oO][aA])");
        }
    }
}