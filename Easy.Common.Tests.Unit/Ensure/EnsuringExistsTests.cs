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
            string nonExistingDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            DirectoryInfo nonExistingDirInfo = new DirectoryInfo(nonExistingDirectoryPath);

            Should.Throw<DirectoryNotFoundException>(() => Ensure.Exists(nonExistingDirInfo))
                .Message.ShouldBe($"Cannot find: '{nonExistingDirInfo.FullName}'.");

            nonExistingDirInfo.Create();

            DirectoryInfo existingDirInfo = nonExistingDirInfo;

            Should.NotThrow(() => Ensure.Exists(existingDirInfo)).ShouldBe(existingDirInfo);

            try
            {
                existingDirInfo.Delete();
            } catch { /* ignored */ }
        }

        [Test]
        public void When_ensuring_fileInfo_exists()
        {
            string nonExistingFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            FileInfo nonExistingFileInfo = new FileInfo(nonExistingFilePath);

            Should.Throw<FileNotFoundException>(() => Ensure.Exists(nonExistingFileInfo))
                .Message.ShouldBe($"Cannot find: '{nonExistingFileInfo.FullName}'.");

            nonExistingFileInfo.Create();

            FileInfo existingFileInfo = nonExistingFileInfo;

            Should.NotThrow(() => Ensure.Exists(existingFileInfo)).ShouldBe(existingFileInfo);

            try
            {
                existingFileInfo.Delete();
            } catch
            {
                /* ignored */
            }
        }
    }
}