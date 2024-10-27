namespace Easy.Common.Tests.Unit.DirectoryJunction;

using NUnit.Framework;
using Shouldly;
using System.IO;
using DirectoryJunction = Easy.Common.DirectoryJunction;

[TestFixture]
internal sealed class DirectoryJunctionTests
{
    private string? _tempFolder;

    [SetUp]
    public void CreateTempFolder()
    {
        _tempFolder = Path.GetTempFileName();
        File.Delete(_tempFolder);
        Directory.CreateDirectory(_tempFolder);
    }

    [TearDown]
    public void DeleteTempFolder()
    {
        if (_tempFolder is not null)
        {
            foreach (FileSystemInfo file in new DirectoryInfo(_tempFolder).GetFileSystemInfos())
            {
                file.Delete();
            }

            Directory.Delete(_tempFolder, true);
            _tempFolder = null;
        }
    }

    [Test]
    public void Exists_NoSuchFile()
    {
        DirectoryJunction.Exists(Path.Combine(_tempFolder, "$$$NoSuchFolder$$$")).ShouldBeFalse();
    }

    [Test]
    public void Exists_IsADirectory()
    {
        File.Create(Path.Combine(_tempFolder, "AFile")).Close();

        DirectoryJunction.Exists(Path.Combine(_tempFolder, "AFile")).ShouldBeFalse();
    }

    [Test]
    public void Create_VerifyExists_GetTarget_Delete()
    {
        string targetFolder = Path.Combine(_tempFolder, "ADirectory");
        string junctionPoint = Path.Combine(_tempFolder, "SymLink");

        Directory.CreateDirectory(targetFolder);
        File.Create(Path.Combine(targetFolder, "AFile")).Close();

        // Verify behavior before junction point created.
        File.Exists(Path.Combine(junctionPoint, "AFile")).ShouldBeFalse();

        DirectoryJunction.Exists(junctionPoint).ShouldBeFalse();

        // Create junction point and confirm its properties.
        DirectoryJunction.Create(junctionPoint, targetFolder, false /*don't overwrite*/);

        DirectoryJunction.Exists(junctionPoint).ShouldBeTrue();

        targetFolder.ShouldBe(DirectoryJunction.GetTarget(junctionPoint));

        File.Exists(Path.Combine(junctionPoint, "AFile")).ShouldBeTrue();

        // Delete junction point.
        DirectoryJunction.Delete(junctionPoint);

        DirectoryJunction.Exists(junctionPoint).ShouldBeFalse();

        File.Exists(Path.Combine(junctionPoint, "AFile")).ShouldBeFalse();

        Directory.Exists(junctionPoint).ShouldBeFalse();

        // Cleanup
        File.Delete(Path.Combine(targetFolder, "AFile"));
    }

    [Test]
    public void Create_ThrowsIfOverwriteNotSpecifiedAndDirectoryExists()
    {
        string targetFolder = Path.Combine(_tempFolder, "ADirectory");
        string junctionPoint = Path.Combine(_tempFolder, "SymLink");

        Directory.CreateDirectory(junctionPoint);

        Should.Throw<IOException>(() => DirectoryJunction.Create(junctionPoint, targetFolder, false))
            .Message.ShouldBe("Target path does not exist or is not a directory.");
    }

    [Test]
    public void Create_OverwritesIfSpecifiedAndDirectoryExists()
    {
        string targetFolder = Path.Combine(_tempFolder, "ADirectory");
        string junctionPoint = Path.Combine(_tempFolder, "SymLink");

        Directory.CreateDirectory(junctionPoint);
        Directory.CreateDirectory(targetFolder);

        DirectoryJunction.Create(junctionPoint, targetFolder, true);

        targetFolder.ShouldBe(DirectoryJunction.GetTarget(junctionPoint));
    }

    [Test]
    public void Create_ThrowsIfTargetDirectoryDoesNotExist()
    {
        string targetFolder = Path.Combine(_tempFolder, "ADirectory");
        string junctionPoint = Path.Combine(_tempFolder, "SymLink");

        Should.Throw<IOException>(() => DirectoryJunction.Create(junctionPoint, targetFolder, false))
            .Message.ShouldBe("Target path does not exist or is not a directory.");
    }

    [Test]
    public void GetTarget_NonExistentJunctionPoint()
    {
        Should.Throw<IOException>(() => DirectoryJunction.GetTarget(Path.Combine(_tempFolder, "SymLink")))
            .Message.ShouldBe("Unable to open reparse point.");
    }

    [Test]
    public void GetTarget_CalledOnADirectoryThatIsNotAJunctionPoint()
    {
        Should.Throw<IOException>(() => DirectoryJunction.GetTarget(_tempFolder))
            .Message.ShouldBe("Path is not a junction point.");
    }

    [Test]
    public void GetTarget_CalledOnAFile()
    {
        File.Create(Path.Combine(_tempFolder, "AFile")).Close();

        Should.Throw<IOException>(() => DirectoryJunction.GetTarget(Path.Combine(_tempFolder, "AFile")))
            .Message.ShouldBe("Path is not a junction point.");
    }

    [Test]
    public void Delete_NonExistentJunctionPoint()
    {
        // Should do nothing.
        DirectoryJunction.Delete(Path.Combine(_tempFolder, "SymLink"));
    }

    [Test]
    public void Delete_CalledOnADirectoryThatIsNotAJunctionPoint()
    {
        Should.Throw<IOException>(() => DirectoryJunction.Delete(_tempFolder))
            .Message.ShouldBe("Unable to delete junction point.");
    }

    [Test]
    public void Delete_CalledOnAFile()
    {
        File.Create(Path.Combine(_tempFolder, "AFile")).Close();

        Should.Throw<IOException>(() => DirectoryJunction.Delete(Path.Combine(_tempFolder, "AFile")))
            .Message.ShouldBe("Path is not a junction point.");
    }
}