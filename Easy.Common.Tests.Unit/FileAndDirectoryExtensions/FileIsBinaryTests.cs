namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using System.IO;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class FileIsBinaryTests
    {
        [Test]
        public void When_checking_an_empty_file()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_a_non_empty_text_file()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllText(tmpFile.FullName, "Foo");
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_a_text_file_with_tab_only()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllText(tmpFile.FullName, new string('\t', 1));
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_a_text_file_with_carriange_return_only()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllText(tmpFile.FullName, new string('\r', 1));
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_a_text_file_with_newline_only()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllText(tmpFile.FullName, new string('\n', 1));
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_an_empty_binary_file()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllBytes(tmpFile.FullName, new byte[0]);
                tmpFile.IsBinary().ShouldBeFalse();
            } finally
            {
                tmpFile.Delete();
            }
        }

        [Test]
        public void When_checking_a_non_empty_binary_file()
        {
            var tmpFile = new FileInfo(Path.GetTempFileName());
            try
            {
                File.WriteAllBytes(tmpFile.FullName, new byte[] {1});
                tmpFile.IsBinary().ShouldBeTrue();
            } finally
            {
                tmpFile.Delete();
            }
        }
    }
}