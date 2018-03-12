namespace Easy.Common.Tests.Unit.StreamExtensions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class ReadingLinesTests
    {
        [Test]
        public void When_reading_null_stream()
        {
            Should.Throw<ArgumentNullException>(() => ((MemoryStream) null).ReadLines())
                .Message.ShouldBe("Value cannot be null.\r\nParameter name: stream");
        }
        
        [Test]
        public void When_reading_empty_stream()
        {
            using (var mem = new MemoryStream())
            {
                mem.ReadLines().ShouldBeEmpty();
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(1);
                lines[0].ShouldBe(string.Empty);
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(2);
                lines[0].ShouldBe(string.Empty);
                lines[1].ShouldBe(string.Empty);
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(1);
                lines[0].ShouldBe("A");
            }
        }

        [Test]
        public void When_reading_stream_with_lines_terminated_by_carriage_return()
        {
            using (var mem = new MemoryStream())
            {
                var input = Encoding.UTF8.GetBytes("A\rB_\r\r");
                mem.Write(input, 0, input.Length);
                mem.Position = 0;

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe("B_");
                lines[2].ShouldBe(string.Empty);
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe("B");
                lines[2].ShouldBe(string.Empty);
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe("B");
                lines[2].ShouldBe(string.Empty);
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe(new string('_', 1_500_000));
                lines[2].ShouldBe("B");
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe(new string('_', 1_500_000));
                lines[2].ShouldBe("B");
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

                var lines = mem.ReadLines().ToArray();
                lines.Length.ShouldBe(3);
                lines[0].ShouldBe("A");
                lines[1].ShouldBe(new string('_', 1_500_000));
                lines[2].ShouldBe("B");
            }
        }
    }
}