namespace Easy.Common.Tests.Unit.BytesToHexConverter;

using NUnit.Framework;
using Shouldly;
using System;
using BytesToHexConverter = Easy.Common.BytesToHexConverter;

[TestFixture]
internal sealed class BytesToHexConverterTests
{
    [Test]
    public void When_converting_bytes()
    {
        byte[] bytes = new byte[] { 37, 80, 68, 70 };
        string hex = BytesToHexConverter.ToHex(bytes);
        hex.ShouldBe("25504446");
    }

    [Test]
    public void When_converting_hex()
    {
        const string Hex = "25504446";
        byte[] bytes = BytesToHexConverter.FromHex(Hex);
        bytes.ShouldBe(new byte[] { 37, 80, 68, 70 });
    }

    [Test]
    public void When_converting_empty_bytes() =>
        BytesToHexConverter.ToHex(Array.Empty<byte>())
            .ShouldBe(string.Empty);

    [Test]
    public void When_converting_empty_hex() =>
        BytesToHexConverter.FromHex(string.Empty)
            .ShouldBe(Array.Empty<byte>());

    [Test]
    public void When_converting_null_bytes() => Should.Throw<ArgumentNullException>(() => BytesToHexConverter.ToHex(null));

    [Test]
    public void When_converting_null_hex() => Should.Throw<ArgumentNullException>(() => BytesToHexConverter.FromHex(null));
}