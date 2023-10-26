namespace Easy.Common.Tests.Unit.StringBuilderExtensions;

using System.Text;
using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class StringBuilderExtensionsTests
{
    [Test]
    public void When_appending_text_multiple_times()
    {
        var builder = new StringBuilder();

        builder.AppendMultiple("|-", 1)
            .ToString().ShouldBe("|-");

        builder.Clear().AppendMultiple("|-|", 5)
            .ToString().ShouldBe("|-||-||-||-||-|");
    }

    [Test]
    public void When_appending_characters_multiple_times()
    {
        var builder = new StringBuilder();

        builder.AppendMultiple('|', 1)
            .ToString().ShouldBe("|");

        builder.Clear().AppendMultiple('-', 5)
            .ToString().ShouldBe("-----");
    }

    [Test]
    public void When_appending_space()
    {
        var builder = new StringBuilder();

        builder.AppendSpace(3)
            .ToString().ShouldBe("   ");
    }
}