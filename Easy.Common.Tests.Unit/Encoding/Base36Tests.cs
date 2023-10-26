namespace Easy.Common.Tests.Unit.Encoding;

using Easy.Common.Encoding;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class Base36Tests
{
    [TestCase]
    public void When_encoding() => Base36.Encode(10)
        .ShouldBe("A");

    [TestCase]
    public void When_decoding() => Base36.Decode("A")
        .ShouldBe(10);
}