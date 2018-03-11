namespace Easy.Common.Tests.Unit.StreamExtensions
{
    using System.IO;
    using System.Text;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class DetectingEncodingTests
    {
        [Test]
        public void When_detecting_empty_stream()
        {
            var defaultEncodingIfNoBom = Encoding.ASCII;
            
            using (var mem = new MemoryStream())
            {
                mem.Position = 0;
                mem.DetectEncoding(defaultEncodingIfNoBom).ShouldBe(Encoding.ASCII);
                mem.Position.ShouldBe(0);
            }
        }

        [Test]
        public void When_detecting_utf8()
        {
            var defaultEncodingIfNoBom = Encoding.ASCII;
            
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem, Encoding.UTF8))
            {
                writer.Write("Hello");
                writer.Flush();

                mem.Position = 0;
                mem.DetectEncoding(defaultEncodingIfNoBom).ShouldBe(Encoding.UTF8);
                mem.Position.ShouldBe(0);
            }
        }

        [Test]
        public void When_detecting_utf16()
        {
            var defaultEncodingIfNoBom = Encoding.ASCII;
            
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem, Encoding.Unicode))
            {
                writer.Write("Hello");
                writer.Flush();

                mem.Position = 0;
                mem.DetectEncoding(defaultEncodingIfNoBom).ShouldBe(Encoding.Unicode);
                mem.Position.ShouldBe(0);
            }
        }

        [Test]
        public void When_detecting_utf32()
        {
            var defaultEncodingIfNoBom = Encoding.ASCII;
            
            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem, Encoding.UTF32))
            {
                writer.Write("Hello");
                writer.Flush();

                mem.Position = 0;
                mem.DetectEncoding(defaultEncodingIfNoBom).ShouldBe(Encoding.UTF32);
                mem.Position.ShouldBe(0);
            }
        }
    }
}