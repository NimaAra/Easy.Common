namespace Easy.Common.Tests.Unit.Encoding;

using Easy.Common.Encoding;
using NUnit.Framework;
using Shouldly;

[TestFixture]
public sealed class Base64Tests
{
    [TestCase]
    public void When_encoding() => Base64.Encode(new byte[] { 1, 15, 30, 40, 0, 13, 10, 43 })
        .ShouldBe("AQ8eKAANCis");

    [TestCase]
    public void When_decoding() => Base64.Decode("AQ8eKAANCis")
        .ShouldBe(new byte[] { 1, 15, 30, 40, 0, 13, 10, 43 });
}