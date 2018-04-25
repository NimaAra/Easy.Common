namespace Easy.Common.Tests.Unit.FileAndDirectoryExtensions
{
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using Easy.Common.Extensions;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    internal sealed class EnumeratingDirectoriesTests
    {
        [Test]
        public void When_enumerating_directories()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-root-_", Path.GetRandomFileName()));
            try
            {
                root.Create();
                var aSub = root.CreateSubdirectory("A");
                var aSub1 = aSub.CreateSubdirectory("a1");
                aSub1.CreateSubdirectory("a1s");
                var bSub = root.CreateSubdirectory("B");
                
                var result = root.EnumerateDirectoriesSafe().ToArray();
                result.Length.ShouldBe(2);
                result.ShouldContain(d => d.Name == "A");
                result.ShouldContain(d => d.Name == "B");

            } finally
            {
                root.Delete(true);
            }
        }

        [Test]
        public void When_enumerating_directories_recursive()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-root-_", Path.GetRandomFileName()));
            try
            {
                root.Create();
                var aSub = root.CreateSubdirectory("A");
                var aSub1 = aSub.CreateSubdirectory("a1");
                aSub1.CreateSubdirectory("a1s");
                var bSub = root.CreateSubdirectory("B");
                
                var result = root.EnumerateDirectoriesSafe("*", SearchOption.AllDirectories).ToArray();
                result.Length.ShouldBe(4);
                result.ShouldContain(d => d.Name == "A");
                result.ShouldContain(d => d.Name == "a1");
                result.ShouldContain(d => d.Name == "a1s");
                result.ShouldContain(d => d.Name == "B");

            } finally
            {
                root.Delete(true);
            }
        }

        [Test]
        public void When_enumerating_directories_search()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-root-_", Path.GetRandomFileName()));
            try
            {
                root.Create();
                var aSub = root.CreateSubdirectory("A");
                var aSub1 = aSub.CreateSubdirectory("a1");
                aSub1.CreateSubdirectory("a1s");
                var bSub = root.CreateSubdirectory("B");
                
                var result = root.EnumerateDirectoriesSafe("a*", SearchOption.AllDirectories).ToArray();
                result.Length.ShouldBe(3);
                result.ShouldContain(d => d.Name == "A");
                result.ShouldContain(d => d.Name == "a1");
                result.ShouldContain(d => d.Name == "a1s");

            } finally
            {
                root.Delete(true);
            }
        }

        [Test]
        public void When_enumerating_all_directories()
        {
            var root = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "_-root-_", Path.GetRandomFileName()));
            try
            {
                root.Create();
                var aSub = root.CreateSubdirectory("A");
                var aSub1 = aSub.CreateSubdirectory("a1");
                aSub1.CreateSubdirectory("a1s");
                var bSub = root.CreateSubdirectory("B");

                var result = new ConcurrentBag<DirectoryInfo>();
                
                root.EnumerateAllDirectoriesSafe(d => result.Add(d));
                
                result.Count.ShouldBe(4);
                result.ShouldContain(d => d.Name == "A");
                result.ShouldContain(d => d.Name == "a1");
                result.ShouldContain(d => d.Name == "a1s");
                result.ShouldContain(d => d.Name == "B");
            } finally
            {
                root.Delete(true);
            }
        }
    }
}