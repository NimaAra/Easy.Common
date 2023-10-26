namespace Easy.Common.Tests.Unit.Delimiters;

using Easy.Common;
using NUnit.Framework;
using Shouldly;

[TestFixture]
internal sealed class DelimitersTests
{
    [Test]
    public void When_checking_delimiters()
    {
        Delimiters.Comma.ShouldBe(new[] { ',' });
        Delimiters.Dot.ShouldBe(new[] { '.' });
        Delimiters.SemiColon.ShouldBe(new[] { ';' });
        Delimiters.Colon.ShouldBe(new[] { ':' });
        Delimiters.Space.ShouldBe(new[] { ' ' });
        Delimiters.Tab.ShouldBe(new[] { '\t' });
        Delimiters.Pipe.ShouldBe(new[] { '|' });
    }
}