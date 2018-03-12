namespace Easy.Common.Tests.Unit.StreamExtensions
{
    using System;
    using System.IO;
    using System.Text;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class CountingLinesTests
    {
        [Test]
        public void When_reading_null_stream()
        {
            Should.Throw<ArgumentNullException>(() => ((MemoryStream) null).CountLines())
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: stream");
        }
        
        [Test]
        public void When_reading_empty_stream()
        {
            using (var mem = new MemoryStream())
            {
                mem.CountLines().ShouldBe(0);
            }
        }
        
        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void When_reading_stream_with_empty_line(string content)
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes(content);
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(1);
            }
        }

        [TestCase("\r\r")]
        [TestCase("\n\n")]
        [TestCase("\r\n\r\n")]
        public void When_reading_stream_with_empty_lines(string content)
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes(content);
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(2);
            }
        }

        [TestCase("A")]
        [TestCase("A\r")]
        [TestCase("A\n")]
        [TestCase("A\r\n")]
        public void When_reading_stream_with_single_line(string content)
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes(content);
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(1);
            }
        }

        [Test]
        public void When_reading_stream_with_lines_terminated_by_carriage_return()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes("A\rB\r\r");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }

        [Test]
        public void When_reading_stream_with_lines_terminated_by_new_line()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes("A\nB\n\n");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }

        [Test]
        public void When_reading_stream_with_lines_terminated_by_carriage_return_followed_by_new_line()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes("A\r\nB\r\n\r\n");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }

        [Test]
        public void When_reading_long_line_terminated_by_carriage_return()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes($"A\r{new string('_', 1_500_000)}\rB");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }

        [Test]
        public void When_reading_long_line_terminated_by_carriage_by_new_line()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes($"A\n{new string('_', 1_500_000)}\nB");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }

        [Test]
        public void When_reading_long_line_terminated_by_carriage_return_followed_by_new_line()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes($"A\r\n{new string('_', 1_500_000)}\r\nB");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                mem.CountLines().ShouldBe(3);
            }
        }
    }
}