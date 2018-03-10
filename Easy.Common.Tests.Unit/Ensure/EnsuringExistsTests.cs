namespace Easy.Common.Tests.Unit.Ensure
{
    using System.IO;
    using NUnit.Framework;
    using Shouldly;
    using Ensure = Easy.Common.Ensure;

    [TestFixture]
    internal sealed class EnsuringExistsTests
    {
        [Test]
        public void When_ensuring_directoryInfo_exists()
        {
            var nonExistingDirectoryPath = Path.GetRandomFileName();
            var nonExistingDirInfo = new DirectoryInfo(nonExistingDirectoryPath);

            Should.Throw<DirectoryNotFoundException>(() => Ensure.Exists(nonExistingDirInfo))
                .Message.ShouldBe($"Cannot find: '{nonExistingDirInfo.FullName}'.");

            nonExistingDirInfo.Create();

            var existingDirInfo = nonExistingDirInfo;

            Should.NotThrow(() => Ensure.Exists(existingDirInfo));
            Ensure.Exists(existingDirInfo).ShouldBe(existingDirInfo);

            try
            {
                existingDirInfo.Delete();
            } catch { /* ignored */ }
        }

        [Test]
        public void When_ensuring_fileInfo_exists()
        {
            var nonExistingFilePath = Path.GetRandomFileName();
            var nonExistingFileInfo = new FileInfo(nonExistingFilePath);

            Should.Throw<FileNotFoundException>(() => Ensure.Exists(nonExistingFileInfo))
                .Message.ShouldBe($"Cannot find: '{nonExistingFileInfo.FullName}'.");

            nonExistingFileInfo.Create();

            var existingFileInfo = nonExistingFileInfo;

            Should.NotThrow(() => Ensure.Exists(existingFileInfo));
            Ensure.Exists(existingFileInfo).ShouldBe(existingFileInfo);

            try
            {
                existingFileInfo.Delete();
            }
            catch { /* ignored */ }
        }
    }
}