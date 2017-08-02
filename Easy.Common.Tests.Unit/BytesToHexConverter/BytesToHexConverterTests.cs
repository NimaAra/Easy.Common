namespace Easy.Common.Tests.Unit.BytesToHexConverter
{
    using System;
    using NUnit.Framework;
    using Shouldly;
    using BytesToHexConverter = Easy.Common.BytesToHexConverter;

    [TestFixture]
    internal sealed class BytesToHexConverterTests
    {
        [Test]
        public void When_converting_bytes()
        {
            var bytes = new byte[] { 37, 80, 68, 70 };
            var hex = BytesToHexConverter.ToHexString(bytes);
            hex.ShouldBe("25504446");
        }

        [Test]
        public void When_converting_hex()
        {
            const string Hex = "25504446";
            var bytes = BytesToHexConverter.FromHexString(Hex);
            bytes.ShouldBe(new byte[] { 37, 80, 68, 70 });
        }

        [Test]
        public void When_converting_empty_bytes()
        {
            BytesToHexConverter.ToHexString(new byte[0])
                .ShouldBe(string.Empty);
        }

        [Test]
        public void When_converting_empty_hex()
        {
            BytesToHexConverter.FromHexString(string.Empty)
                .ShouldBe(new byte[0]);
        }

        [Test]
        public void When_converting_null_bytes()
        {
            Should.Throw<ArgumentNullException>(() => BytesToHexConverter.ToHexString(null));
        }

        [Test]
        public void When_converting_null_hex()
        {
            Should.Throw<ArgumentNullException>(() => BytesToHexConverter.FromHexString(null));
        }
    }
}