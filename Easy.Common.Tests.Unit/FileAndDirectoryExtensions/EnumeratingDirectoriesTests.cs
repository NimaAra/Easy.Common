namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using System.IO;
    using System.Linq;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class EnumeratingDirectoriesTests
    {
        private DirectoryInfo _root;
        
        [OneTimeSetUp]
        public void SetUp() => _root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-EnumeratingDirectoriesTests-_"));
        
        [Test]
        public void When_enumerating_directories()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();
            var aSub = dir.CreateSubdirectory("A");
            var aSub1 = aSub.CreateSubdirectory("a1");
            aSub1.CreateSubdirectory("a1s");
            var bSub = dir.CreateSubdirectory("B");
                
            var result = dir.EnumerateDirectoriesSafe().ToArray();
            result.Length.ShouldBe(2);
            result.ShouldContain(d => d.Name == "A");
            result.ShouldContain(d => d.Name == "B");
        }

        [Test]
        public void When_enumerating_directories_recursive()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();
            var aSub = dir.CreateSubdirectory("A");
            var aSub1 = aSub.CreateSubdirectory("a1");
            aSub1.CreateSubdirectory("a1s");
            var bSub = dir.CreateSubdirectory("B");
                
            var result = dir.EnumerateDirectoriesSafe("*", SearchOption.AllDirectories).ToArray();
            result.Length.ShouldBe(4);
            result.ShouldContain(d => d.Name == "A");
            result.ShouldContain(d => d.Name == "a1");
            result.ShouldContain(d => d.Name == "a1s");
            result.ShouldContain(d => d.Name == "B");
        }

        [Test]
        public void When_enumerating_directories_search()
        {
            var dir = _root.CreateSubdirectory(Path.GetRandomFileName());
            dir.Create();
            var aSub = dir.CreateSubdirectory("A");
            var aSub1 = aSub.CreateSubdirectory("a1");
            aSub1.CreateSubdirectory("a1s");
            var bSub = dir.CreateSubdirectory("B");
                
            var result = dir.EnumerateDirectoriesSafe("a*", SearchOption.AllDirectories).ToArray();
            result.Length.ShouldBe(3);
            result.ShouldContain(d => d.Name == "A");
            result.ShouldContain(d => d.Name == "a1");
            result.ShouldContain(d => d.Name == "a1s");
        }

        [OneTimeTearDown]
        public void TearDown() => _root.Delete(true);
    }
}