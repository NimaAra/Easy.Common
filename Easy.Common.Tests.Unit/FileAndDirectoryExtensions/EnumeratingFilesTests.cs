namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using System.IO;
    using System.Linq;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class EnumeratingFilesTests
    {
        private DirectoryInfo _root;
        
        [OneTimeSetUp]
        public void SetUp() => _root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-EnumeratingFilesTests-_"));
        
        [Test]
        public void When_enumerating_files()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();

            File.Create(Path.Combine(dir.FullName, "0.bin")).Dispose();
            File.Create(Path.Combine(dir.FullName, "00.bin")).Dispose();

            var aSub = dir.CreateSubdirectory("A");
            File.Create(Path.Combine(aSub.FullName, "A_0.bin")).Dispose();

            var aSub1 = aSub.CreateSubdirectory("a1");
            File.Create(Path.Combine(aSub1.FullName, "A1_0.bin")).Dispose();

            var aSub1s = aSub1.CreateSubdirectory("a1s");
            File.Create(Path.Combine(aSub1s.FullName, "A11_0.bin")).Dispose();

            var bSub = dir.CreateSubdirectory("B");
            File.Create(Path.Combine(bSub.FullName, "B_0.bin")).Dispose();

            var result = dir.EnumerateFilesSafe().ToArray();
            result.Length.ShouldBe(2);
            result.ShouldContain(f => f.Name == "0.bin");
            result.ShouldContain(f => f.Name == "00.bin");
        }

        [Test]
        public void When_enumerating_files_recursive()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();
                
            File.Create(Path.Combine(dir.FullName, "0.bin")).Dispose();
            File.Create(Path.Combine(dir.FullName, "00.bin")).Dispose();

            var aSub = dir.CreateSubdirectory("A");
            File.Create(Path.Combine(aSub.FullName, "A_0.bin")).Dispose();

            var aSub1 = aSub.CreateSubdirectory("a1");
            File.Create(Path.Combine(aSub1.FullName, "A1_0.bin")).Dispose();

            var aSub1s = aSub1.CreateSubdirectory("a1s");
            File.Create(Path.Combine(aSub1s.FullName, "A11_0.bin")).Dispose();

            var bSub = dir.CreateSubdirectory("B");
            File.Create(Path.Combine(bSub.FullName, "B_0.bin")).Dispose();
                
            var result = dir.EnumerateFilesSafe("*", SearchOption.AllDirectories).ToArray();
            result.Length.ShouldBe(6);
            result.ShouldContain(f => f.Name == "0.bin");
            result.ShouldContain(f => f.Name == "00.bin");
                
            result.ShouldContain(f => f.Name == "A_0.bin" && f.DirectoryName.EndsWith("A"));
            result.ShouldContain(f => f.Name == "A1_0.bin" && f.DirectoryName.EndsWith(@"A\a1"));
            result.ShouldContain(f => f.Name == "A11_0.bin" && f.DirectoryName.EndsWith(@"A\a1\a1s"));
            result.ShouldContain(f => f.Name == "B_0.bin" && f.DirectoryName.EndsWith(@"B"));

        }

        [Test]
        public void When_enumerating_files_search()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();
                
            File.Create(Path.Combine(dir.FullName, "0.bin")).Dispose();
            File.Create(Path.Combine(dir.FullName, "00.bin")).Dispose();

            var aSub = dir.CreateSubdirectory("A");
            File.Create(Path.Combine(aSub.FullName, "A_0.bin")).Dispose();

            var aSub1 = aSub.CreateSubdirectory("a1");
            File.Create(Path.Combine(aSub1.FullName, "A1_0.bin")).Dispose();

            var aSub1s = aSub1.CreateSubdirectory("a1s");
            File.Create(Path.Combine(aSub1s.FullName, "A11_0.bin")).Dispose();

            var bSub = dir.CreateSubdirectory("B");
            File.Create(Path.Combine(bSub.FullName, "B_0.bin")).Dispose();
                
            var result = dir.EnumerateFilesSafe("a*", SearchOption.AllDirectories).ToArray();
            result.Length.ShouldBe(3);
            result.ShouldContain(f => f.Name == "A_0.bin" && f.DirectoryName.EndsWith("A"));
            result.ShouldContain(f => f.Name == "A1_0.bin" && f.DirectoryName.EndsWith(@"A\a1"));
            result.ShouldContain(f => f.Name == "A11_0.bin" && f.DirectoryName.EndsWith(@"A\a1\a1s"));
        }

        [OneTimeTearDown]
        public void TearDown() => _root.Delete(true);
    }
}