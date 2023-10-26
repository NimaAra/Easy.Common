namespace Easy.Common.Tests.Unit.BaseEncoding;

using NUnit.Framework;
using Shouldly;

[TestFixture]
public class Base36Tests
{
    [TestCase]
    public void When_encoding() => Base36.Encode(10).ShouldBe("A");
        
    [TestCase]
    public void When_decoding()  => Base36.Decode("A").ShouldBe(10);
}